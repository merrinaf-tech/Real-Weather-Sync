using System.Threading;
using System.Threading.Tasks;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Retrieves the current conditions for a pair of coordinates.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Fetches the current weather at the given coordinates.
        /// </summary>
        /// <returns>A snapshot, never <c>null</c>.</returns>
        /// <exception cref="WeatherProviderException">
        /// Transport failure, HTTP error, rate limiting, or an unusable response body.
        /// </exception>
        Task<WeatherSnapshot> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken);
    }
}
