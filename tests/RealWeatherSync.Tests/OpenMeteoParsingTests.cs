using System;
using System.Collections.Generic;
using RealWeatherSync.Models;
using RealWeatherSync.Services;

namespace RealWeatherSync.Tests
{
    public static class OpenMeteoParsingTests
    {
        public static void Run()
        {
            Assert.Section("Open-Meteo hourly temperature parsing");

            var response = new OpenMeteoWeatherResponse
            {
                Current = new OpenMeteoCurrent
                {
                    Time = "2026-01-10T10:15",
                    Temperature2m = -6f
                },
                Hourly = new OpenMeteoHourly
                {
                    Time = new List<string>
                    {
                        "2026-01-10T09:00", "2026-01-10T10:00", "2026-01-10T11:00"
                    },
                    Temperature2m = new List<float?> { -12f, -6f, 2f },
                    Visibility = new List<float?> { 1000f, 2000f, 3000f }
                }
            };

            var past = OpenMeteoClient.BuildSnapshot(response, -1);
            Assert.Near("a negative shifted temperature is retained", past.TemperatureCelsius, -12f);
            Assert.Equal("the shifted observation is the requested hour",
                past.ObservationTimeLocal, "2026-01-10T09:00");

            var current = OpenMeteoClient.BuildSnapshot(response, 0);
            Assert.True("cold hours remain in the game-clock timeline",
                current.Timeline != null && current.Timeline.Samples.Count == 3);
            Assert.Near("the first timeline hour remains below freezing",
                current.Timeline.Samples[0].Weather.TemperatureCelsius, -12f);
            Assert.Near("the second timeline hour remains below freezing",
                current.Timeline.Samples[1].Weather.TemperatureCelsius, -6f);
        }
    }
}
