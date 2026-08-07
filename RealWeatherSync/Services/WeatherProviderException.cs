using System;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Raised by a weather / geocoding provider for any failure the caller is
    /// expected to recover from: no connectivity, DNS failure, HTTP error,
    /// timeout, rate limiting, or an unusable response body.
    /// </summary>
    public class WeatherProviderException : Exception
    {
        public WeatherProviderException(string message)
            : base(message)
        {
        }

        public WeatherProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public WeatherProviderException(string message, bool rateLimited)
            : base(message)
        {
            RateLimited = rateLimited;
        }

        /// <summary>True when the provider explicitly refused us for sending too many requests.</summary>
        public bool RateLimited { get; }
    }
}
