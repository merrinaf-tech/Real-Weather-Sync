using System;
using Game.Simulation;
using RealWeatherSync.Models;

namespace RealWeatherSync.Systems
{
    /// <summary>
    /// The only place in the mod that writes to Game.Simulation.ClimateSystem.
    ///
    /// It touches exactly four overridable properties - temperature, cloudiness,
    /// precipitation and fog - and remembers which of them it switched on, so
    /// releasing never clears an override that belongs to somebody else.
    ///
    /// Explicitly NOT touched: currentDate (season / date), aurora, thunder,
    /// rainbow, hail, wind, latitude / longitude, or anything outside ClimateSystem.
    ///
    /// Must only be used from the main thread.
    /// </summary>
    public sealed class ClimateOverrideController
    {
        private readonly ClimateSystem _climateSystem;

        private bool _temperatureOverridden;
        private bool _cloudinessOverridden;
        private bool _precipitationOverridden;
        private bool _fogOverridden;

        public ClimateOverrideController(ClimateSystem climateSystem)
        {
            if (climateSystem == null)
            {
                throw new ArgumentNullException("climateSystem");
            }

            _climateSystem = climateSystem;
        }

        /// <summary>True when at least one override is currently owned by this mod.</summary>
        public bool IsActive
        {
            get { return _temperatureOverridden || _cloudinessOverridden || _precipitationOverridden || _fogOverridden; }
        }

        /// <summary>
        /// The temperature at which the game switches precipitation from rain to snow.
        /// Read from the game rather than assumed to be 0 C.
        /// </summary>
        public float FreezingTemperatureCelsius
        {
            get
            {
                try
                {
                    return _climateSystem.freezingTemperature;
                }
                catch (Exception)
                {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// The values the game is showing right now. Used as the starting point of
        /// the first transition so switching the mod on fades in from the weather
        /// the player can currently see.
        /// </summary>
        public ClimateTarget ReadCurrent()
        {
            ClimateTarget current;
            current.TemperatureCelsius = _climateSystem.temperature;
            current.Cloudiness = _climateSystem.cloudiness;
            current.Precipitation = _climateSystem.precipitation;
            current.Fog = _climateSystem.fog;
            return current;
        }

        /// <summary>
        /// Pushes the given visual values into the climate system.
        ///
        /// Temperature and fog are opt-out because they behave differently from the rest:
        /// temperature is the value the largest number of game systems read (see README,
        /// "What the game reads back"), and fog is the only one nothing outside rendering reads.
        /// </summary>
        public void Apply(ClimateTarget target, bool includeFog, bool includeTemperature)
        {
            if (includeTemperature)
            {
                _climateSystem.temperature.overrideValue = target.TemperatureCelsius;
                _climateSystem.temperature.overrideState = true;
                _temperatureOverridden = true;
            }
            else if (_temperatureOverridden)
            {
                _climateSystem.temperature.overrideState = false;
                _temperatureOverridden = false;
            }

            _climateSystem.cloudiness.overrideValue = target.Cloudiness;
            _climateSystem.cloudiness.overrideState = true;
            _cloudinessOverridden = true;

            _climateSystem.precipitation.overrideValue = target.Precipitation;
            _climateSystem.precipitation.overrideState = true;
            _precipitationOverridden = true;

            if (includeFog)
            {
                _climateSystem.fog.overrideValue = target.Fog;
                _climateSystem.fog.overrideState = true;
                _fogOverridden = true;
            }
            else if (_fogOverridden)
            {
                _climateSystem.fog.overrideState = false;
                _fogOverridden = false;
            }
        }

        /// <summary>
        /// Hands every property we own back to the game. Idempotent, and safe to call
        /// from teardown paths.
        /// </summary>
        /// <returns>True when something was actually released.</returns>
        public bool Release()
        {
            var releasedAnything = false;

            if (_temperatureOverridden)
            {
                _climateSystem.temperature.overrideState = false;
                _temperatureOverridden = false;
                releasedAnything = true;
            }

            if (_cloudinessOverridden)
            {
                _climateSystem.cloudiness.overrideState = false;
                _cloudinessOverridden = false;
                releasedAnything = true;
            }

            if (_precipitationOverridden)
            {
                _climateSystem.precipitation.overrideState = false;
                _precipitationOverridden = false;
                releasedAnything = true;
            }

            if (_fogOverridden)
            {
                _climateSystem.fog.overrideState = false;
                _fogOverridden = false;
                releasedAnything = true;
            }

            return releasedAnything;
        }
    }
}
