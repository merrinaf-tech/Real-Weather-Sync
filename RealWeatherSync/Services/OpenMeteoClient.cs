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

        /// <summary>How many geocoding candidates to ask for before filtering by country / region.</summary>
        private const int GeocodingCandidateCount = 10;

        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);

        /// <summary>Responses are small; anything larger than this is not something we should be parsing.</summary>
        private const int MaxResponseBytes = 512 * 1024;

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

        public async Task<LocationResult> ResolveLocationAsync(string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            var trimmed = query.Trim();
            if (trimmed.Length == 0)
            {
                return null;
            }

            string cityPart;
            string qualifier;
            SplitQuery(trimmed, out cityPart, out qualifier);

            if (cityPart.Length == 0)
            {
                return null;
            }

            var url = new StringBuilder(GeocodingEndpoint)
                .Append("?name=").Append(Uri.EscapeDataString(cityPart))
                .Append("&count=").Append(GeocodingCandidateCount.ToString(CultureInfo.InvariantCulture))
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
                return null;
            }

            if (response.Error.HasValue && response.Error.Value)
            {
                throw new WeatherProviderException("Geocoding service reported an error: " +
                                                   (response.Reason ?? "unknown reason"));
            }

            var chosen = SelectBestResult(response.Results, qualifier);
            if (chosen == null)
            {
                return null;
            }

            if (!chosen.Latitude.HasValue || !chosen.Longitude.HasValue)
            {
                return null;
            }

            var result = new LocationResult(
                trimmed,
                chosen.Name,
                chosen.Admin1,
                chosen.Country,
                chosen.CountryCode,
                chosen.Timezone,
                chosen.Latitude.Value,
                chosen.Longitude.Value);

            return result.HasValidCoordinates ? result : null;
        }

        public async Task<WeatherSnapshot> GetCurrentWeatherAsync(double latitude, double longitude,
            CancellationToken cancellationToken)
        {
            var ci = CultureInfo.InvariantCulture;

            var url = new StringBuilder(ForecastEndpoint)
                .Append("?latitude=").Append(latitude.ToString("0.#####", ci))
                .Append("&longitude=").Append(longitude.ToString("0.#####", ci))
                .Append("&current=").Append(CurrentVariables)
                // Visibility is an hourly-only variable in Open-Meteo. Asking for one
                // hour either side of now gives us a value to line up with current.time.
                .Append("&hourly=visibility&past_hours=1&forecast_hours=1")
                .Append("&temperature_unit=celsius&precipitation_unit=mm&wind_speed_unit=kmh")
                // timezone=auto only aligns the timestamps we read back with the
                // location. The game clock, date and season are never touched.
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

            if (!current.Temperature2m.HasValue)
            {
                throw new WeatherProviderException("Weather response contained no temperature.");
            }

            var snapshot = new WeatherSnapshot
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
                VisibilityMeters = ExtractVisibility(response.Hourly, current.Time)
            };

            return snapshot;
        }

        /// <summary>
        /// Picks the hourly visibility sample whose timestamp matches the current
        /// observation. Returns null rather than guessing when nothing lines up.
        /// </summary>
        private static float? ExtractVisibility(OpenMeteoHourly hourly, string currentTime)
        {
            if (hourly == null || hourly.Time == null || hourly.Visibility == null)
            {
                return null;
            }

            var count = Math.Min(hourly.Time.Count, hourly.Visibility.Count);
            if (count == 0)
            {
                return null;
            }

            var index = -1;

            if (!string.IsNullOrEmpty(currentTime) && currentTime.Length >= 13)
            {
                // Both series use "yyyy-MM-ddTHH:mm" in the same (location local) zone.
                var currentHour = currentTime.Substring(0, 13);
                for (var i = 0; i < count; i++)
                {
                    var t = hourly.Time[i];
                    if (t != null && t.Length >= 13 && string.Equals(t.Substring(0, 13), currentHour, StringComparison.Ordinal))
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index < 0)
            {
                // Fall back to the most recent sample that actually has a value.
                for (var i = count - 1; i >= 0; i--)
                {
                    if (hourly.Visibility[i].HasValue)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index < 0)
            {
                return null;
            }

            var value = hourly.Visibility[index];
            if (!value.HasValue || float.IsNaN(value.Value) || float.IsInfinity(value.Value) || value.Value < 0f)
            {
                return null;
            }

            return value.Value;
        }

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

        /// <summary>
        /// Open-Meteo returns candidates already ranked by relevance. With no
        /// qualifier we take the top one; with a qualifier we take the best ranked
        /// candidate whose country, country code or region matches, and report
        /// "not found" rather than silently returning a city on another continent.
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
                if (candidate == null)
                {
                    continue;
                }

                if (Matches(candidate.Country, qualifier)
                    || Matches(candidate.CountryCode, qualifier)
                    || Matches(candidate.Admin1, qualifier)
                    || Matches(candidate.Admin2, qualifier))
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
