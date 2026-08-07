using System.Threading;
using System.Threading.Tasks;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Turns a free-form city name into coordinates. Kept separate from
    /// <see cref="IWeatherService"/> so a different geocoder could be dropped in
    /// without touching the weather provider.
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Resolves <paramref name="query"/> ("Lyon", "Lyon, France",
        /// "New York, United States", ...).
        /// </summary>
        /// <returns>
        /// The best matching location, or <c>null</c> when nothing matched.
        /// Implementations throw <see cref="WeatherProviderException"/> for transport
        /// and protocol failures; "no result" is not an error.
        /// </returns>
        Task<LocationResult> ResolveLocationAsync(string query, CancellationToken cancellationToken);
    }
}
