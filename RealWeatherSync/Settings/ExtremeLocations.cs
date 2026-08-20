using RealWeatherSync.Models;

namespace RealWeatherSync.Settings
{
    /// <summary>
    /// Famously miserable places, offered as one-click jumps.
    ///
    /// Half joke, half tool: these are the quickest way to see the snow, fog and heavy-rain
    /// branches of the mapping without waiting for the weather at home to cooperate.
    /// </summary>
    public enum ExtremeLocationOption
    {
        None = 0,
        Yakutsk,
        Longyearbyen,
        Ushuaia,
        Reykjavik,
        MountWashington,
        DeathValley,
        Cherrapunji
    }

    /// <summary>
    /// Coordinates for <see cref="ExtremeLocationOption"/>. Hardcoded on purpose: picking a
    /// preset must not cost a geocoding request.
    /// </summary>
    public static class ExtremeLocations
    {
        /// <summary>Returns the location for a preset, or null for <see cref="ExtremeLocationOption.None"/>.</summary>
        public static LocationResult Get(ExtremeLocationOption option)
        {
            switch (option)
            {
                case ExtremeLocationOption.Yakutsk:
                    return Make("Yakutsk", "Sakha Republic", "Russia", "RU", 62.0339, 129.7331);
                case ExtremeLocationOption.Longyearbyen:
                    return Make("Longyearbyen", "Svalbard", "Norway", "NO", 78.2232, 15.6267);
                case ExtremeLocationOption.Ushuaia:
                    return Make("Ushuaia", "Tierra del Fuego", "Argentina", "AR", -54.8019, -68.3030);
                case ExtremeLocationOption.Reykjavik:
                    return Make("Reykjavik", "Capital Region", "Iceland", "IS", 64.1355, -21.8954);
                case ExtremeLocationOption.MountWashington:
                    return Make("Mount Washington", "New Hampshire", "United States", "US", 44.2705, -71.3033);
                case ExtremeLocationOption.DeathValley:
                    return Make("Death Valley", "California", "United States", "US", 36.5323, -116.9325);
                case ExtremeLocationOption.Cherrapunji:
                    return Make("Cherrapunji", "Meghalaya", "India", "IN", 25.3000, 91.5800);
                default:
                    return null;
            }
        }

        /// <summary>One-line reason each place earns its spot, for the options tooltip.</summary>
        public static string DescribeEnglish(ExtremeLocationOption option)
        {
            switch (option)
            {
                case ExtremeLocationOption.Yakutsk:
                    return "Yakutsk - the coldest city on Earth";
                case ExtremeLocationOption.Longyearbyen:
                    return "Longyearbyen - polar night, polar day, polar bears";
                case ExtremeLocationOption.Ushuaia:
                    return "Ushuaia - the end of the world, and it shows";
                case ExtremeLocationOption.Reykjavik:
                    return "Reykjavik - wind and rain, sideways";
                case ExtremeLocationOption.MountWashington:
                    return "Mount Washington - fog, and the worst weather in America";
                case ExtremeLocationOption.DeathValley:
                    return "Death Valley - the hottest place ever recorded";
                case ExtremeLocationOption.Cherrapunji:
                    return "Cherrapunji - one of the wettest places on Earth";
                default:
                    return "Stay where you are";
            }
        }

        private static LocationResult Make(string name, string admin1, string country, string countryCode,
            double latitude, double longitude)
        {
            // Query is set to the display name so the recent-cities list shows something sensible.
            return new LocationResult(name + ", " + country, name, admin1, country, countryCode,
                string.Empty, latitude, longitude);
        }
    }
}
