using System;
using System.Collections.Generic;
using System.Diagnostics;
using Colossal.Serialization.Entities;
using Game;
using Game.Simulation;
using RealWeatherSync.Compatibility;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Mapping;
using RealWeatherSync.Models;
using RealWeatherSync.Settings;

namespace RealWeatherSync.Systems
{
    /// <summary>
    /// Drives the visual climate from the last weather Open-Meteo returned.
    ///
    /// Runs in SystemUpdatePhase.MainLoop. Everything it does happens on the main
    /// thread: it polls <see cref="Services.WeatherCoordinator"/> for results that
    /// were fetched in the background, interpolates towards them using real
    /// (wall clock) time, and writes through <see cref="ClimateOverrideController"/>.
    ///
    /// Overrides are only applied when all of these hold:
    ///   - a normal game is loaded (not the editor, not the main menu)
    ///   - the mod is enabled
    ///   - a city has been resolved
    ///   - at least one weather response has arrived
    ///   - no incompatible weather mod was detected (or the player opted out of the check)
    /// </summary>
    public partial class RealWeatherSystem : GameSystemBase
    {
        /// <summary>Re-assert the override values at least this often, in seconds.</summary>
        private const double HeartbeatSeconds = 2.0;

        /// <summary>Below this, two climate values are treated as identical.</summary>
        private const float ValueEpsilon = 0.0005f;

        private ClimateSystem _climateSystem;
        private ClimateOverrideController _controller;

        private readonly Stopwatch _realTime = Stopwatch.StartNew();

        private bool _isGame;
        private bool _conflictChecked;
        private bool _conflictDetected;
        private string _conflictNames = string.Empty;

        private ClimateTarget _from;
        private ClimateTarget _to;
        private ClimateTarget _applied;
        private ClimateTarget _lastWritten;

        private bool _hasTarget;
        private bool _hasApplied;
        private bool _hasWritten;

        private double _transitionStartSeconds;
        private double _transitionDurationSeconds;

        private double _lastWriteSeconds;
        private bool _loggedOverridesActive;

        /// <summary>
        /// Set while the game is running so <see cref="Mod"/> can release overrides
        /// during disposal. Cleared in OnDestroy.
        /// </summary>
        public static RealWeatherSystem Instance { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();

            Instance = this;

            try
            {
                _climateSystem = World.GetOrCreateSystemManaged<ClimateSystem>();
                _controller = new ClimateOverrideController(_climateSystem);
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, "Could not obtain ClimateSystem; Real Weather Sync will stay inactive.");
                _controller = null;
                Enabled = false;
            }
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);

            // A load is starting: hand the climate back before the world changes.
            ReleaseOverrides("game preload");
            ResetTransition();
            _conflictChecked = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            _isGame = mode.IsGame();
            _conflictChecked = false;
            ResetTransition();

            if (!_isGame)
            {
                ReleaseOverrides("not a normal game session (" + mode + ")");
                StatusReport.Set(StatusKind.WaitingForGame);
                return;
            }

            Mod.Log.Info("Game loaded; Real Weather Sync is armed.");

            // A freshly loaded city should get current weather promptly rather than
            // waiting out the remainder of the previous interval.
            var coordinator = Mod.Coordinator;
            if (coordinator != null)
            {
                coordinator.InvalidateSchedule();
                coordinator.RearmSnapshotPickup();
            }
        }

        protected override void OnUpdate()
        {
            if (_controller == null)
            {
                return;
            }

            var settings = Mod.Settings;
            var coordinator = Mod.Coordinator;
            if (settings == null || coordinator == null)
            {
                return;
            }

            if (!_isGame)
            {
                ReleaseOverrides("no city loaded");
                StatusReport.Set(StatusKind.WaitingForGame);
                return;
            }

            if (!settings.EnableRealWeather)
            {
                ReleaseOverrides("mod disabled");
                ResetTransition();
                StatusReport.Set(StatusKind.Disabled);
                return;
            }

            if (Mod.ConsumeResetRequest())
            {
                ReleaseOverrides("reset requested by the player");
                ResetTransition();
                StatusReport.Set(StatusKind.ReleasedByPlayer);
                return;
            }

            if (Mod.OverridesSuspended)
            {
                StatusReport.Set(StatusKind.ReleasedByPlayer);
                return;
            }

            if (!EnsureNoConflict(settings))
            {
                return;
            }

            // A location resolved on a background thread is written into the
            // settings here, on the main thread.
            LocationResult resolved;
            if (coordinator.TryTakeLocation(out resolved))
            {
                try
                {
                    // Set the query first: StoreLocation is what writes the file.
                    settings.CityQuery = resolved.Query;
                    settings.StoreLocation(resolved);
                }
                catch (Exception e)
                {
                    Mod.Log.Error(e, "Could not persist the resolved location.");
                }
            }

            if (!coordinator.HasLocation)
            {
                ReleaseOverrides("no city configured");
                StatusReport.Set(StatusKind.CityNotConfigured);
                return;
            }

            coordinator.Tick(settings.UpdateIntervalSeconds);

            WeatherSnapshot snapshot;
            if (coordinator.TryTakeSnapshot(out snapshot))
            {
                BeginTransitionTo(snapshot, settings);
            }

            if (!_hasTarget)
            {
                // A city is known but no weather has arrived yet; leave the game's
                // own weather alone rather than applying zeroed values.
                return;
            }

            AdvanceAndApply(settings);
        }

        /// <summary>
        /// Runs the compatibility check once per load. Returns false when Real
        /// Weather Sync must stay out of the way.
        /// </summary>
        private bool EnsureNoConflict(RealWeatherSettings settings)
        {
            if (!_conflictChecked)
            {
                _conflictChecked = true;

                List<string> conflicts;
                _conflictDetected = WeatherModCompatibility.TryDetectConflicts(out conflicts);
                _conflictNames = WeatherModCompatibility.Describe(conflicts);

                if (_conflictDetected)
                {
                    Mod.Log.Warn("Incompatible weather mod detected: " + _conflictNames +
                                 ". Real Weather Sync will not override the climate while it is active. " +
                                 "Disable the other mod, or enable \"Ignore mod conflicts\" if you know what you are doing.");
                }
            }

            if (!_conflictDetected || settings.IgnoreModConflicts)
            {
                return true;
            }

            ReleaseOverrides("incompatible weather mod active");
            ResetTransition();
            StatusReport.Set(StatusKind.IncompatibleModActive, _conflictNames);
            return false;
        }

        private void BeginTransitionTo(WeatherSnapshot snapshot, RealWeatherSettings settings)
        {
            WeatherMappingOptions options;
            options.SyncFog = settings.SyncFog;
            options.ForceSnowAppearance = settings.ForceSnowAppearance;
            options.FreezingTemperatureCelsius = _controller.FreezingTemperatureCelsius;

            var target = WeatherMapper.Map(snapshot, options);

            // A new result arriving mid-transition continues from wherever the
            // interpolation currently is, never from the previous start value.
            _from = _hasApplied ? _applied : ReadCurrentSafely();
            _to = target;
            _hasTarget = true;

            _transitionStartSeconds = NowSeconds;
            _transitionDurationSeconds = settings.SmoothTransitions
                ? RealWeatherSettings.TransitionDurationSeconds
                : 0.0;

            StatusReport.RecordTarget(target);
            Mod.Log.Info("Mapped target values: " + target +
                         " (from " + snapshot + ", transition " +
                         _transitionDurationSeconds.ToString("0", System.Globalization.CultureInfo.InvariantCulture) + " s)");
        }

        private void AdvanceAndApply(RealWeatherSettings settings)
        {
            var now = NowSeconds;

            float t;
            if (_transitionDurationSeconds <= 0.0)
            {
                t = 1f;
            }
            else
            {
                var elapsed = now - _transitionStartSeconds;
                if (elapsed <= 0.0)
                {
                    t = 0f;
                }
                else if (elapsed >= _transitionDurationSeconds)
                {
                    t = 1f;
                }
                else
                {
                    t = (float)(elapsed / _transitionDurationSeconds);
                }
            }

            _applied = ClimateTarget.Lerp(_from, _to, t);
            _hasApplied = true;

            var transitionRunning = t < 1f;
            var heartbeatDue = now - _lastWriteSeconds >= HeartbeatSeconds;

            if (!_hasWritten || !_controller.IsActive || transitionRunning || heartbeatDue || Differs(_applied, _lastWritten))
            {
                try
                {
                    _controller.Apply(_applied, settings.SyncFog);
                }
                catch (Exception e)
                {
                    Mod.Log.Error(e, "Failed to write the climate overrides; disabling Real Weather Sync's overrides.");
                    ReleaseOverrides("write failure");
                    _hasTarget = false;
                    return;
                }

                _lastWritten = _applied;
                _hasWritten = true;
                _lastWriteSeconds = now;

                if (!_loggedOverridesActive)
                {
                    _loggedOverridesActive = true;
                    StatusReport.SetOverridesActive(true);
                    Mod.Log.Info("Climate overrides activated.");
                }
            }

            if (!transitionRunning)
            {
                StatusReport.RecordTarget(_applied);
            }
        }

        private ClimateTarget ReadCurrentSafely()
        {
            try
            {
                return _controller.ReadCurrent();
            }
            catch (Exception e)
            {
                Mod.Log.Warn("Could not read the current climate values (" + e.Message + "); starting the transition from the target.");
                return _to;
            }
        }

        private static bool Differs(ClimateTarget a, ClimateTarget b)
        {
            return Math.Abs(a.TemperatureCelsius - b.TemperatureCelsius) > ValueEpsilon
                   || Math.Abs(a.Cloudiness - b.Cloudiness) > ValueEpsilon
                   || Math.Abs(a.Precipitation - b.Precipitation) > ValueEpsilon
                   || Math.Abs(a.Fog - b.Fog) > ValueEpsilon;
        }

        private double NowSeconds
        {
            get { return _realTime.Elapsed.TotalSeconds; }
        }

        private void ResetTransition()
        {
            _hasTarget = false;
            _hasApplied = false;
            _hasWritten = false;
            _transitionDurationSeconds = 0.0;
        }

        /// <summary>Releases the overrides if we hold any, and logs the reason once.</summary>
        public void ReleaseOverrides(string reason)
        {
            if (_controller == null)
            {
                return;
            }

            bool released;
            try
            {
                released = _controller.Release();
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, "Failed to release the climate overrides.");
                return;
            }

            _hasWritten = false;

            if (!released && !_loggedOverridesActive)
            {
                return;
            }

            _loggedOverridesActive = false;
            StatusReport.SetOverridesActive(false);

            if (released)
            {
                Mod.Log.Info("Climate overrides deactivated: " + reason + ".");
            }
        }

        /// <summary>
        /// Called from <see cref="Mod.OnDispose"/>. Restores the game's own weather so
        /// nothing is left behind when the mod goes away.
        /// </summary>
        public void ReleaseForShutdown()
        {
            ReleaseOverrides("mod disposed");
            ResetTransition();
        }

        protected override void OnDestroy()
        {
            ReleaseOverrides("system destroyed");

            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }

            base.OnDestroy();
        }
    }
}
