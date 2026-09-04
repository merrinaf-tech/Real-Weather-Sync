using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Localization;
using RealWeatherSync.Localization.Strings;
using RealWeatherSync.Services;
using RealWeatherSync.Settings;
using RealWeatherSync.Systems;

namespace RealWeatherSync
{
    /// <summary>
    /// Entry point for Real Weather Sync.
    ///
    /// It overrides four visual climate values - temperature, cloudiness, precipitation
    /// and fog - to match the current real world weather of a city the player picks.
    /// It writes nothing else: not the clock, the date, the season, the day/night cycle,
    /// or anything serialised into a save.
    ///
    /// It is NOT true that this leaves the simulation untouched, and that claim must not
    /// be reinstated. Overridden properties are returned by
    /// OverridableProperty.op_Implicit, so ten simulation systems read the mod's values -
    /// heating demand, upkeep, fire risk, leisure, tourism, snow cover, wetness and
    /// weather events. See AGENTS.md section 3a for the verified list. The mod adds no
    /// systems and changes no rules, but it does feed the ones already there.
    /// </summary>
    public class Mod : IMod
    {
        public const string Id = "RealWeatherSync";
        public const string Name = "Real Weather Sync";
        public const string Version = "1.4.0";

        public static readonly ILog Log =
            LogManager.GetLogger(Id).SetShowsErrorsInUI(false);

        public static RealWeatherSettings Settings { get; private set; }

        public static WeatherCoordinator Coordinator { get; private set; }

        /// <summary>
        /// True after "Reset to Game Weather" until the player applies a city,
        /// forces a refresh, or toggles the mod off and on again. Session scoped:
        /// it is not persisted.
        /// </summary>
        public static bool OverridesSuspended { get; private set; }

        private static OpenMeteoClient _client;
        private static List<LocaleSource> _locales;
        private static int _resetRequested;
        private static int _skipTransitionRequested;

        /// <summary>
        /// False while the settings object is being deserialised, so the property
        /// setters do not fire side effects before everything exists.
        /// </summary>
        private static bool _ready;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info("Real Weather Sync " + Version + " loading.");

            try
            {
                ExecutableAsset asset;
                if (GameManager.instance != null && GameManager.instance.modManager != null &&
                    GameManager.instance.modManager.TryGetExecutableAsset(this, out asset))
                {
                    Log.Info("Mod assembly located at " + asset.path);
                }
            }
            catch (Exception e)
            {
                Log.Warn("Could not resolve the mod's executable asset: " + e.Message);
            }

            Settings = new RealWeatherSettings(this);
            Settings.RegisterInOptionsUI();

            RegisterLocales();

            AssetDatabase.global.LoadSettings(Id, Settings, new RealWeatherSettings(this));
            Log.Info("Settings loaded (enabled=" + Settings.EnableRealWeather +
                     ", city=\"" + Settings.CityQuery +
                     "\", interval=" + (int)Settings.UpdateInterval + " min" +
                     ", smoothing=" + Settings.SmoothTransitions + ").");

            _client = new OpenMeteoClient("RealWeatherSync/" + Version + " (Cities Skylines II mod)");
            Coordinator = new WeatherCoordinator(_client, _client, Log);

            Coordinator.TimeShiftHours = Settings.TimeShiftHours;
            StatusReport.SetOppositeDay(Settings.OppositeDay);

            var stored = Settings.BuildStoredLocation();
            if (stored != null)
            {
                Coordinator.RestoreLocation(stored);
            }
            else
            {
                StatusReport.Set(Settings.EnableRealWeather
                    ? StatusKind.CityNotConfigured
                    : StatusKind.Disabled);
            }

            updateSystem.UpdateAt<RealWeatherSystem>(SystemUpdatePhase.MainLoop);

            _ready = true;
            Log.Info("Real Weather Sync loaded.");
        }

        public void OnDispose()
        {
            Log.Info("Real Weather Sync disposing.");
            _ready = false;

            try
            {
                var system = RealWeatherSystem.Instance;
                if (system != null)
                {
                    system.ReleaseForShutdown();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to release the climate overrides during disposal.");
            }

            if (Coordinator != null)
            {
                Coordinator.Dispose();
                Coordinator = null;
            }

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

            UnregisterLocales();

            if (Settings != null)
            {
                try
                {
                    Settings.UnregisterInOptionsUI();
                }
                catch (Exception e)
                {
                    Log.Warn("Could not unregister the options page: " + e.Message);
                }

                Settings = null;
            }

            StatusReport.Reset();
            OverridesSuspended = false;
            Interlocked.Exchange(ref _resetRequested, 0);

            Log.Info("Real Weather Sync disposed.");
        }

        /// <summary>
        /// Registers one dictionary source per shipped language. The game picks whichever
        /// matches the player's interface language; the others simply sit unused, so a
        /// failure in one must not stop the rest - least of all en-US.
        /// </summary>
        private static void RegisterLocales()
        {
            _locales = new List<LocaleSource>();

            var manager = GameManager.instance != null ? GameManager.instance.localizationManager : null;
            if (manager == null)
            {
                Log.Warn("No localisation manager available; the mod will fall back to its built-in English.");
                return;
            }

            foreach (var table in LocaleTables.All())
            {
                try
                {
                    var source = new LocaleSource(Settings, table);
                    manager.AddSource(source.LocaleId, source);
                    _locales.Add(source);
                }
                catch (Exception e)
                {
                    Log.Warn("Could not register the " + table.LocaleId + " translation: " + e.Message);
                }
            }

            Log.Info("Registered " + _locales.Count + " translations.");
        }

        private static void UnregisterLocales()
        {
            if (_locales == null)
            {
                return;
            }

            var manager = GameManager.instance != null ? GameManager.instance.localizationManager : null;
            if (manager != null)
            {
                foreach (var source in _locales)
                {
                    try
                    {
                        manager.RemoveSource(source.LocaleId, source);
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Could not remove the " + source.LocaleId + " translation: " + e.Message);
                    }
                }
            }

            _locales = null;
        }

        // ------------------------------------------------------------------
        // Callbacks from the options page. All of these run on the main thread.
        // ------------------------------------------------------------------

        internal static void OnEnabledChanged(bool enabled)
        {
            if (!_ready)
            {
                return;
            }

            Log.Info(enabled ? "Real weather enabled by the player." : "Real weather disabled by the player.");

            if (!enabled)
            {
                return;
            }

            OverridesSuspended = false;

            if (Coordinator != null)
            {
                Coordinator.InvalidateSchedule();
                Coordinator.RearmSnapshotPickup();
            }
        }

        internal static void OnApplyCityPressed()
        {
            if (!_ready || Settings == null || Coordinator == null)
            {
                return;
            }

            OverridesSuspended = false;

            try
            {
                // Persist the typed query even if the lookup ends up failing.
                Settings.ApplyAndSave();
            }
            catch (Exception e)
            {
                Log.Warn("Could not save the settings before applying the city: " + e.Message);
            }

            Coordinator.RequestApplyCity(Settings.CityQuery);
        }

        internal static void OnRefreshNowPressed()
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            OverridesSuspended = false;
            Coordinator.InvalidateSchedule();
            Coordinator.RequestImmediateRefresh();
        }

        /// <summary>Refresh, then snap to the result instead of fading into it.</summary>
        internal static void OnApplyImmediatelyPressed()
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            Log.Info("Immediate apply requested by the player.");
            OverridesSuspended = false;
            Interlocked.Exchange(ref _skipTransitionRequested, 1);
            Coordinator.InvalidateSchedule();
            Coordinator.RequestImmediateRefresh();
        }

        internal static void OnSearchCityPressed()
        {
            if (!_ready || Settings == null || Coordinator == null)
            {
                return;
            }

            try
            {
                Settings.ApplyAndSave();
            }
            catch (Exception e)
            {
                Log.Warn("Could not save the settings before searching: " + e.Message);
            }

            Coordinator.RequestSearch(Settings.CityQuery);
        }

        /// <summary>
        /// A city was picked from the search results dropdown. The value is the index into
        /// <see cref="WeatherCoordinator.Candidates"/>; an empty value is the "select..." row.
        /// </summary>
        internal static void OnSearchResultSelected(string value)
        {
            if (!_ready || Coordinator == null || string.IsNullOrEmpty(value))
            {
                return;
            }

            var candidates = Coordinator.Candidates;
            int index;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
            {
                return;
            }

            if (candidates == null || index < 0 || index >= candidates.Count)
            {
                return;
            }

            OverridesSuspended = false;
            Coordinator.ApplyCandidate(candidates[index]);
        }

        /// <summary>A city was picked from the recent-cities dropdown.</summary>
        internal static void OnFavouriteSelected(string value)
        {
            if (!_ready || Settings == null || Coordinator == null || string.IsNullOrEmpty(value))
            {
                return;
            }

            var favourites = Settings.Favourites;
            int index;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
            {
                return;
            }

            if (index < 0 || index >= favourites.Count)
            {
                return;
            }

            OverridesSuspended = false;
            Coordinator.ApplyCandidate(favourites[index]);
        }

        internal static void OnFollowGameClockChanged(bool enabled)
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            Log.Info(enabled
                ? "Following the in-game clock: weather now tracks the last 24 hours by in-game hour."
                : "Following the in-game clock disabled: back to the current reading.");

            // The manual shift is ignored while the clock drives the choice of hour.
            Coordinator.TimeShiftHours = enabled ? 0 : (Settings != null ? Settings.TimeShiftHours : 0);
            Coordinator.RearmSnapshotPickup();
        }

        internal static void OnAntipodeModeChanged(bool enabled)
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            Log.Info(enabled
                ? "Antipode mode on: fetching the weather from the opposite side of the planet."
                : "Antipode mode off.");

            StatusReport.SetAntipode(enabled);
            Coordinator.AntipodeMode = enabled;
        }

        /// <summary>Jump to one of the built-in extreme locations. No geocoding involved.</summary>
        internal static void OnExtremeLocationSelected(ExtremeLocationOption option)
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            var location = ExtremeLocations.Get(option);
            if (location == null)
            {
                return;
            }

            Log.Info("Extreme location selected: " + location.DisplayName + ".");
            OverridesSuspended = false;
            Coordinator.ApplyCandidate(location);
        }

        internal static void OnTimeShiftChanged(int hours)
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            Log.Info("Weather time shift set to " + hours + " h (the game clock is not affected).");
            Coordinator.TimeShiftHours = hours;
        }

        /// <summary>Consumed once by the system: apply the next target without fading.</summary>
        internal static bool ConsumeSkipTransitionRequest()
        {
            return Interlocked.Exchange(ref _skipTransitionRequested, 0) == 1;
        }

        internal static void OnResetToGameWeatherPressed()
        {
            if (!_ready)
            {
                return;
            }

            Log.Info("Reset to game weather requested by the player.");
            OverridesSuspended = true;
            Interlocked.Exchange(ref _resetRequested, 1);
        }

        internal static void OnUpdateIntervalChanged()
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            Log.Info("Update interval changed to " + (int)Settings.UpdateInterval + " minutes.");
            Coordinator.InvalidateSchedule();
        }

        /// <summary>
        /// A mapping preference changed, so the last snapshot has to be run through
        /// <see cref="Mapping.WeatherMapper"/> again.
        /// </summary>
        internal static void OnMappingOptionsChanged()
        {
            if (!_ready || Coordinator == null)
            {
                return;
            }

            if (Settings != null)
            {
                StatusReport.SetOppositeDay(Settings.OppositeDay);
            }

            Coordinator.RearmSnapshotPickup();
        }

        /// <summary>Consumed once by <see cref="RealWeatherSystem"/> on the main thread.</summary>
        internal static bool ConsumeResetRequest()
        {
            return Interlocked.Exchange(ref _resetRequested, 0) == 1;
        }
    }
}
