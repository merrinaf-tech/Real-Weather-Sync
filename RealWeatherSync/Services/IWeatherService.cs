using System.Threading;
using System.Threading.Tasks;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Retrieves conditions for a pair of coordinates.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Fetches the conditions at the given coordinates.
        /// </summary>
        /// <param name="timeShiftHours">
        /// 0 for current conditions. Negative reads a past hour, positive reads a forecast hour.
        /// This shifts only which weather reading is used - it never touches the game clock.
        /// </param>
        /// <returns>A snapshot, never <c>null</c>.</returns>
        /// <exception cref="WeatherProviderException">
        /// Transport failure, HTTP error, rate limiting, or an unusable response body.
        /// </exception>
        Task<WeatherSnapshot> GetWeatherAsync(double latitude, double longitude, int timeShiftHours,
            CancellationToken cancellationToken);
    }
}
