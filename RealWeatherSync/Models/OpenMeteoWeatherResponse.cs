using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// Wire format of https://api.open-meteo.com/v1/forecast for the subset of
    /// variables Real Weather Sync requests. All members nullable - see
    /// <see cref="OpenMeteoGeocodingResponse"/> for the rationale.
    /// </summary>
    public sealed class OpenMeteoWeatherResponse
    {
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("current")]
        public OpenMeteoCurrent Current { get; set; }

        [JsonProperty("hourly")]
        public OpenMeteoHourly Hourly { get; set; }

        [JsonProperty("error")]
        public bool? Error { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public sealed class OpenMeteoCurrent
    {
        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("temperature_2m")]
        public float? Temperature2m { get; set; }

        [JsonProperty("relative_humidity_2m")]
        public float? RelativeHumidity2m { get; set; }

        [JsonProperty("is_day")]
        public int? IsDay { get; set; }

        [JsonProperty("precipitation")]
        public float? Precipitation { get; set; }

        [JsonProperty("rain")]
        public float? Rain { get; set; }

        [JsonProperty("showers")]
        public float? Showers { get; set; }

        /// <summary>Centimetres of fresh snow, not water equivalent.</summary>
        [JsonProperty("snowfall")]
        public float? Snowfall { get; set; }

        [JsonProperty("weather_code")]
        public int? WeatherCode { get; set; }

        [JsonProperty("cloud_cover")]
        public float? CloudCover { get; set; }
    }

    /// <summary>
    /// Hourly series. Visibility is always requested because Open-Meteo exposes it as an
    /// hourly-only variable; the remaining series are only populated when a time shift is in
    /// effect and the reading has to come from a past or forecast hour.
    /// </summary>
    public sealed class OpenMeteoHourly
    {
        [JsonProperty("time")]
        public List<string> Time { get; set; }

        /// <summary>Metres.</summary>
        [JsonProperty("visibility")]
        public List<float?> Visibility { get; set; }

        [JsonProperty("temperature_2m")]
        public List<float?> Temperature2m { get; set; }

        [JsonProperty("relative_humidity_2m")]
        public List<float?> RelativeHumidity2m { get; set; }

        [JsonProperty("precipitation")]
        public List<float?> Precipitation { get; set; }

        [JsonProperty("rain")]
        public List<float?> Rain { get; set; }

        [JsonProperty("showers")]
        public List<float?> Showers { get; set; }

        /// <summary>Centimetres of fresh snow, not water equivalent.</summary>
        [JsonProperty("snowfall")]
        public List<float?> Snowfall { get; set; }

        [JsonProperty("weather_code")]
        public List<int?> WeatherCode { get; set; }

        [JsonProperty("cloud_cover")]
        public List<float?> CloudCover { get; set; }

        [JsonProperty("is_day")]
        public List<int?> IsDay { get; set; }
    }
}
