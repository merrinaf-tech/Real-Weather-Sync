using RealWeatherSync.Mapping;
using RealWeatherSync.Models;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Covers <see cref="WeatherMapper"/>: the real-world-to-game conversion, its clamping, and
    /// the documented curve breakpoints. Pure arithmetic, no network, no game.
    /// </summary>
    public static class MapperTests
    {
        public static void Run()
        {
            var options = WeatherMappingOptions.Default;

            Assert.Section("Cloudiness");
            Assert.Near("0% clear", WeatherMapper.MapCloudiness(0f), 0f);
            Assert.Near("48%", WeatherMapper.MapCloudiness(48f), 0.48f);
            Assert.Near("100% overcast", WeatherMapper.MapCloudiness(100f), 1f);
            Assert.Near("130% clamps to 1", WeatherMapper.MapCloudiness(130f), 1f);
            Assert.Near("-5% clamps to 0", WeatherMapper.MapCloudiness(-5f), 0f);

            Assert.Section("Temperature");
            Assert.Near("32 C passes through", WeatherMapper.MapTemperature(Snap(t: 32f), options), 32f);
            Assert.Near("120 C clamps to 50", WeatherMapper.MapTemperature(Snap(t: 120f), options), 50f);
            Assert.Near("-80 C clamps to -50", WeatherMapper.MapTemperature(Snap(t: -80f), options), -50f);

            // The engine picks rain vs snow from the visual temperature, so snow above freezing
            // has to be pulled below it or the game draws rain. See README "Rain vs snow".
            Assert.Near("snow at +3 C is forced below freezing",
                WeatherMapper.MapTemperature(Snap(t: 3f, snow: 0.5f), options), -1.5f);

            var noForce = options;
            noForce.ForceSnowAppearance = false;
            Assert.Near("...unless the option is off",
                WeatherMapper.MapTemperature(Snap(t: 3f, snow: 0.5f), noForce), 3f);

            Assert.Near("already-cold snow is untouched",
                WeatherMapper.MapTemperature(Snap(t: -6f, snow: 0.5f), options), -6f);

            Assert.Section("Precipitation curve (mm/h)");
            Assert.Near("dry", WeatherMapper.MapPrecipitation(Snap()), 0f);
            Assert.Near("0.1 drizzle", WeatherMapper.MapPrecipitation(Snap(p: 0.1f, rain: 0.1f)), 0.10f);
            Assert.Near("0.5 light", WeatherMapper.MapPrecipitation(Snap(p: 0.5f, rain: 0.5f)), 0.25f);
            Assert.Near("2.5 moderate", WeatherMapper.MapPrecipitation(Snap(p: 2.5f, rain: 2.5f)), 0.50f);
            Assert.Near("7.5 heavy", WeatherMapper.MapPrecipitation(Snap(p: 7.5f, rain: 7.5f)), 0.75f);
            Assert.Near("30 clamps to 1", WeatherMapper.MapPrecipitation(Snap(p: 30f, rain: 30f)), 1.00f);
            Assert.Near("1.5 interpolates", WeatherMapper.MapPrecipitation(Snap(p: 1.5f, rain: 1.5f)), 0.375f);

            Assert.Section("Snow curve (cm/h) and code floor");
            Assert.Near("1 cm snow beats its water equivalent",
                WeatherMapper.MapPrecipitation(Snap(p: 0.7f, snow: 1.0f)), 0.55f);
            Assert.Near("6 cm snow saturates", WeatherMapper.MapPrecipitation(Snap(p: 4.2f, snow: 6.0f)), 1.00f);
            Assert.Near("code 61 with no measured rain", WeatherMapper.MapPrecipitation(Snap(code: 61)), 0.20f);
            Assert.Near("code 65 with no measured rain", WeatherMapper.MapPrecipitation(Snap(code: 65)), 0.60f);
            Assert.Near("code 3 overcast stays dry", WeatherMapper.MapPrecipitation(Snap(code: 3)), 0f);
            Assert.Near("measured amount beats the floor",
                WeatherMapper.MapPrecipitation(Snap(p: 7.5f, rain: 7.5f, code: 61)), 0.75f);
            Assert.Near("components used when total is missing",
                WeatherMapper.MapPrecipitation(Snap(rain: 1.5f, showers: 1.0f)), 0.50f);

            Assert.Section("Fog");
            Assert.Near("no data", WeatherMapper.MapFog(Snap()), 0f);
            Assert.Near("overcast is NOT fog", WeatherMapper.MapFog(Snap(cloud: 100f, code: 3)), 0f);
            Assert.Near("code 45 fog", WeatherMapper.MapFog(Snap(code: 45)), 0.72f);
            Assert.Near("code 48 rime fog", WeatherMapper.MapFog(Snap(code: 48)), 0.82f);
            Assert.Near("43 km visibility", WeatherMapper.MapFog(Snap(vis: 43000f)), 0f);
            Assert.Near("10 km visibility", WeatherMapper.MapFog(Snap(vis: 10000f)), 0f);
            Assert.Near("4 km visibility", WeatherMapper.MapFog(Snap(vis: 4000f)), 0.20f);
            Assert.Near("1 km visibility", WeatherMapper.MapFog(Snap(vis: 1000f)), 0.50f);
            Assert.Near("100 m visibility", WeatherMapper.MapFog(Snap(vis: 100f)), 0.90f);
            Assert.Near("low visibility in heavy rain is capped",
                WeatherMapper.MapFog(Snap(p: 8f, rain: 8f, code: 65, vis: 800f)), 0.25f);
            Assert.Near("fog code beats good visibility",
                WeatherMapper.MapFog(Snap(code: 45, vis: 20000f)), 0.72f);

            var noFog = options;
            noFog.SyncFog = false;
            Assert.Near("fog disabled", WeatherMapper.Map(Snap(code: 45), noFog).Fog, 0f);

            Assert.Section("End to end");
            var mild = WeatherMapper.Map(Snap(t: 32f, cloud: 48f, code: 1), options);
            Assert.Near("mild temperature", mild.TemperatureCelsius, 32f);
            Assert.Near("mild cloudiness", mild.Cloudiness, 0.48f);
            Assert.Near("mild precipitation", mild.Precipitation, 0f);
            Assert.Near("mild fog", mild.Fog, 0f);

            var blizzard = WeatherMapper.Map(Snap(t: -4f, cloud: 100f, snow: 3f, p: 2.1f, code: 75, vis: 600f), options);
            Assert.Near("blizzard temperature", blizzard.TemperatureCelsius, -4f);
            Assert.Near("blizzard cloudiness", blizzard.Cloudiness, 1f);
            Assert.Near("blizzard precipitation", blizzard.Precipitation, 0.80f);
            Assert.Near("blizzard fog capped", blizzard.Fog, 0.25f);

            var empty = WeatherMapper.Map(null, options);
            Assert.Near("null snapshot temperature", empty.TemperatureCelsius, 0f);
            Assert.Near("null snapshot precipitation", empty.Precipitation, 0f);

            Assert.Section("Interpolation");
            var a = new ClimateTarget { TemperatureCelsius = 0f, Cloudiness = 0f, Precipitation = 0f, Fog = 0f };
            var b = new ClimateTarget { TemperatureCelsius = 10f, Cloudiness = 1f, Precipitation = 0.5f, Fog = 0.2f };
            Assert.Near("t=0", ClimateTarget.Lerp(a, b, 0f).TemperatureCelsius, 0f);
            Assert.Near("t=0.5", ClimateTarget.Lerp(a, b, 0.5f).TemperatureCelsius, 5f);
            Assert.Near("t=1", ClimateTarget.Lerp(a, b, 1f).Cloudiness, 1f);
            Assert.Near("t below 0 clamps", ClimateTarget.Lerp(a, b, -3f).Fog, 0f);
            Assert.Near("t above 1 clamps", ClimateTarget.Lerp(a, b, 4f).Fog, 0.2f);

            Assert.Section("Opposite day");
            var opposite = options;
            opposite.OppositeDay = true;

            var flipped = WeatherMapper.Map(Snap(t: 25f, cloud: 10f), opposite);
            Assert.Near("25 C mirrors to 5 C", flipped.TemperatureCelsius, 5f);
            Assert.Near("10% cloud becomes 90%", flipped.Cloudiness, 0.9f);
            Assert.Near("dry becomes soaking", flipped.Precipitation, 1f);
            Assert.Near("fog is deliberately NOT inverted", flipped.Fog, 0f);

            var flipped2 = WeatherMapper.Map(Snap(t: -5f, cloud: 100f, p: 7.5f, rain: 7.5f), opposite);
            Assert.Near("-5 C mirrors to 35 C", flipped2.TemperatureCelsius, 35f);
            Assert.Near("overcast becomes clear", flipped2.Cloudiness, 0f);
            Assert.Near("heavy rain becomes nearly dry", flipped2.Precipitation, 0.25f);
            Assert.Near("mirrored temperature still clamps",
                WeatherMapper.Map(Snap(t: -50f, cloud: 0f), opposite).TemperatureCelsius, 50f);
            Assert.Near("a fog code still produces fog", WeatherMapper.Map(Snap(code: 45), opposite).Fog, 0.72f);

            Assert.Section("WMO condition names");
            Assert.Equal("code 0", WeatherCodes.EnglishFor(0), "Clear sky");
            Assert.Equal("code 61", WeatherCodes.EnglishFor(61), "Slight rain");
            Assert.Equal("code 75", WeatherCodes.EnglishFor(75), "Heavy snow");
            Assert.Equal("code 45", WeatherCodes.EnglishFor(45), "Fog");
            Assert.Equal("unknown code", WeatherCodes.EnglishFor(1234), "Unknown conditions");
            Assert.Equal("locale id for 61", WeatherCodes.LocaleIdFor(61), "RealWeatherSync.Wmo.RainSlight");

            var ids = 0;
            var blanks = 0;
            foreach (var pair in WeatherCodes.All())
            {
                ids++;
                if (string.IsNullOrEmpty(pair.Key) || string.IsNullOrEmpty(pair.Value))
                {
                    blanks++;
                }
            }

            Assert.True("every entry has an id and a name", blanks == 0);
            Assert.True("table has " + ids + " entries", ids >= 28);
        }

        internal static WeatherSnapshot Snap(float t = 15f, float cloud = 0f, float p = 0f, float rain = 0f,
            float showers = 0f, float snow = 0f, int code = 0, float? vis = null)
        {
            return new WeatherSnapshot
            {
                TemperatureCelsius = t,
                CloudCoverPercent = cloud,
                PrecipitationMm = p,
                RainMm = rain,
                ShowersMm = showers,
                SnowfallCm = snow,
                WeatherCode = code,
                VisibilityMeters = vis
            };
        }
    }
}
