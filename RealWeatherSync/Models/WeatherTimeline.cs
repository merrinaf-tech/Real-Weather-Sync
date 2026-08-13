using System;
using System.Collections.Generic;
using System.Globalization;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// One hour of real weather, with the location-local timestamp it belongs to.
    /// </summary>
    public sealed class HourlySample
    {
        public DateTime LocalTime { get; set; }
        public WeatherSnapshot Weather { get; set; }
    }

    /// <summary>
    /// The hourly series around "now" at the resolved location, kept so the mod can follow the
    /// in-game clock through a real day instead of showing one frozen reading.
    ///
    /// The samples come free with the request we already make (past_days=1 &amp; forecast_days=2),
    /// so keeping them costs no extra traffic.
    /// </summary>
    public sealed class WeatherTimeline
    {
        private readonly List<HourlySample> _samples;

        public WeatherTimeline(List<HourlySample> samples, DateTime localNow)
        {
            _samples = samples ?? new List<HourlySample>();
            LocalNow = localNow;
        }

        /// <summary>Current local time at the location, as reported by the provider.</summary>
        public DateTime LocalNow { get; }

        public IReadOnlyList<HourlySample> Samples
        {
            get { return _samples; }
        }

        public bool IsUsable
        {
            get { return _samples.Count >= 2; }
        }

        /// <summary>
        /// The sample for <paramref name="hourOfDay"/> taken from the most recent 24 hours: the
        /// latest moment at or before "now" whose hour matches.
        ///
        /// With real time 10:00 and an in-game hour of 15, that is yesterday at 15:00. With an
        /// in-game hour of 8, it is today at 08:00 - two hours ago, still inside the window.
        /// </summary>
        public DateTime ResolveTargetTime(float hourOfDay)
        {
            var hour = NormaliseHour(hourOfDay);
            var wholeHour = (int)Math.Floor(hour);

            // Start from today at that hour, then step back a day if it has not happened yet.
            var candidate = LocalNow.Date.AddHours(wholeHour);
            if (candidate > LocalNow)
            {
                candidate = candidate.AddDays(-1);
            }

            return candidate.AddHours(hour - wholeHour);
        }

        /// <summary>
        /// Blends the two hourly samples bracketing <paramref name="localTime"/>.
        /// Returns false when the series cannot cover that moment.
        /// </summary>
        public bool TryGetBracket(DateTime localTime, out WeatherSnapshot before, out WeatherSnapshot after,
            out float blend)
        {
            before = null;
            after = null;
            blend = 0f;

            if (_samples.Count == 0)
            {
                return false;
            }

            if (localTime <= _samples[0].LocalTime)
            {
                before = _samples[0].Weather;
                after = _samples[0].Weather;
                return before != null;
            }

            var last = _samples.Count - 1;
            if (localTime >= _samples[last].LocalTime)
            {
                before = _samples[last].Weather;
                after = _samples[last].Weather;
                return before != null;
            }

            for (var i = 1; i <= last; i++)
            {
                // "<=" so that a timestamp landing exactly on an hour becomes the LOWER bound
                // with blend 0, rather than the upper bound with blend 1. Same applied value
                // either way, but it keeps the logged hour honest and avoids the cache flipping
                // brackets twice as the clock crosses each hour.
                if (_samples[i].LocalTime <= localTime)
                {
                    continue;
                }

                var lower = _samples[i - 1];
                var upper = _samples[i];

                var span = (upper.LocalTime - lower.LocalTime).TotalHours;
                blend = span <= 0 ? 0f : (float)((localTime - lower.LocalTime).TotalHours / span);

                if (blend < 0f)
                {
                    blend = 0f;
                }
                else if (blend > 1f)
                {
                    blend = 1f;
                }

                before = lower.Weather;
                after = upper.Weather;
                return before != null && after != null;
            }

            return false;
        }

        /// <summary>Wraps any value into 0 &lt;= h &lt; 24.</summary>
        public static float NormaliseHour(float hourOfDay)
        {
            if (float.IsNaN(hourOfDay) || float.IsInfinity(hourOfDay))
            {
                return 0f;
            }

            var hour = hourOfDay % 24f;
            if (hour < 0f)
            {
                hour += 24f;
            }

            return hour;
        }

        public override string ToString()
        {
            var ci = CultureInfo.InvariantCulture;
            if (_samples.Count == 0)
            {
                return "empty timeline";
            }

            return _samples.Count.ToString(ci) + " hourly samples, " +
                   _samples[0].LocalTime.ToString("yyyy-MM-dd HH:mm", ci) + " .. " +
                   _samples[_samples.Count - 1].LocalTime.ToString("yyyy-MM-dd HH:mm", ci) +
                   " (local now " + LocalNow.ToString("yyyy-MM-dd HH:mm", ci) + ")";
        }
    }
}
