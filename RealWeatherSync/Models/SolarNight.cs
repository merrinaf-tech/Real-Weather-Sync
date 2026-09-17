using System;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// Approximate solar elevation for the real weather point at a UTC instant.
    /// Equations: https://gml.noaa.gov/grad/solcalc/solareqns.PDF
    /// </summary>
    public static class SolarNight
    {
        public static bool IsDark(double latitude, double longitude, DateTimeOffset utc)
        {
            if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsInfinity(latitude)
                || double.IsInfinity(longitude) || latitude < -90 || latitude > 90
                || longitude < -180 || longitude > 180) return false;

            var time = utc.ToUniversalTime();
            var hour = time.Hour + time.Minute / 60.0 + time.Second / 3600.0;
            var daysInYear = DateTime.IsLeapYear(time.Year) ? 366.0 : 365.0;
            var gamma = 2.0 * Math.PI / daysInYear * (time.DayOfYear - 1 + (hour - 12.0) / 24.0);
            var equationOfTime = 229.18 * (0.000075 + 0.001868 * Math.Cos(gamma)
                - 0.032077 * Math.Sin(gamma) - 0.014615 * Math.Cos(2 * gamma)
                - 0.040849 * Math.Sin(2 * gamma));
            var declination = 0.006918 - 0.399912 * Math.Cos(gamma)
                + 0.070257 * Math.Sin(gamma) - 0.006758 * Math.Cos(2 * gamma)
                + 0.000907 * Math.Sin(2 * gamma) - 0.002697 * Math.Cos(3 * gamma)
                + 0.00148 * Math.Sin(3 * gamma);
            var trueSolarMinutes = hour * 60.0 + equationOfTime + 4.0 * longitude;
            var hourAngle = (trueSolarMinutes / 4.0 - 180.0) * Math.PI / 180.0;
            var latRadians = latitude * Math.PI / 180.0;
            var sinElevation = Math.Sin(latRadians) * Math.Sin(declination)
                + Math.Cos(latRadians) * Math.Cos(declination) * Math.Cos(hourAngle);
            // Civil twilight has ended once the centre of the sun is six degrees below the horizon.
            return sinElevation <= -0.10452846326765346;
        }
    }
}
