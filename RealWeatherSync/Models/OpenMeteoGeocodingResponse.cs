using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// Wire format of https://geocoding-api.open-meteo.com/v1/search.
    /// Every member is nullable on purpose: the API omits fields freely and we must
    /// never throw while deserialising a partially populated result.
    /// </summary>
    public sealed class OpenMeteoGeocodingResponse
    {
        [JsonProperty("results")]
        public List<OpenMeteoGeocodingResult> Results { get; set; }

        [JsonProperty("error")]
        public bool? Error { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public sealed class OpenMeteoGeocodingResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("admin1")]
        public string Admin1 { get; set; }

        [JsonProperty("admin2")]
        public string Admin2 { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("population")]
        public long? Population { get; set; }
    }
}
