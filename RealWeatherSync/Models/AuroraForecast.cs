using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace RealWeatherSync.Models
{
    /// <summary>One timestamped NOAA OVATION probability grid; independent of game APIs.</summary>
    public sealed class AuroraForecast
    {
        private readonly byte[,] _probability;

        private AuroraForecast(DateTimeOffset observed, DateTimeOffset forecast, byte[,] probability)
        {
            ObservedAt = observed;
            ForecastAt = forecast;
            _probability = probability;
        }

        public DateTimeOffset ObservedAt { get; }
        public DateTimeOffset ForecastAt { get; }

        public bool IsForMoment(DateTimeOffset now)
        {
            return ObservedAt <= now && ObservedAt >= now.AddMinutes(-120)
                && ForecastAt >= now.AddMinutes(-10) && ForecastAt <= now.AddMinutes(10);
        }

        public float ProbabilityAt(double latitude, double longitude)
        {
            if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsInfinity(latitude)
                || double.IsInfinity(longitude) || latitude < -90 || latitude > 90)
            {
                return 0f;
            }
            var lon = ((int)Math.Round(longitude) % 360 + 360) % 360;
            var lat = Math.Max(0, Math.Min(180, (int)Math.Round(latitude) + 90));
            return _probability[lon, lat] / 100f;
        }

        public static AuroraForecast Parse(string json)
        {
            var root = JObject.Parse(json);
            var observed = ParseUtc((string)root["Observation Time"]);
            var forecast = ParseUtc((string)root["Forecast Time"]);
            var coordinates = root["coordinates"] as JArray;
            if (coordinates == null || coordinates.Count < 60000 || forecast < observed)
            {
                throw new FormatException("Invalid NOAA aurora grid or timestamps.");
            }

            var grid = new byte[360, 181];
            var seen = new bool[360, 181];
            var valid = 0;
            foreach (var point in coordinates)
            {
                var triple = point as JArray;
                if (triple == null || triple.Count != 3) continue;
                var lon = (int)triple[0];
                var lat = (int)triple[1];
                var value = (int)triple[2];
                if (lon < 0 || lon >= 360 || lat < -90 || lat > 90 || value < 0 || value > 100) continue;
                grid[lon, lat + 90] = (byte)value;
                if (!seen[lon, lat + 90])
                {
                    seen[lon, lat + 90] = true;
                    valid++;
                }
            }
            if (valid < 60000) throw new FormatException("Incomplete NOAA aurora grid.");
            return new AuroraForecast(observed, forecast, grid);
        }

        private static DateTimeOffset ParseUtc(string value)
        {
            DateTimeOffset result;
            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                throw new FormatException("Invalid NOAA aurora timestamp.");
            }
            return result.ToUniversalTime();
        }
    }
}
