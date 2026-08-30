using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using RealWeatherSync.Mapping;
using RealWeatherSync.Models;
using RealWeatherSync.Services;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Exercises <see cref="OpenMeteoClient"/> against the REAL Open-Meteo API.
    ///
    /// These are integration tests on purpose: the parsing bugs worth catching are the ones that
    /// only appear against the live response shape. They need a working internet connection -
    /// skip them with --offline when there is none.
    /// </summary>
    public static class OpenMeteoLiveTests
    {
        public static async Task RunAsync()
        {
            Assert.Section("Query parsing (offline)");
            Split("Lyon", "Lyon", "");
            Split("  Lyon  ", "Lyon", "");
            Split("Lyon, France", "Lyon", "France");
            Split("Milazzo,Italy", "Milazzo", "Italy");
            Split("New York, United States", "New York", "United States");

            var candidates = new List<OpenMeteoGeocodingResult>
            {
                new OpenMeteoGeocodingResult { Name = "Springfield", Country = "United States", CountryCode = "US", Admin1 = "Missouri", Latitude = 37.2, Longitude = -93.3 },
                new OpenMeteoGeocodingResult { Name = "Springfield", Country = "United States", CountryCode = "US", Admin1 = "Illinois", Latitude = 39.8, Longitude = -89.6 },
                new OpenMeteoGeocodingResult { Name = "Springfield", Country = "New Zealand", CountryCode = "NZ", Admin1 = "Canterbury", Latitude = -43.3, Longitude = 171.9 }
            };

            Assert.True("no qualifier takes the top result",
                OpenMeteoClient.SelectBestResult(candidates, "").Admin1 == "Missouri");
            Assert.True("a region qualifier picks Illinois",
                OpenMeteoClient.SelectBestResult(candidates, "Illinois").Admin1 == "Illinois");
            Assert.True("a country qualifier picks New Zealand",
                OpenMeteoClient.SelectBestResult(candidates, "New Zealand").Country == "New Zealand");
            Assert.True("a country code works too",
                OpenMeteoClient.SelectBestResult(candidates, "NZ").Country == "New Zealand");
            Assert.True("an unmatched qualifier returns null, not the wrong continent",
                OpenMeteoClient.SelectBestResult(candidates, "Mongolia") == null);
            Assert.True("an empty list returns null",
                OpenMeteoClient.SelectBestResult(new List<OpenMeteoGeocodingResult>(), "France") == null);

            using (var client = new OpenMeteoClient("RealWeatherSync/tests"))
            {
                Assert.Section("Live round trips");
                await RoundTrip(client, "Lyon, France", "FR").ConfigureAwait(false);
                await RoundTrip(client, "Milazzo, Italy", "IT").ConfigureAwait(false);
                await RoundTrip(client, "New York, United States", "US").ConfigureAwait(false);

                Assert.Section("Live failure handling");
                Assert.True("a nonsense city resolves to null",
                    await client.ResolveLocationAsync("Zzzqqqxyznotacity", CancellationToken.None).ConfigureAwait(false) == null);
                Assert.True("a blank city resolves to null",
                    await client.ResolveLocationAsync("   ", CancellationToken.None).ConfigureAwait(false) == null);
                Assert.True("Lyon, Mongolia resolves to null rather than the wrong Lyon",
                    await client.ResolveLocationAsync("Lyon, Mongolia", CancellationToken.None).ConfigureAwait(false) == null);

                using (var cts = new CancellationTokenSource())
                {
                    cts.Cancel();
                    try
                    {
                        await client.ResolveLocationAsync("Paris", cts.Token).ConfigureAwait(false);
                        Assert.True("a pre-cancelled request throws", false);
                    }
                    catch (OperationCanceledException)
                    {
                        Assert.True("a pre-cancelled request throws OperationCanceledException", true);
                    }
                }

                Assert.Section("Live multi-result search");
                var many = await client.SearchLocationsAsync("Springfield", 10, CancellationToken.None).ConfigureAwait(false);
                Assert.True("search returns several candidates (" + many.Count + ")", many.Count > 1);

                var distinct = new HashSet<string>();
                foreach (var m in many)
                {
                    distinct.Add(m.DisplayName);
                }

                Assert.True("candidates are distinct places", distinct.Count == many.Count);

                var filtered = await client.SearchLocationsAsync("Springfield, United States", 10, CancellationToken.None).ConfigureAwait(false);
                var allUs = filtered.Count > 0;
                foreach (var f in filtered)
                {
                    if (!string.Equals(f.CountryCode, "US", StringComparison.OrdinalIgnoreCase))
                    {
                        allUs = false;
                    }
                }

                Assert.True("a qualified search keeps only US results (" + filtered.Count + ")", allUs);
                Assert.True("a nonsense search returns an empty list",
                    (await client.SearchLocationsAsync("Zzzqqqxyznotacity", 10, CancellationToken.None).ConfigureAwait(false)).Count == 0);

                Assert.Section("Live time shift");
                var lyon = await client.ResolveLocationAsync("Lyon, France", CancellationToken.None).ConfigureAwait(false);
                var now = await client.GetWeatherAsync(lyon.Latitude, lyon.Longitude, 0, CancellationToken.None).ConfigureAwait(false);
                var past = await client.GetWeatherAsync(lyon.Latitude, lyon.Longitude, -24, CancellationToken.None).ConfigureAwait(false);
                var future = await client.GetWeatherAsync(lyon.Latitude, lyon.Longitude, 24, CancellationToken.None).ConfigureAwait(false);

                Console.WriteLine("         now    " + now.ObservationTimeLocal + "  " + now);
                Console.WriteLine("         -24h   " + past.ObservationTimeLocal + "  " + past);
                Console.WriteLine("         +24h   " + future.ObservationTimeLocal + "  " + future);

                Assert.True("shift 0 is reported as 0", now.TimeShiftHours == 0);
                Assert.True("shift -24 is reported", past.TimeShiftHours == -24);
                Assert.True("shift +24 is reported", future.TimeShiftHours == 24);
                Assert.True("the past timestamp differs from now", past.ObservationTimeLocal != now.ObservationTimeLocal);
                Assert.True("the future timestamp differs from now", future.ObservationTimeLocal != now.ObservationTimeLocal);
                Assert.True("past and future are exactly two days apart",
                    HoursApart(past.ObservationTimeLocal, future.ObservationTimeLocal, 48));

                foreach (var shifted in new[] { past, future })
                {
                    Assert.True("the " + shifted.TimeShiftHours + "h reading is plausible", IsSane(shifted));
                    Assert.True("  it maps into range", InRange(WeatherMapper.Map(shifted, WeatherMappingOptions.Default)));
                }

                var clamped = await client.GetWeatherAsync(lyon.Latitude, lyon.Longitude, 999, CancellationToken.None).ConfigureAwait(false);
                Assert.True("an absurd shift clamps to +" + OpenMeteoClient.MaxTimeShiftHours + "h",
                    clamped.TimeShiftHours == OpenMeteoClient.MaxTimeShiftHours);

                Assert.Section("Live hourly timeline");
                Assert.True("a timeline came back", now.Timeline != null && now.Timeline.IsUsable);
                if (now.Timeline != null)
                {
                    Console.WriteLine("         " + now.Timeline);
                    Assert.True("it spans enough hours to cover a day",
                        now.Timeline.Samples.Count >= 24);
                    var target = now.Timeline.ResolveTargetTime(15f);
                    Assert.True("resolving in-game 15:00 lands within the last 24 real hours",
                        (now.Timeline.LocalNow - target).TotalHours >= 0
                        && (now.Timeline.LocalNow - target).TotalHours < 24);
                }

                Assert.Section("Live antipode");
                var antipode = lyon.CreateAntipode();
                var oceanWeather = await client.GetWeatherAsync(antipode.Latitude, antipode.Longitude, 0, CancellationToken.None).ConfigureAwait(false);
                Assert.True("Open-Meteo answers for an open-ocean antipode: " + oceanWeather, IsSane(oceanWeather));
            }

            Assert.Section("Disposal");
            var disposed = new OpenMeteoClient("x");
            disposed.Dispose();
            disposed.Dispose();
            try
            {
                await disposed.GetWeatherAsync(45.0, 5.0, 0, CancellationToken.None).ConfigureAwait(false);
                Assert.True("a disposed client throws WeatherProviderException", false);
            }
            catch (WeatherProviderException)
            {
                Assert.True("a disposed client throws WeatherProviderException", true);
            }
        }

        private static async Task RoundTrip(OpenMeteoClient client, string query, string expectedCountryCode)
        {
            LocationResult location;
            try
            {
                location = await client.ResolveLocationAsync(query, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Assert.True("resolve \"" + query + "\" (" + e.GetType().Name + ": " + e.Message + ")", false);
                return;
            }

            if (location == null)
            {
                Assert.True("resolve \"" + query + "\"", false);
                return;
            }

            Assert.True("resolve \"" + query + "\" -> " + location.DisplayName + " [" + location.CountryCode + "]",
                string.Equals(location.CountryCode, expectedCountryCode, StringComparison.OrdinalIgnoreCase)
                && location.HasValidCoordinates);

            var snapshot = await client.GetWeatherAsync(location.Latitude, location.Longitude, 0, CancellationToken.None).ConfigureAwait(false);
            Assert.True("  weather -> " + snapshot, IsSane(snapshot));
            Assert.True("  visibility present", snapshot.VisibilityMeters.HasValue);
            Assert.True("  maps into range", InRange(WeatherMapper.Map(snapshot, WeatherMappingOptions.Default)));
        }

        private static bool IsSane(WeatherSnapshot s)
        {
            return s != null
                   && s.TemperatureCelsius > -95f && s.TemperatureCelsius < 65f
                   && s.CloudCoverPercent >= 0f && s.CloudCoverPercent <= 100f
                   && s.PrecipitationMm >= 0f
                   && s.WeatherCode >= 0;
        }

        private static bool InRange(ClimateTarget t)
        {
            return t.TemperatureCelsius >= -50f && t.TemperatureCelsius <= 50f
                   && t.Cloudiness >= 0f && t.Cloudiness <= 1f
                   && t.Precipitation >= 0f && t.Precipitation <= 1f
                   && t.Fog >= 0f && t.Fog <= 1f;
        }

        private static bool HoursApart(string earlier, string later, int hours)
        {
            DateTime a, b;
            if (!DateTime.TryParse(earlier, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out a) ||
                !DateTime.TryParse(later, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out b))
            {
                return false;
            }

            return Math.Abs((b - a).TotalHours - hours) < 0.01;
        }

        private static void Split(string query, string expectedCity, string expectedQualifier)
        {
            string city;
            string qualifier;
            OpenMeteoClient.SplitQuery(query.Trim(), out city, out qualifier);
            Assert.True("split \"" + query + "\" -> \"" + city + "\" / \"" + qualifier + "\"",
                city == expectedCity && qualifier == expectedQualifier);
        }
    }
}
