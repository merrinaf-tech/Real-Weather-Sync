namespace RealWeatherSync.Models
{
    /// <summary>
    /// Keeps the aurora's current-only NOAA contract independent from the weather timeline.
    /// Game-free so the mode combinations can be covered by the offline suite.
    /// </summary>
    public static class AuroraSyncPolicy
    {
        public static bool ShouldRun(bool enabled, bool followGameClock, int timeShiftHours)
        {
            return enabled && (followGameClock || timeShiftHours == 0);
        }
    }
}
