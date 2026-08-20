using System;
using System.Globalization;
using RealWeatherSync.Localization;
using RealWeatherSync.Mapping;
using RealWeatherSync.Models;

namespace RealWeatherSync.Diagnostics
{
    /// <summary>
    /// Single place holding what the options page shows about the mod's state.
    ///
    /// Written from the main thread and from network continuations, read from the
    /// UI thread, so every access goes through the same lock. Nothing here touches
    /// a game system.
    /// </summary>
    public static class StatusReport
    {
        private static readonly object Gate = new object();

        private static StatusKind _kind = StatusKind.Disabled;
        private static string _detail = string.Empty;
        private static string _location = string.Empty;
        private static DateTime? _lastSuccessUtc;
        private static WeatherSnapshot _lastSnapshot;
        private static ClimateTarget _lastTarget;
        private static bool _hasTarget;
        private static bool _overridesActive;
        private static bool _oppositeDay;
        private static bool _antipode;

        /// <summary>Lets the status block label a result that was deliberately inverted.</summary>
        public static void SetOppositeDay(bool active)
        {
            lock (Gate)
            {
                _oppositeDay = active;
            }
        }

        /// <summary>Lets the status block say the reading came from the other side of the planet.</summary>
        public static void SetAntipode(bool active)
        {
            lock (Gate)
            {
                _antipode = active;
            }
        }

        public static StatusKind Kind
        {
            get { lock (Gate) { return _kind; } }
        }

        public static void Set(StatusKind kind)
        {
            Set(kind, string.Empty);
        }

        public static void Set(StatusKind kind, string detail)
        {
            lock (Gate)
            {
                _kind = kind;
                _detail = detail ?? string.Empty;
            }
        }

        public static void SetLocation(string displayName, double latitude, double longitude)
        {
            var ci = CultureInfo.InvariantCulture;
            var text = string.IsNullOrEmpty(displayName)
                ? string.Empty
                : displayName + "  (" + latitude.ToString("0.####", ci) + ", " + longitude.ToString("0.####", ci) + ")";

            lock (Gate)
            {
                _location = text;
            }
        }

        public static void ClearLocation()
        {
            lock (Gate)
            {
                _location = string.Empty;
            }
        }

        public static void RecordSuccess(WeatherSnapshot snapshot)
        {
            lock (Gate)
            {
                _lastSnapshot = snapshot;
                _lastSuccessUtc = DateTime.UtcNow;
            }
        }

        public static void RecordTarget(ClimateTarget target)
        {
            lock (Gate)
            {
                _lastTarget = target;
                _hasTarget = true;
            }
        }

        public static void SetOverridesActive(bool active)
        {
            lock (Gate)
            {
                _overridesActive = active;
            }
        }

        public static bool OverridesActive
        {
            get { lock (Gate) { return _overridesActive; } }
        }

        public static void Reset()
        {
            lock (Gate)
            {
                _kind = StatusKind.Disabled;
                _detail = string.Empty;
                _location = string.Empty;
                _lastSuccessUtc = null;
                _lastSnapshot = null;
                _hasTarget = false;
                _overridesActive = false;
                _oppositeDay = false;
                _antipode = false;
            }
        }

        /// <summary>One line summary for the "Status" row of the options page.</summary>
        public static string DescribeStatus()
        {
            StatusKind kind;
            string detail;
            lock (Gate)
            {
                kind = _kind;
                detail = _detail;
            }

            var text = Describe(kind);
            if (!string.IsNullOrEmpty(detail))
            {
                text = text + " - " + detail;
            }

            return text;
        }

        private static string Describe(StatusKind kind)
        {
            switch (kind)
            {
                case StatusKind.Disabled:
                    return Translation.Get(LocaleKeys.StatusDisabled, "Disabled");
                case StatusKind.CityNotConfigured:
                    return Translation.Get(LocaleKeys.StatusCityNotConfigured, "City not configured");
                case StatusKind.ResolvingLocation:
                    return Translation.Get(LocaleKeys.StatusResolvingLocation, "Resolving location");
                case StatusKind.Refreshing:
                    return Translation.Get(LocaleKeys.StatusRefreshing, "Refreshing weather");
                case StatusKind.Connected:
                    return Translation.Get(LocaleKeys.StatusConnected, "Connected");
                case StatusKind.CandidatesReady:
                    return Translation.Get(LocaleKeys.StatusCandidatesReady, "Pick a city from the search results");
                case StatusKind.Offline:
                    return Translation.Get(LocaleKeys.StatusOffline, "Offline - using last valid weather");
                case StatusKind.ErrorResolvingCity:
                    return Translation.Get(LocaleKeys.StatusErrorResolvingCity, "Error resolving city");
                case StatusKind.IncompatibleModActive:
                    return Translation.Get(LocaleKeys.StatusIncompatibleMod, "Incompatible weather mod active");
                case StatusKind.ReleasedByPlayer:
                    return Translation.Get(LocaleKeys.StatusReleased, "Overrides released - using game weather");
                case StatusKind.WaitingForGame:
                    return Translation.Get(LocaleKeys.StatusWaitingForGame, "Waiting for a city to be loaded");
                default:
                    return string.Empty;
            }
        }

        public static string DescribeLocation()
        {
            lock (Gate)
            {
                if (!string.IsNullOrEmpty(_location))
                {
                    return _location;
                }
            }

            return Translation.Get(LocaleKeys.LocationNotResolved, "No location resolved yet");
        }

        public static string DescribeLastUpdate()
        {
            DateTime? last;
            lock (Gate)
            {
                last = _lastSuccessUtc;
            }

            if (!last.HasValue)
            {
                return Translation.Get(LocaleKeys.LastUpdateNever, "Never");
            }

            var local = last.Value.ToLocalTime();
            var age = DateTime.UtcNow - last.Value;
            var minutes = (int)age.TotalMinutes;

            var stamp = local.ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture);
            if (minutes <= 0)
            {
                return stamp + " (" + Translation.Get(LocaleKeys.LastUpdateJustNow, "just now") + ")";
            }

            var pattern = Translation.Get(LocaleKeys.LastUpdateMinutesAgo, "{0} min ago");
            string relative;
            try
            {
                relative = string.Format(CultureInfo.CurrentCulture, pattern, minutes);
            }
            catch (FormatException)
            {
                relative = minutes.ToString(CultureInfo.CurrentCulture) + " min";
            }

            return stamp + " (" + relative + ")";
        }

        /// <summary>Multi-line diagnostic block: what was measured and what was applied.</summary>
        public static string DescribeWeather()
        {
            WeatherSnapshot snapshot;
            ClimateTarget target;
            bool hasTarget;
            bool overridesActive;
            bool oppositeDay;
            bool antipode;

            lock (Gate)
            {
                snapshot = _lastSnapshot;
                target = _lastTarget;
                hasTarget = _hasTarget;
                overridesActive = _overridesActive;
                oppositeDay = _oppositeDay;
                antipode = _antipode;
            }

            if (snapshot == null)
            {
                return Translation.Get(LocaleKeys.WeatherNoData, "No weather data received yet.");
            }

            var ci = CultureInfo.CurrentCulture;

            // Human-readable condition first: it is the line most players actually read.
            var text = Translation.Get(
                           WeatherCodes.LocaleIdFor(snapshot.WeatherCode),
                           WeatherCodes.EnglishFor(snapshot.WeatherCode));

            if (snapshot.TimeShiftHours != 0)
            {
                var hours = Math.Abs(snapshot.TimeShiftHours).ToString(ci);
                var pattern = snapshot.TimeShiftHours < 0
                    ? Translation.Get(LocaleKeys.WeatherTimeShiftPast, "{0} h in the past")
                    : Translation.Get(LocaleKeys.WeatherTimeShiftFuture, "{0} h ahead (forecast)");

                string shiftText;
                try
                {
                    shiftText = string.Format(ci, pattern, hours);
                }
                catch (FormatException)
                {
                    shiftText = hours + " h";
                }

                text += "  [" + shiftText + "]";
            }

            text += Environment.NewLine +
                    Translation.Get(LocaleKeys.WeatherObserved, "Observed") + ": " +
                    TemperatureDisplay.Format(snapshot.TemperatureCelsius) + ", " +
                    Translation.Get(LocaleKeys.WeatherClouds, "clouds") + " " +
                    snapshot.CloudCoverPercent.ToString("0", ci) + "%, " +
                    Translation.Get(LocaleKeys.WeatherPrecipitation, "precipitation") + " " +
                    snapshot.PrecipitationMm.ToString("0.00", ci) + " mm/h, " +
                    Translation.Get(LocaleKeys.WeatherSnow, "snow") + " " +
                    snapshot.SnowfallCm.ToString("0.00", ci) + " cm/h";

            if (snapshot.VisibilityMeters.HasValue)
            {
                text += ", " + Translation.Get(LocaleKeys.WeatherVisibility, "visibility") + " " +
                        snapshot.VisibilityMeters.Value.ToString("0", ci) + " m";
            }

            if (hasTarget)
            {
                text += Environment.NewLine +
                        Translation.Get(LocaleKeys.WeatherApplied, "Applied") + ": " +
                        TemperatureDisplay.Format(target.TemperatureCelsius) + ", " +
                        Translation.Get(LocaleKeys.WeatherClouds, "clouds") + " " +
                        (target.Cloudiness * 100f).ToString("0", ci) + "%, " +
                        Translation.Get(LocaleKeys.WeatherPrecipitation, "precipitation") + " " +
                        (target.Precipitation * 100f).ToString("0", ci) + "%, " +
                        Translation.Get(LocaleKeys.WeatherFog, "fog") + " " +
                        (target.Fog * 100f).ToString("0", ci) + "%";

                if (oppositeDay)
                {
                    text += "  [" + Translation.Get(LocaleKeys.WeatherOppositeDay, "Opposite day") + "]";
                }

                if (antipode)
                {
                    text += "  [" + Translation.Get(LocaleKeys.WeatherAntipode, "Antipode") + "]";
                }
            }

            text += Environment.NewLine + (overridesActive
                ? Translation.Get(LocaleKeys.OverridesActive, "Climate overrides are active.")
                : Translation.Get(LocaleKeys.OverridesInactive, "Climate overrides are not active."));

            return text;
        }
    }
}
