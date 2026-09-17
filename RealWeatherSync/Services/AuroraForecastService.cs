using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>NOAA OVATION grid. All network and parsing work stays off the game thread.</summary>
    public sealed class AuroraForecastService : IDisposable
    {
        private const string Url = "https://services.swpc.noaa.gov/json/ovation_aurora_latest.json";
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RetryInterval = TimeSpan.FromMinutes(5);
        private readonly HttpClient _http;
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Task<AuroraForecast> _request;
        private DateTimeOffset _nextRequest;
        private readonly List<AuroraForecast> _recent = new List<AuroraForecast>();

        public AuroraForecast ForecastFor(DateTimeOffset now)
        {
            AuroraForecast best = null;
            var bestDistance = double.MaxValue;
            foreach (var candidate in _recent)
            {
                if (!candidate.IsForMoment(now)) continue;
                var distance = Math.Abs((candidate.ForecastAt - now).TotalSeconds);
                if (distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }
            return best;
        }

        public AuroraForecastService(string userAgent)
        {
            _http = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }) { Timeout = TimeSpan.FromSeconds(20) };
            _http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        }

        /// <summary>Call only from the main thread. Never blocks it.</summary>
        public void Tick()
        {
            var now = DateTimeOffset.UtcNow;
            if (_request != null && _request.IsCompleted)
            {
                try
                {
                    var forecast = _request.GetAwaiter().GetResult();
                    _recent.RemoveAll(x => x.ForecastAt == forecast.ForecastAt || x.ForecastAt < now.AddMinutes(-10));
                    _recent.Add(forecast);
                    _nextRequest = now + RefreshInterval;
                }
                catch (Exception e)
                {
                    _nextRequest = now + RetryInterval;
                    Mod.Log.Warn("NOAA aurora forecast unavailable: " + e.Message);
                }
                _request = null;
            }

            if (_request == null && now >= _nextRequest)
            {
                _nextRequest = now + RetryInterval;
                _request = FetchAsync(_shutdown.Token);
            }
        }

        private async Task<AuroraForecast> FetchAsync(CancellationToken token)
        {
            using (var response = await _http.GetAsync(Url, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (json.Length > 2 * 1024 * 1024)
                {
                    throw new InvalidOperationException("NOAA response exceeds the expected size.");
                }
                return AuroraForecast.Parse(json);
            }
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            _http.Dispose();
            _shutdown.Dispose();
        }
    }

}
