using System;
using System.Globalization;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// Provider independent view of "the weather right now" at one location.
    /// Units are normalised here so <see cref="Mapping.WeatherMapper"/> never has to
    /// know which weather service produced the values.
    /// </summary>
    public sealed class WeatherSnapshot
    {
        /// <summary>Wall clock time (UTC) at which this snapshot was received.</summary>
        public DateTime ReceivedUtc { get; set; }

        /// <summary>Observation time as reported by the provider, in the location's local time. Diagnostics only.</summary>
        public string ObservationTimeLocal { get; set; }

        /// <summary>Air temperature, degrees Celsius.</summary>
        public float TemperatureCelsius { get; set; }

        /// <summary>Total cloud cover, 0-100 %.</summary>
        public float CloudCoverPercent { get; set; }

        /// <summary>Total precipitation of the preceding hour, millimetres (rain + showers + snow water equivalent).</summary>
        public float PrecipitationMm { get; set; }

        /// <summary>Rain of the preceding hour, millimetres.</summary>
        public float RainMm { get; set; }

        /// <summary>Convective showers of the preceding hour, millimetres.</summary>
        public float ShowersMm { get; set; }

        /// <summary>Snowfall of the preceding hour, <b>centimetres</b> (Open-Meteo reports snow depth, not water equivalent).</summary>
        public float SnowfallCm { get; set; }

        /// <summary>Relative humidity, 0-100 %. Used only as a weak sanity check for fog.</summary>
        public float RelativeHumidityPercent { get; set; }

        /// <summary>WMO weather interpretation code.</summary>
        public int WeatherCode { get; set; }

        /// <summary>Horizontal visibility in metres, when the provider supplied it.</summary>
        public float? VisibilityMeters { get; set; }

        /// <summary>Daylight flag reported by the provider. Recorded for diagnostics; never used to change the game clock.</summary>
        public bool IsDay { get; set; }

        /// <summary>
        /// Hours this reading is shifted from "now" at the location: negative for the past,
        /// positive for a forecast, 0 for current conditions. Diagnostics only - shifting the
        /// weather reading never shifts the game clock.
        /// </summary>
        public int TimeShiftHours { get; set; }

        /// <summary>
        /// The hourly series around this reading, when the provider supplied one. Used by the
        /// "follow the in-game clock" mode to walk a real day hour by hour. Null is valid and
        /// simply means that mode has nothing to work with.
        /// </summary>
        public WeatherTimeline Timeline { get; set; }

        public override string ToString()
        {
            var ci = CultureInfo.InvariantCulture;
            return string.Concat(
                "temp=", TemperatureCelsius.ToString("0.0", ci), "C",
                " cloud=", CloudCoverPercent.ToString("0", ci), "%",
                " precip=", PrecipitationMm.ToString("0.00", ci), "mm",
                " rain=", RainMm.ToString("0.00", ci), "mm",
                " showers=", ShowersMm.ToString("0.00", ci), "mm",
                " snow=", SnowfallCm.ToString("0.00", ci), "cm",
                " code=", WeatherCode.ToString(ci),
                " visibility=", VisibilityMeters.HasValue ? VisibilityMeters.Value.ToString("0", ci) + "m" : "n/a",
                " isDay=", IsDay ? "1" : "0");
        }
    }
}
