using System.Collections.Generic;
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
        /// "New York, United States", ...) to the single best match.
        /// </summary>
        /// <returns>
        /// The best matching location, or <c>null</c> when nothing matched.
        /// Implementations throw <see cref="WeatherProviderException"/> for transport
        /// and protocol failures; "no result" is not an error.
        /// </returns>
        Task<LocationResult> ResolveLocationAsync(string query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns every plausible match for <paramref name="query"/>, best ranked first, so the
        /// player can pick the right one instead of trusting a single guess.
        /// </summary>
        /// <returns>An empty list when nothing matched; never <c>null</c>.</returns>
        Task<IReadOnlyList<LocationResult>> SearchLocationsAsync(string query, int maxResults,
            CancellationToken cancellationToken);
    }
}
