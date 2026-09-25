using RealWeatherSync.Mapping;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Covers <see cref="WeatherEventPolicy"/>: when the game is allowed to create weather
    /// events - tornado, hail storm, lightning strike - while this mod is driving the weather.
    ///
    /// The rule this protects is narrow on purpose. The game rolls those events from temperature,
    /// rain and cloudiness alone, so widening "counts as a storm" beyond the three thunderstorm
    /// codes silently widens how often a synced city gets a tornado. That is what the option
    /// exists to stop.
    /// </summary>
    public static class WeatherEventPolicyTests
    {
        public static void Run()
        {
            Assert.Section("Thunderstorm codes");

            Assert.True("95 is a thunderstorm", WeatherCodes.IsThunderstorm(95));
            Assert.True("96 is a thunderstorm with slight hail", WeatherCodes.IsThunderstorm(96));
            Assert.True("99 is a thunderstorm with heavy hail", WeatherCodes.IsThunderstorm(99));

            // The near misses. 82 is "violent rain showers" and 75 is heavy snow: both are severe
            // weather in plain language, neither is a thunderstorm.
            Assert.True("0 (clear) is not", !WeatherCodes.IsThunderstorm(0));
            Assert.True("65 (heavy rain) is not", !WeatherCodes.IsThunderstorm(65));
            Assert.True("75 (heavy snow) is not", !WeatherCodes.IsThunderstorm(75));
            Assert.True("82 (violent showers) is not", !WeatherCodes.IsThunderstorm(82));
            Assert.True("97 is not a WMO code the mod knows", !WeatherCodes.IsThunderstorm(97));

            Assert.Section("Weather event gate");

            // Option off: never interfere, whatever the weather is doing.
            Assert.True("option off, calm -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(false, true, 0));
            Assert.True("option off, thunderstorm -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(false, true, 95));

            // Not driving the weather: the values the game rolls against are its own, so the
            // generator is none of our business even with the option on.
            Assert.True("not driving, calm -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(true, false, 0));
            Assert.True("not driving, thunderstorm -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(true, false, 95));

            // The case the option exists for: a calm real day must not produce a tornado.
            Assert.True("driving, clear -> suppressed",
                WeatherEventPolicy.ShouldSuppressEvents(true, true, 0));
            Assert.True("driving, heavy rain -> suppressed",
                WeatherEventPolicy.ShouldSuppressEvents(true, true, 65));
            Assert.True("driving, violent showers -> suppressed",
                WeatherEventPolicy.ShouldSuppressEvents(true, true, 82));

            // A real thunderstorm hands the generator back.
            Assert.True("driving, thunderstorm -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(true, true, 95));
            Assert.True("driving, thunderstorm with hail -> allowed",
                !WeatherEventPolicy.ShouldSuppressEvents(true, true, 99));
        }
    }
}
