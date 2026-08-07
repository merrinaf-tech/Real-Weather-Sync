namespace RealWeatherSync.Settings
{
    /// <summary>
    /// How often the mod asks Open-Meteo for fresh conditions. The numeric values
    /// are the interval in minutes, so no lookup table is needed.
    /// </summary>
    public enum UpdateIntervalOption
    {
        FifteenMinutes = 15,
        ThirtyMinutes = 30,
        SixtyMinutes = 60
    }
}
