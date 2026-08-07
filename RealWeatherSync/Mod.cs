using System;
using System.Threading;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Localization;
using RealWeatherSync.Services;
using RealWeatherSync.Settings;
using RealWeatherSync.Systems;

namespace RealWeatherSync
{
    /// <summary>
    /// Entry point for Real Weather Sync.
    ///
    /// Real Weather Sync is a purely cosmetic mod. It overrides four visual climate
    /// values - temperature, cloudiness, precipitation and fog - to match the
    /// current real world weather of a city the player picks. It does not touch the
    /// simulation, the clock, the date, the season, the day/night cycle, disasters,
    /// or anything that is written into a save.
    /// </summary>
    public class Mod : IMod
    {
        public const string Id = "RealWeatherSync";
        public const string Name = "Real Weather Sync";
        public const string Version = "1.0.0";

        private const string LocaleId = "en-US";

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
        private static LocaleEN _locale;
        private static int _resetRequested;

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

            _locale = new LocaleEN(Settings);
            GameManager.instance.localizationManager.AddSource(LocaleId, _locale);

            AssetDatabase.global.LoadSettings(Id, Settings, new RealWeatherSettings(this));
            Log.Info("Settings loaded (enabled=" + Settings.EnableRealWeather +
                     ", city=\"" + Settings.CityQuery +
                     "\", interval=" + (int)Settings.UpdateInterval + " min" +
                     ", smoothing=" + Settings.SmoothTransitions + ").");

            _client = new OpenMeteoClient("RealWeatherSync/" + Version + " (Cities Skylines II mod)");
            Coordinator = new WeatherCoordinator(_client, _client, Log);

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

            if (_locale != null)
            {
                try
                {
                    if (GameManager.instance != null && GameManager.instance.localizationManager != null)
                    {
                        GameManager.instance.localizationManager.RemoveSource(LocaleId, _locale);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Could not remove the localisation source: " + e.Message);
                }

                _locale = null;
            }

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

            Coordinator.RearmSnapshotPickup();
        }

        /// <summary>Consumed once by <see cref="RealWeatherSystem"/> on the main thread.</summary>
        internal static bool ConsumeResetRequest()
        {
            return Interlocked.Exchange(ref _resetRequested, 0) == 1;
        }
    }
}
