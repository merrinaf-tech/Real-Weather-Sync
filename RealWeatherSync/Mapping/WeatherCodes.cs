namespace RealWeatherSync.Mapping
{
    /// <summary>
    /// Turns a WMO weather interpretation code into a locale id, so the status panel can say
    /// "Light rain" instead of "code 61".
    ///
    /// Deliberately free of game types: this hands back a key plus an English fallback, and the
    /// caller resolves it through the localisation system.
    /// </summary>
    public static class WeatherCodes
    {
        public const string Prefix = "RealWeatherSync.Wmo.";

        /// <summary>
        /// Every condition this class knows about: locale-id suffix paired with its English text.
        /// Single source of truth - <see cref="LocaleIdFor"/>, <see cref="EnglishFor"/> and
        /// <see cref="All"/> all read from here.
        /// </summary>
        private static readonly string[,] Table =
        {
            { "Clear", "Clear sky" },
            { "MainlyClear", "Mainly clear" },
            { "PartlyCloudy", "Partly cloudy" },
            { "Overcast", "Overcast" },
            { "Fog", "Fog" },
            { "RimeFog", "Depositing rime fog" },
            { "DrizzleLight", "Light drizzle" },
            { "DrizzleModerate", "Moderate drizzle" },
            { "DrizzleDense", "Dense drizzle" },
            { "FreezingDrizzleLight", "Light freezing drizzle" },
            { "FreezingDrizzleDense", "Dense freezing drizzle" },
            { "RainSlight", "Slight rain" },
            { "RainModerate", "Moderate rain" },
            { "RainHeavy", "Heavy rain" },
            { "FreezingRainLight", "Light freezing rain" },
            { "FreezingRainHeavy", "Heavy freezing rain" },
            { "SnowSlight", "Slight snow" },
            { "SnowModerate", "Moderate snow" },
            { "SnowHeavy", "Heavy snow" },
            { "SnowGrains", "Snow grains" },
            { "ShowersSlight", "Slight rain showers" },
            { "ShowersModerate", "Moderate rain showers" },
            { "ShowersViolent", "Violent rain showers" },
            { "SnowShowersSlight", "Slight snow showers" },
            { "SnowShowersHeavy", "Heavy snow showers" },
            { "Thunderstorm", "Thunderstorm" },
            { "ThunderstormHailSlight", "Thunderstorm with slight hail" },
            { "ThunderstormHailHeavy", "Thunderstorm with heavy hail" },
            { "Unknown", "Unknown conditions" }
        };

        /// <summary>Locale id for a WMO code; falls back to the "unknown" id.</summary>
        public static string LocaleIdFor(int weatherCode)
        {
            return Prefix + SuffixFor(weatherCode);
        }

        /// <summary>English text for a WMO code, used as the localisation fallback.</summary>
        public static string EnglishFor(int weatherCode)
        {
            return EnglishForSuffix(SuffixFor(weatherCode));
        }

        /// <summary>All (locale id, English text) pairs, so LocaleEN can register them in one loop.</summary>
        public static System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> All()
        {
            for (var i = 0; i < Table.GetLength(0); i++)
            {
                yield return new System.Collections.Generic.KeyValuePair<string, string>(
                    Prefix + Table[i, 0], Table[i, 1]);
            }
        }

        private static string EnglishForSuffix(string suffix)
        {
            for (var i = 0; i < Table.GetLength(0); i++)
            {
                if (Table[i, 0] == suffix)
                {
                    return Table[i, 1];
                }
            }

            return Table[Table.GetLength(0) - 1, 1];
        }

        private static string SuffixFor(int weatherCode)
        {
            switch (weatherCode)
            {
                case 0: return "Clear";
                case 1: return "MainlyClear";
                case 2: return "PartlyCloudy";
                case 3: return "Overcast";
                case 45: return "Fog";
                case 48: return "RimeFog";
                case 51: return "DrizzleLight";
                case 53: return "DrizzleModerate";
                case 55: return "DrizzleDense";
                case 56: return "FreezingDrizzleLight";
                case 57: return "FreezingDrizzleDense";
                case 61: return "RainSlight";
                case 63: return "RainModerate";
                case 65: return "RainHeavy";
                case 66: return "FreezingRainLight";
                case 67: return "FreezingRainHeavy";
                case 71: return "SnowSlight";
                case 73: return "SnowModerate";
                case 75: return "SnowHeavy";
                case 77: return "SnowGrains";
                case 80: return "ShowersSlight";
                case 81: return "ShowersModerate";
                case 82: return "ShowersViolent";
                case 85: return "SnowShowersSlight";
                case 86: return "SnowShowersHeavy";
                case 95: return "Thunderstorm";
                case 96: return "ThunderstormHailSlight";
                case 99: return "ThunderstormHailHeavy";
                default: return "Unknown";
            }
        }
    }
}
