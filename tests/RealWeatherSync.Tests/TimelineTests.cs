using System;
using System.Collections.Generic;
using RealWeatherSync.Models;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Covers <see cref="WeatherTimeline"/>: mapping an in-game hour onto the matching hour from
    /// the last 24 real hours, and blending between the two samples that bracket it.
    ///
    /// This is the logic behind "Follow the in-game clock", and the place a subtle off-by-one is
    /// easiest to introduce - the exact-hour bracket bug was caught here.
    /// </summary>
    public static class TimelineTests
    {
        public static void Run()
        {
            // Local "now" at the city is 2026-08-07 10:00, with 72 hourly samples spanning
            // 2026-08-06 00:00 .. 2026-08-08 23:00. Temperature encodes day*100 + hour so an
            // assertion can name exactly which sample was picked.
            var localNow = new DateTime(2026, 8, 7, 10, 0, 0);
            var samples = new List<HourlySample>();
            var start = new DateTime(2026, 8, 6, 0, 0, 0);
            for (var i = 0; i < 72; i++)
            {
                var stamp = start.AddHours(i);
                samples.Add(new HourlySample
                {
                    LocalTime = stamp,
                    Weather = MapperTests.Snap(t: stamp.Day * 100 + stamp.Hour)
                });
            }

            var timeline = new WeatherTimeline(samples, localNow);

            Assert.Section("In-game hour to real hour");
            Assert.True("timeline is usable", timeline.IsUsable);

            // The canonical example: real 10:00, game 15:00 -> yesterday 15:00.
            Assert.Equal("game 15h -> yesterday 15:00",
                timeline.ResolveTargetTime(15f), new DateTime(2026, 8, 6, 15, 0, 0));
            // An hour that already happened today stays today.
            Assert.Equal("game 8h -> today 08:00",
                timeline.ResolveTargetTime(8f), new DateTime(2026, 8, 7, 8, 0, 0));
            Assert.Equal("game 10h -> today 10:00 (the current hour)",
                timeline.ResolveTargetTime(10f), new DateTime(2026, 8, 7, 10, 0, 0));
            Assert.Equal("game 11h -> yesterday 11:00 (not reached today)",
                timeline.ResolveTargetTime(11f), new DateTime(2026, 8, 6, 11, 0, 0));
            Assert.Equal("game 0h -> today 00:00",
                timeline.ResolveTargetTime(0f), new DateTime(2026, 8, 7, 0, 0, 0));
            Assert.Equal("game 23h -> yesterday 23:00",
                timeline.ResolveTargetTime(23f), new DateTime(2026, 8, 6, 23, 0, 0));
            Assert.Equal("fractional hours keep their minutes",
                timeline.ResolveTargetTime(15.5f), new DateTime(2026, 8, 6, 15, 30, 0));
            Assert.Equal("25h wraps to 1h",
                timeline.ResolveTargetTime(25f), new DateTime(2026, 8, 7, 1, 0, 0));
            Assert.Equal("-1h wraps to 23h",
                timeline.ResolveTargetTime(-1f), new DateTime(2026, 8, 6, 23, 0, 0));

            var outOfWindow = 0;
            for (var h = 0; h < 24; h++)
            {
                var age = (localNow - timeline.ResolveTargetTime(h)).TotalHours;
                if (age < 0 || age >= 24)
                {
                    outOfWindow++;
                }
            }

            Assert.True("all 24 in-game hours land inside the last 24 real hours", outOfWindow == 0);

            Assert.Section("Bracketing and blending");
            WeatherSnapshot before;
            WeatherSnapshot after;
            float blend;

            Assert.True("half past an hour brackets",
                timeline.TryGetBracket(new DateTime(2026, 8, 6, 15, 30, 0), out before, out after, out blend));
            Assert.Near("  lower sample is 15:00", before.TemperatureCelsius, 615f);
            Assert.Near("  upper sample is 16:00", after.TemperatureCelsius, 616f);
            Assert.Near("  blend is 0.5", blend, 0.5f);

            // Regression: an exact hour must be the LOWER bound with blend 0, not the upper bound
            // with blend 1. Same applied value either way, but it keeps the logged hour honest
            // and stops the bracket cache flipping twice per hour.
            Assert.True("exact hour brackets",
                timeline.TryGetBracket(new DateTime(2026, 8, 6, 15, 0, 0), out before, out after, out blend));
            Assert.Near("  blend is 0 on the hour", blend, 0f);
            Assert.Near("  lower sample is the exact hour", before.TemperatureCelsius, 615f);

            Assert.True("before the series clamps to the first sample",
                timeline.TryGetBracket(new DateTime(2020, 1, 1), out before, out after, out blend)
                && before.TemperatureCelsius == 600f);
            Assert.True("after the series clamps to the last sample",
                timeline.TryGetBracket(new DateTime(2030, 1, 1), out before, out after, out blend)
                && before.TemperatureCelsius == 823f);

            var empty = new WeatherTimeline(new List<HourlySample>(), localNow);
            Assert.True("an empty timeline reports itself unusable", !empty.IsUsable);
            Assert.True("an empty timeline brackets nothing",
                !empty.TryGetBracket(localNow, out before, out after, out blend));
        }
    }
}
