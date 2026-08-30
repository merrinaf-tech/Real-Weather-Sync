using System;
using System.Globalization;
using RealWeatherSync.Models;
using RealWeatherSync.Settings;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Covers the antipode transform and the built-in extreme-location table.
    /// </summary>
    public static class LocationTests
    {
        public static void Run()
        {
            Assert.Section("Antipode maths");
            Antipode("Lyon -> South Pacific", 45.749, 4.848, -45.749, -175.152);
            Antipode("negative longitude flips the other way", 40.7, -74.0, -40.7, 106.0);
            Antipode("equator stays on the equator", 0.0, 10.0, 0.0, -170.0);
            Antipode("prime meridian -> antimeridian", 51.5, 0.0, -51.5, 180.0);
            Antipode("antimeridian -> prime meridian", 10.0, 180.0, -10.0, 0.0);
            Antipode("-180 normalises to 0", 10.0, -180.0, -10.0, 0.0);
            Antipode("north pole -> south pole", 90.0, 0.0, -90.0, 180.0);

            var origin = new LocationResult("q", "Lyon", "a", "c", "cc", "tz", 45.749, 4.848);
            var there = origin.CreateAntipode();
            var back = there.CreateAntipode();
            Assert.Near("applying it twice returns the latitude", (float)back.Latitude, 45.749f);
            Assert.Near("applying it twice returns the longitude", (float)back.Longitude, 4.848f);
            Assert.True("the antipode has valid coordinates", there.HasValidCoordinates);
            Assert.True("it is renamed clearly: " + there.Name, there.Name.StartsWith("Antipode of"));

            Assert.Section("Extreme-location presets");
            var presets = 0;
            foreach (ExtremeLocationOption option in Enum.GetValues(typeof(ExtremeLocationOption)))
            {
                var location = ExtremeLocations.Get(option);

                if (option == ExtremeLocationOption.None)
                {
                    Assert.True("None maps to no location", location == null);
                    continue;
                }

                presets++;
                Assert.True(option + " -> " + (location == null ? "NULL" : location.DisplayName),
                    location != null && location.HasValidCoordinates
                    && !string.IsNullOrEmpty(location.Name) && !string.IsNullOrEmpty(location.Country));
                Assert.True("  its antipode is valid too",
                    location != null && location.CreateAntipode().HasValidCoordinates);
                Assert.True("  it has a description",
                    !string.IsNullOrEmpty(ExtremeLocations.DescribeEnglish(option)));
            }

            Assert.True("there are " + presets + " presets", presets >= 5);
        }

        private static void Antipode(string label, double lat, double lon, double expectedLat, double expectedLon)
        {
            var a = new LocationResult("q", "X", "", "", "", "", lat, lon).CreateAntipode();
            var ok = Math.Abs(a.Latitude - expectedLat) < 1e-9 && Math.Abs(a.Longitude - expectedLon) < 1e-9;

            Assert.True(string.Format(CultureInfo.InvariantCulture,
                "{0,-42} ({1},{2}) -> ({3},{4})", label, lat, lon, a.Latitude, a.Longitude), ok);
        }
    }
}
