using System;
using System.Text;
using RealWeatherSync.Models;

namespace RealWeatherSync.Tests
{
    public static class AuroraForecastTests
    {
        public static void Run()
        {
            Assert.Section("NOAA OVATION forecast contract");
            var now = new DateTimeOffset(2026, 9, 17, 20, 0, 0, TimeSpan.Zero);
            var json = new StringBuilder(1100000);
            json.Append("{\"Observation Time\":\"2026-09-17T19:55:00Z\",\"Forecast Time\":\"2026-09-17T20:45:00Z\",\"coordinates\":[");
            for (var lon = 0; lon < 360; lon++)
            {
                for (var lat = -90; lat <= 90; lat++)
                {
                    if (lon != 0 || lat != -90) json.Append(',');
                    json.Append('[').Append(lon).Append(',').Append(lat).Append(',')
                        .Append(lon == 350 && lat == 65 ? 72 : 0).Append(']');
                }
            }
            json.Append("]}");

            var forecast = AuroraForecast.Parse(json.ToString());
            Assert.True("future forecast is not shown early", !forecast.IsForMoment(now));
            Assert.True("ISO UTC timestamps and full NOAA grid parse", forecast.IsForMoment(now.AddMinutes(45)));
            Assert.Near("negative longitude wraps to NOAA 0..359", forecast.ProbabilityAt(65, -10), 0.72f);
            Assert.Near("same location at 350 degrees", forecast.ProbabilityAt(65, 350), 0.72f);
            Assert.Near("other location has no aurora", forecast.ProbabilityAt(65, 10), 0f);
            Assert.True("old forecast is rejected", !forecast.IsForMoment(now.AddHours(2)));
            Assert.True("future observation is rejected", !forecast.IsForMoment(now.AddHours(-1)));

            Assert.Section("Real-location darkness");
            Assert.True("equator at equinox noon is daylight",
                !SolarNight.IsDark(0, 0, new DateTimeOffset(2026, 3, 20, 12, 0, 0, TimeSpan.Zero)));
            Assert.True("equator at equinox midnight is dark",
                SolarNight.IsDark(0, 0, new DateTimeOffset(2026, 3, 20, 0, 0, 0, TimeSpan.Zero)));
            Assert.True("high latitude summer midnight is not dark",
                !SolarNight.IsDark(69.65, 18.96, new DateTimeOffset(2026, 6, 21, 0, 0, 0, TimeSpan.Zero)));
            Assert.True("high latitude winter midnight is dark",
                SolarNight.IsDark(69.65, 18.96, new DateTimeOffset(2026, 12, 21, 0, 0, 0, TimeSpan.Zero)));
        }
    }
}
