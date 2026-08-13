using System;
using System.Globalization;
using Game.SceneFlow;
using Game.Settings;

namespace RealWeatherSync.Diagnostics
{
    /// <summary>
    /// Formats a Celsius value using whatever unit the player selected in the game's own
    /// interface settings, so the mod's status panel does not contradict the rest of the UI.
    ///
    /// Only the *display* is converted. Everything internal - the mapping, the clamps, the value
    /// handed to ClimateSystem - stays in Celsius, which is what the game expects.
    /// </summary>
    public static class TemperatureDisplay
    {
        /// <summary>Reads the game's preference, defaulting to Celsius if it cannot be read.</summary>
        public static InterfaceSettings.TemperatureUnit CurrentUnit
        {
            get
            {
                try
                {
                    var manager = GameManager.instance;
                    var settings = manager != null ? manager.settings : null;
                    var ui = settings != null ? settings.userInterface : null;
                    if (ui != null)
                    {
                        return ui.temperatureUnit;
                    }
                }
                catch (Exception)
                {
                    // The options page must never break over a display preference.
                }

                return InterfaceSettings.TemperatureUnit.Celsius;
            }
        }

        /// <summary>"18.3 °C", "64.9 °F" or "291.5 K", per the player's preference.</summary>
        public static string Format(float celsius)
        {
            return Format(celsius, CurrentUnit);
        }

        public static string Format(float celsius, InterfaceSettings.TemperatureUnit unit)
        {
            var ci = CultureInfo.CurrentCulture;

            switch (unit)
            {
                case InterfaceSettings.TemperatureUnit.Fahrenheit:
                    return ToFahrenheit(celsius).ToString("0.0", ci) + " °F";
                case InterfaceSettings.TemperatureUnit.Kelvin:
                    return ToKelvin(celsius).ToString("0.0", ci) + " K";
                default:
                    return celsius.ToString("0.0", ci) + " °C";
            }
        }

        public static float ToFahrenheit(float celsius)
        {
            return celsius * 9f / 5f + 32f;
        }

        public static float ToKelvin(float celsius)
        {
            return celsius + 273.15f;
        }
    }
}
