namespace RealWeatherSync.Mapping
{
    /// <summary>
    /// Decides whether the game may create weather events - tornado, hail storm, lightning
    /// strike - while this mod is driving the weather.
    ///
    /// <c>Game.Simulation.WeatherHazardSystem</c> rolls for those events from temperature,
    /// precipitation and cloudiness alone, the three values this mod overrides, compared against
    /// each phenomenon prefab's occurrence window. It knows nothing about where those numbers
    /// came from. So a calm real day whose values happen to land inside the tornado prefab's
    /// window produces a tornado in a city whose real counterpart has never had one - which is
    /// exactly what happened to a player following Lyon.
    ///
    /// The fix is not to create events but to withhold permission: the generator runs only while
    /// the real city is genuinely having a thunderstorm. Game-free on purpose, so the offline
    /// suite covers it.
    /// </summary>
    public static class WeatherEventPolicy
    {
        /// <summary>
        /// True when the game's weather event generator should be held off.
        /// </summary>
        /// <param name="limitToRealStorms">The player's setting. Off means never interfere.</param>
        /// <param name="modIsDrivingWeather">
        /// False while the game's own weather is on screen. The mod then has no business touching
        /// the generator: the values it would roll against are not ours to answer for.
        /// </param>
        /// <param name="weatherCode">WMO code of the reading currently being shown.</param>
        public static bool ShouldSuppressEvents(bool limitToRealStorms, bool modIsDrivingWeather, int weatherCode)
        {
            if (!limitToRealStorms || !modIsDrivingWeather)
            {
                return false;
            }

            return !WeatherCodes.IsThunderstorm(weatherCode);
        }
    }
}
