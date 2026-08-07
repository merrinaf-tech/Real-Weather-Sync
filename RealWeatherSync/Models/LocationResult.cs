using System;
using System.Text;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// A geographical location resolved from a user supplied city name.
    /// Immutable; produced by <see cref="Services.ILocationService"/>.
    /// </summary>
    public sealed class LocationResult
    {
        public LocationResult(string query, string name, string admin1, string country,
                              string countryCode, string timezone, double latitude, double longitude)
        {
            Query = query ?? string.Empty;
            Name = name ?? string.Empty;
            Admin1 = admin1 ?? string.Empty;
            Country = country ?? string.Empty;
            CountryCode = countryCode ?? string.Empty;
            Timezone = timezone ?? string.Empty;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>The raw text the player typed, kept so the query can be re-applied later.</summary>
        public string Query { get; }

        public string Name { get; }

        /// <summary>First level administrative area (region / state / province). May be empty.</summary>
        public string Admin1 { get; }

        public string Country { get; }

        public string CountryCode { get; }

        /// <summary>IANA timezone reported by Open-Meteo. Diagnostic only - the game clock is never touched.</summary>
        public string Timezone { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        /// <summary>Human readable "Lyon, Auvergne-Rhone-Alpes, France".</summary>
        public string DisplayName
        {
            get
            {
                var sb = new StringBuilder(Name);
                if (!string.IsNullOrEmpty(Admin1) && !string.Equals(Admin1, Name, StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(", ").Append(Admin1);
                }

                if (!string.IsNullOrEmpty(Country))
                {
                    sb.Append(", ").Append(Country);
                }

                return sb.ToString();
            }
        }

        public bool HasValidCoordinates
        {
            get
            {
                return !double.IsNaN(Latitude) && !double.IsNaN(Longitude)
                       && Latitude >= -90.0 && Latitude <= 90.0
                       && Longitude >= -180.0 && Longitude <= 180.0;
            }
        }
    }
}
