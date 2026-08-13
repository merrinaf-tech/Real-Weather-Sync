using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Open-Meteo implementation of both provider interfaces.
    ///
    /// Open-Meteo needs no account and no API key. Only the city name (for
    /// geocoding) and the resolved coordinates (for the forecast) leave this
    /// machine; nothing else is collected or transmitted.
    ///
    /// One <see cref="HttpClient"/> is created per mod load and reused for every
    /// request. All calls are asynchronous; nothing here touches a game API, so it
    /// is safe to run on a thread pool thread.
    /// </summary>
    public sealed class OpenMeteoClient : ILocationService, IWeatherService, IDisposable
    {
        private const string GeocodingEndpoint = "https://geocoding-api.open-meteo.com/v1/search";
        private const string ForecastEndpoint = "https://api.open-meteo.com/v1/forecast";

        private const string CurrentVariables =
            "temperature_2m,relative_humidity_2m,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover";

        /// <summary>
        /// Hourly series. Visibility is always needed (Open-Meteo has no "current" visibility);
        /// the rest are only read when a time shift moves the reading off the current hour.
        /// </summary>
        private const string HourlyVariables =
            "visibility,temperature_2m,relative_humidity_2m,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover";

        /// <summary>Largest shift the mod offers in either direction; keeps the request to 3 days.</summary>
        public const int MaxTimeShiftHours = 24;

        private const int DefaultCandidateCount = 10;

        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);

        /// <summary>Responses are small; anything larger than this is not something we should be parsing.</summary>
        private const int MaxResponseBytes = 1024 * 1024;

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;
        private int _disposed;

        public OpenMeteoClient(string userAgent)
        {
            // Unity's Mono runtime does not always negotiate TLS 1.2 by default.
            TryEnableModernTls();

            _httpClient = new HttpClient
            {
                Timeout = RequestTimeout
            };

            try
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", string.IsNullOrEmpty(userAgent)
                    ? "RealWeatherSync"
                    : userAgent);
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            catch (Exception)
            {
                // A header that the runtime refuses is not worth failing the mod over.
            }

            _jsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                // A single malformed member must not take the whole response down.
                Error = (sender, args) => { args.ErrorContext.Handled = true; }
            };
        }

        // ------------------------------------------------------------------
        // Geocoding
        // ------------------------------------------------------------------

        public async Task<LocationResult> ResolveLocationAsync(string query, CancellationToken cancellationToken)
        {
            var candidates = await SearchLocationsAsync(query, DefaultCandidateCount, cancellationToken)
                .ConfigureAwait(false);

            return candidates.Count > 0 ? candidates[0] : null;
        }

        public async Task<IReadOnlyList<LocationResult>> SearchLocationsAsync(string query, int maxResults,
            CancellationToken cancellationToken)
        {
            var empty = new List<LocationResult>();

            if (string.IsNullOrEmpty(query))
            {
                return empty;
            }

            var trimmed = query.Trim();
            if (trimmed.Length == 0)
            {
                return empty;
            }

            string cityPart;
            string qualifier;
            SplitQuery(trimmed, out cityPart, out qualifier);

            if (cityPart.Length == 0)
            {
                return empty;
            }

            var count = maxResults < 1 ? 1 : (maxResults > 100 ? 100 : maxResults);

            var url = new StringBuilder(GeocodingEndpoint)
                .Append("?name=").Append(Uri.EscapeDataString(cityPart))
                .Append("&count=").Append(count.ToString(CultureInfo.InvariantCulture))
                .Append("&language=en&format=json")
                .ToString();

            var body = await GetStringAsync(url, cancellationToken).ConfigureAwait(false);

            OpenMeteoGeocodingResponse response;
            try
            {
                response = JsonConvert.DeserializeObject<OpenMeteoGeocodingResponse>(body, _jsonSettings);
            }
            catch (Exception e)
            {
                throw new WeatherProviderException("Could not parse the geocoding response.", e);
            }

            if (response == null || response.Results == null || response.Results.Count == 0)
            {
                return empty;
            }

            if (response.Error.HasValue && response.Error.Value)
            {
                throw new WeatherProviderException("Geocoding service reported an error: " +
                                                   (response.Reason ?? "unknown reason"));
            }

            var matches = new List<LocationResult>();
            foreach (var candidate in response.Results)
            {
                if (candidate == null || !candidate.Latitude.HasValue || !candidate.Longitude.HasValue)
                {
                    continue;
                }

                // With a "City, Country" style query, drop candidates that do not match the
                // qualifier rather than silently offering a city on another continent.
                if (!string.IsNullOrEmpty(qualifier) && !MatchesQualifier(candidate, qualifier))
                {
                    continue;
                }

                var result = new LocationResult(
                    trimmed,
                    candidate.Name,
                    candidate.Admin1,
                    candidate.Country,
                    candidate.CountryCode,
                    candidate.Timezone,
                    candidate.Latitude.Value,
                    candidate.Longitude.Value);

                if (result.HasValidCoordinates)
                {
                    matches.Add(result);
                }
            }

            return matches;
        }

        // ------------------------------------------------------------------
        // Weather
        // ------------------------------------------------------------------

        public async Task<WeatherSnapshot> GetWeatherAsync(double latitude, double longitude, int timeShiftHours,
            CancellationToken cancellationToken)
        {
            var shift = ClampShift(timeShiftHours);
            var ci = CultureInfo.InvariantCulture;

            var url = new StringBuilder(ForecastEndpoint)
                .Append("?latitude=").Append(latitude.ToString("0.#####", ci))
                .Append("&longitude=").Append(longitude.ToString("0.#####", ci))
                // "current" is always requested: at shift 0 it is the reading we use, and at any
                // shift it gives us the location's local timestamp to index the hourly series
                // from, so no timezone arithmetic happens on our side.
                .Append("&current=").Append(CurrentVariables)
                .Append("&hourly=").Append(HourlyVariables)
                .Append("&past_days=1&forecast_days=2")
                .Append("&temperature_unit=celsius&precipitation_unit=mm&wind_speed_unit=kmh")
                // timezone=auto only aligns the timestamps we read back with the location. The
                // game clock, date and season are never touched.
                .Append("&timezone=auto")
                .ToString();

            var body = await GetStringAsync(url, cancellationToken).ConfigureAwait(false);

            OpenMeteoWeatherResponse response;
            try
            {
                response = JsonConvert.DeserializeObject<OpenMeteoWeatherResponse>(body, _jsonSettings);
            }
            catch (Exception e)
            {
                throw new WeatherProviderException("Could not parse the weather response.", e);
            }

            if (response == null)
            {
                throw new WeatherProviderException("Weather service returned an empty response.");
            }

            if (response.Error.HasValue && response.Error.Value)
            {
                throw new WeatherProviderException("Weather service reported an error: " +
                                                   (response.Reason ?? "unknown reason"));
            }

            var current = response.Current;
            if (current == null)
            {
                throw new WeatherProviderException("Weather response contained no current conditions.");
            }

            var currentHourIndex = FindHourIndex(response.Hourly, current.Time);

            var snapshot = shift == 0
                ? BuildFromCurrent(current, response.Hourly, currentHourIndex)
                : BuildFromHourly(response.Hourly, currentHourIndex, shift, current);

            // Costs nothing extra: the hourly series is already in this response.
            snapshot.Timeline = BuildTimeline(response.Hourly, current.Time);
            return snapshot;
        }

        /// <summary>
        /// Turns the hourly arrays into a <see cref="WeatherTimeline"/>. Returns null when the
        /// series is unusable; callers treat that as "clock-following mode unavailable".
        /// </summary>
        private static WeatherTimeline BuildTimeline(OpenMeteoHourly hourly, string currentTime)
        {
            if (hourly == null || hourly.Time == null || hourly.Time.Count == 0)
            {
                return null;
            }

            DateTime localNow;
            if (!TryParseLocal(currentTime, out localNow))
            {
                return null;
            }

            var samples = new List<HourlySample>(hourly.Time.Count);
            for (var i = 0; i < hourly.Time.Count; i++)
            {
                DateTime stamp;
                if (!TryParseLocal(hourly.Time[i], out stamp))
                {
                    continue;
                }

                var temperature = ReadNullable(hourly.Temperature2m, i);
                if (!temperature.HasValue)
                {
                    // A gap in the series is better skipped than filled with zeroes.
                    continue;
                }

                var code = ReadNullableInt(hourly.WeatherCode, i);
                var isDay = ReadNullableInt(hourly.IsDay, i);

                samples.Add(new HourlySample
                {
                    LocalTime = stamp,
                    Weather = new WeatherSnapshot
                    {
                        ReceivedUtc = DateTime.UtcNow,
                        ObservationTimeLocal = hourly.Time[i],
                        TemperatureCelsius = temperature.Value,
                        CloudCoverPercent = ReadOrZero(hourly.CloudCover, i),
                        PrecipitationMm = ReadOrZero(hourly.Precipitation, i),
                        RainMm = ReadOrZero(hourly.Rain, i),
                        ShowersMm = ReadOrZero(hourly.Showers, i),
                        SnowfallCm = ReadOrZero(hourly.Snowfall, i),
                        RelativeHumidityPercent = ReadOrZero(hourly.RelativeHumidity2m, i),
                        WeatherCode = code.HasValue ? code.Value : 0,
                        IsDay = isDay.HasValue && isDay.Value != 0,
                        VisibilityMeters = ReadNullable(hourly.Visibility, i)
                    }
                });
            }

            return samples.Count >= 2 ? new WeatherTimeline(samples, localNow) : null;
        }

        /// <summary>Parses Open-Meteo's "yyyy-MM-ddTHH:mm" local timestamps.</summary>
        private static bool TryParseLocal(string value, out DateTime result)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.NoCurrentDateDefault, out result) && result != default(DateTime);
        }

        private static int ClampShift(int hours)
        {
            if (hours > MaxTimeShiftHours)
            {
                return MaxTimeShiftHours;
            }

            return hours < -MaxTimeShiftHours ? -MaxTimeShiftHours : hours;
        }

        private static WeatherSnapshot BuildFromCurrent(OpenMeteoCurrent current, OpenMeteoHourly hourly, int hourIndex)
        {
            if (!current.Temperature2m.HasValue)
            {
                throw new WeatherProviderException("Weather response contained no temperature.");
            }

            return new WeatherSnapshot
            {
                ReceivedUtc = DateTime.UtcNow,
                ObservationTimeLocal = current.Time ?? string.Empty,
                TemperatureCelsius = Sanitise(current.Temperature2m, 0f),
                CloudCoverPercent = Sanitise(current.CloudCover, 0f),
                PrecipitationMm = Sanitise(current.Precipitation, 0f),
                RainMm = Sanitise(current.Rain, 0f),
                ShowersMm = Sanitise(current.Showers, 0f),
                SnowfallCm = Sanitise(current.Snowfall, 0f),
                RelativeHumidityPercent = Sanitise(current.RelativeHumidity2m, 0f),
                WeatherCode = current.WeatherCode.HasValue ? current.WeatherCode.Value : 0,
                IsDay = current.IsDay.HasValue && current.IsDay.Value != 0,
                VisibilityMeters = ReadNullable(hourly == null ? null : hourly.Visibility, hourIndex),
                TimeShiftHours = 0
            };
        }

        /// <summary>
        /// Reads the hourly sample <paramref name="shift"/> hours away from the location's current
        /// hour. Falls back to the current conditions when the series cannot be indexed, so a
        /// shifted reading never degrades into zeroes.
        /// </summary>
        private static WeatherSnapshot BuildFromHourly(OpenMeteoHourly hourly, int currentHourIndex, int shift,
            OpenMeteoCurrent current)
        {
            if (hourly == null || hourly.Time == null || currentHourIndex < 0)
            {
                return BuildFromCurrent(current, hourly, currentHourIndex);
            }

            var index = currentHourIndex + shift;
            if (index < 0 || index >= hourly.Time.Count)
            {
                throw new WeatherProviderException(
                    "The requested time shift falls outside the data Open-Meteo returned.");
            }

            var temperature = ReadNullable(hourly.Temperature2m, index);
            if (!temperature.HasValue)
            {
                throw new WeatherProviderException("The shifted hour contained no temperature.");
            }

            var code = ReadNullableInt(hourly.WeatherCode, index);
            var isDay = ReadNullableInt(hourly.IsDay, index);

            return new WeatherSnapshot
            {
                ReceivedUtc = DateTime.UtcNow,
                ObservationTimeLocal = hourly.Time[index] ?? string.Empty,
                TemperatureCelsius = temperature.Value,
                CloudCoverPercent = ReadOrZero(hourly.CloudCover, index),
                PrecipitationMm = ReadOrZero(hourly.Precipitation, index),
                RainMm = ReadOrZero(hourly.Rain, index),
                ShowersMm = ReadOrZero(hourly.Showers, index),
                SnowfallCm = ReadOrZero(hourly.Snowfall, index),
                RelativeHumidityPercent = ReadOrZero(hourly.RelativeHumidity2m, index),
                WeatherCode = code.HasValue ? code.Value : 0,
                IsDay = isDay.HasValue && isDay.Value != 0,
                VisibilityMeters = ReadNullable(hourly.Visibility, index),
                TimeShiftHours = shift
            };
        }

        /// <summary>
        /// Index of the hourly sample matching the location's current local hour.
        /// Both series use "yyyy-MM-ddTHH:mm" in the same zone, so a prefix compare is enough.
        /// </summary>
        private static int FindHourIndex(OpenMeteoHourly hourly, string currentTime)
        {
            if (hourly == null || hourly.Time == null || hourly.Time.Count == 0)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(currentTime) || currentTime.Length < 13)
            {
                return -1;
            }

            var currentHour = currentTime.Substring(0, 13);
            for (var i = 0; i < hourly.Time.Count; i++)
            {
                var t = hourly.Time[i];
                if (t != null && t.Length >= 13 &&
                    string.Equals(t.Substring(0, 13), currentHour, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        private static float? ReadNullable(List<float?> series, int index)
        {
            if (series == null || index < 0 || index >= series.Count)
            {
                return null;
            }

            var value = series[index];
            if (!value.HasValue || float.IsNaN(value.Value) || float.IsInfinity(value.Value) || value.Value < 0f)
            {
                return null;
            }

            return value.Value;
        }

        private static int? ReadNullableInt(List<int?> series, int index)
        {
            if (series == null || index < 0 || index >= series.Count)
            {
                return null;
            }

            return series[index];
        }

        private static float ReadOrZero(List<float?> series, int index)
        {
            var value = ReadNullable(series, index);
            return value.HasValue ? value.Value : 0f;
        }

        // ------------------------------------------------------------------
        // Query parsing
        // ------------------------------------------------------------------

        /// <summary>
        /// Splits "Milazzo, Italy" into the name Open-Meteo can search for and the
        /// country / region qualifier used to disambiguate the candidates.
        /// </summary>
        internal static void SplitQuery(string query, out string cityPart, out string qualifier)
        {
            var comma = query.IndexOf(',');
            if (comma < 0)
            {
                cityPart = query.Trim();
                qualifier = string.Empty;
                return;
            }

            cityPart = query.Substring(0, comma).Trim();
            qualifier = query.Substring(comma + 1).Trim();
        }

        internal static bool MatchesQualifier(OpenMeteoGeocodingResult candidate, string qualifier)
        {
            return Matches(candidate.Country, qualifier)
                   || Matches(candidate.CountryCode, qualifier)
                   || Matches(candidate.Admin1, qualifier)
                   || Matches(candidate.Admin2, qualifier);
        }

        /// <summary>
        /// Open-Meteo returns candidates already ranked by relevance. With no
        /// qualifier we take the top one; with a qualifier we take the best ranked
        /// candidate whose country, country code or region matches.
        /// </summary>
        internal static OpenMeteoGeocodingResult SelectBestResult(List<OpenMeteoGeocodingResult> results, string qualifier)
        {
            if (results == null || results.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(qualifier))
            {
                return results[0];
            }

            foreach (var candidate in results)
            {
                if (candidate != null && MatchesQualifier(candidate, qualifier))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool Matches(string value, string qualifier)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (string.Equals(value, qualifier, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return value.IndexOf(qualifier, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // ------------------------------------------------------------------
        // Transport
        // ------------------------------------------------------------------

        private async Task<string> GetStringAsync(string url, CancellationToken cancellationToken)
        {
            if (_disposed != 0)
            {
                throw new WeatherProviderException("The weather client has already been disposed.");
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient
                    .GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Either our own cancellation or the HttpClient timeout. Let the
                // caller decide which; it knows whether it asked for cancellation.
                throw;
            }
            catch (HttpRequestException e)
            {
                throw new WeatherProviderException(DescribeTransportFailure(e), e);
            }
            catch (Exception e)
            {
                throw new WeatherProviderException("Weather request failed: " + e.Message, e);
            }

            using (response)
            {
                if (response.StatusCode == (HttpStatusCode)429)
                {
                    throw new WeatherProviderException("Open-Meteo is rate limiting this client (HTTP 429).", true);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new WeatherProviderException("Open-Meteo returned HTTP " +
                                                       ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) +
                                                       " " + response.ReasonPhrase);
                }

                var length = response.Content.Headers.ContentLength;
                if (length.HasValue && length.Value > MaxResponseBytes)
                {
                    throw new WeatherProviderException("Open-Meteo response was unexpectedly large; ignoring it.");
                }

                string body;
                try
                {
                    body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new WeatherProviderException("Could not read the Open-Meteo response body.", e);
                }

                if (string.IsNullOrEmpty(body))
                {
                    throw new WeatherProviderException("Open-Meteo returned an empty body.");
                }

                if (body.Length > MaxResponseBytes)
                {
                    throw new WeatherProviderException("Open-Meteo response was unexpectedly large; ignoring it.");
                }

                return body;
            }
        }

        private static string DescribeTransportFailure(HttpRequestException e)
        {
            var inner = e.InnerException;
            var detail = inner != null ? inner.Message : e.Message;
            return "Could not reach Open-Meteo (no connection, DNS failure or TLS problem): " + detail;
        }

        private static float Sanitise(float? value, float fallback)
        {
            if (!value.HasValue)
            {
                return fallback;
            }

            var v = value.Value;
            return float.IsNaN(v) || float.IsInfinity(v) ? fallback : v;
        }

        private static void TryEnableModernTls()
        {
            try
            {
                // 3072 == Tls12, spelled numerically so this still compiles and runs
                // against runtimes with a narrower SecurityProtocolType enum.
                const SecurityProtocolType tls12 = (SecurityProtocolType)3072;
                if ((ServicePointManager.SecurityProtocol & tls12) == 0)
                {
                    ServicePointManager.SecurityProtocol |= tls12;
                }
            }
            catch (Exception)
            {
                // Nothing useful to do; the request will simply fail later if TLS is unusable.
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            try
            {
                _httpClient.Dispose();
            }
            catch (Exception)
            {
                // Disposal must never throw out of mod teardown.
            }
        }
    }
}
