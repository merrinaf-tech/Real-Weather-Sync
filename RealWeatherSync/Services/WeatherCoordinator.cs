using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Colossal.Logging;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Localization;
using RealWeatherSync.Models;

namespace RealWeatherSync.Services
{
    /// <summary>
    /// Owns everything asynchronous: geocoding, weather refreshes, retry backoff
    /// and the "latest known good" snapshot.
    ///
    /// Threading contract
    ///   - The public methods are called from the game's main thread (the settings
    ///     setters and <see cref="Systems.RealWeatherSystem"/>).
    ///   - The actual HTTP work and JSON parsing happen on thread pool threads.
    ///   - No game API is touched anywhere in this file. Results are handed back by
    ///     polling (<see cref="TryTakeSnapshot"/> / <see cref="TryTakeLocation"/>) so
    ///     that only the main thread ever reaches ClimateSystem.
    ///
    /// At most one operation runs at a time. Automatic refreshes are dropped while
    /// one is in flight; player initiated actions cancel the running one and are
    /// chained behind it, so there is never an unbounded pile of tasks.
    /// </summary>
    public sealed class WeatherCoordinator : IDisposable
    {
        /// <summary>Backoff steps used after a failed refresh, in seconds.</summary>
        private static readonly double[] BackoffSeconds = { 30, 60, 120, 300, 600 };

        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly ILog _log;

        private readonly object _gate = new object();
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private readonly Stopwatch _clock = Stopwatch.StartNew();

        private Task _currentTask = Task.FromResult(0);
        private CancellationTokenSource _operationCts;

        private LocationResult _location;
        private long _locationSequence;
        private long _locationTakenSequence;

        private WeatherSnapshot _snapshot;
        private long _snapshotSequence;
        private long _snapshotTakenSequence;

        private int _consecutiveFailures;
        private double _nextAttemptSeconds = -1.0;

        /// <summary>Last interval handed to <see cref="Tick"/>; used to schedule the slot after a success.</summary>
        private double _intervalSeconds = 15 * 60.0;

        private IReadOnlyList<LocationResult> _candidates = new List<LocationResult>();
        private int _candidatesVersion;
        private bool _searchInProgress;

        private int _timeShiftHours;
        private bool _antipodeMode;

        private int _disposed;

        public WeatherCoordinator(ILocationService locationService, IWeatherService weatherService, ILog log)
        {
            if (locationService == null)
            {
                throw new ArgumentNullException("locationService");
            }

            if (weatherService == null)
            {
                throw new ArgumentNullException("weatherService");
            }

            _locationService = locationService;
            _weatherService = weatherService;
            _log = log;
        }

        /// <summary>Monotonic real time in seconds, unaffected by pause or simulation speed.</summary>
        public double NowSeconds
        {
            get { return _clock.Elapsed.TotalSeconds; }
        }

        public bool HasLocation
        {
            get { lock (_gate) { return _location != null; } }
        }

        public LocationResult Location
        {
            get { lock (_gate) { return _location; } }
        }

        public bool HasWeather
        {
            get { lock (_gate) { return _snapshot != null; } }
        }

        /// <summary>The last successfully retrieved weather, kept across failures.</summary>
        public WeatherSnapshot LastKnownGoodWeather
        {
            get { lock (_gate) { return _snapshot; } }
        }

        public bool IsBusy
        {
            get { lock (_gate) { return !_currentTask.IsCompleted; } }
        }

        /// <summary>
        /// How far the weather reading is shifted from "now", in hours. Only the reading moves;
        /// the game clock is never touched.
        /// </summary>
        public int TimeShiftHours
        {
            get { lock (_gate) { return _timeShiftHours; } }
            set
            {
                lock (_gate)
                {
                    if (_timeShiftHours == value)
                    {
                        return;
                    }

                    _timeShiftHours = value;
                    // The stored reading is for the old offset; fetch again promptly.
                    _nextAttemptSeconds = -1.0;
                }
            }
        }

        /// <summary>
        /// When true, the weather is fetched for the point diametrically opposite the resolved
        /// city. The stored location is untouched - only the request coordinates are mirrored.
        /// </summary>
        public bool AntipodeMode
        {
            get { lock (_gate) { return _antipodeMode; } }
            set
            {
                lock (_gate)
                {
                    if (_antipodeMode == value)
                    {
                        return;
                    }

                    _antipodeMode = value;
                    // The stored reading is for the other side of the planet; fetch again now.
                    _nextAttemptSeconds = -1.0;
                }
            }
        }

        /// <summary>Candidates from the last <see cref="RequestSearch"/>, best ranked first.</summary>
        public IReadOnlyList<LocationResult> Candidates
        {
            get { lock (_gate) { return _candidates; } }
        }

        /// <summary>
        /// Increments whenever the candidate list changes. Wired to the options page through
        /// SettingsUIValueVersion so the results dropdown refreshes without reopening the page.
        /// </summary>
        public int CandidatesVersion
        {
            get { lock (_gate) { return _candidatesVersion; } }
        }

        public bool SearchInProgress
        {
            get { lock (_gate) { return _searchInProgress; } }
        }

        /// <summary>
        /// Restores a location that was persisted in the settings, without issuing a
        /// geocoding request.
        /// </summary>
        public void RestoreLocation(LocationResult location)
        {
            if (location == null || !location.HasValidCoordinates)
            {
                return;
            }

            lock (_gate)
            {
                _location = location;
                // Deliberately not bumping _locationSequence: the settings already
                // hold this value, there is nothing to write back.
                _nextAttemptSeconds = -1.0;
                _consecutiveFailures = 0;
            }

            StatusReport.SetLocation(location.DisplayName, location.Latitude, location.Longitude);
            Log("Restored persisted location " + location.DisplayName + " (" + Format(location) + ")");
        }

        /// <summary>
        /// Player pressed "Apply City". Geocodes the query and, on success, fetches
        /// the weather straight away. A previously resolved location is kept if the
        /// query cannot be resolved.
        /// </summary>
        public void RequestApplyCity(string query)
        {
            var trimmed = query == null ? string.Empty : query.Trim();

            if (trimmed.Length == 0)
            {
                StatusReport.Set(StatusKind.CityNotConfigured,
                    Translation.Get(LocaleKeys.ErrorEmptyCity, "Enter a city name first"));
                Log("Apply City ignored: the city field is empty.");
                return;
            }

            Log("City resolution started for \"" + trimmed + "\"");
            StatusReport.Set(StatusKind.ResolvingLocation);
            Enqueue(token => ResolveThenRefreshAsync(trimmed, token), true);
        }

        /// <summary>
        /// Player pressed "Search". Fills <see cref="Candidates"/> so they can pick the right city
        /// instead of trusting a single guess. Does not change the active location.
        /// </summary>
        public void RequestSearch(string query)
        {
            var trimmed = query == null ? string.Empty : query.Trim();

            if (trimmed.Length == 0)
            {
                SetCandidates(new List<LocationResult>());
                StatusReport.Set(StatusKind.CityNotConfigured,
                    Translation.Get(LocaleKeys.ErrorEmptyCity, "Enter a city name first"));
                return;
            }

            lock (_gate)
            {
                _searchInProgress = true;
            }

            Log("City search started for \"" + trimmed + "\"");
            StatusReport.Set(StatusKind.ResolvingLocation);
            Enqueue(token => SearchAsync(trimmed, token), true);
        }

        /// <summary>
        /// Player picked one of the <see cref="Candidates"/>. Becomes the active location and
        /// triggers an immediate refresh.
        /// </summary>
        public void ApplyCandidate(LocationResult location)
        {
            if (location == null || !location.HasValidCoordinates)
            {
                return;
            }

            lock (_gate)
            {
                _location = location;
                _locationSequence++;
                _consecutiveFailures = 0;
                _nextAttemptSeconds = -1.0;
            }

            StatusReport.SetLocation(location.DisplayName, location.Latitude, location.Longitude);
            Log("City selected from search results: " + location.DisplayName + " (" + Format(location) + ")");

            Enqueue(token => RefreshAsync(location, token), true);
        }

        private async Task SearchAsync(string query, CancellationToken token)
        {
            try
            {
                var results = await _locationService
                    .SearchLocationsAsync(query, 10, token)
                    .ConfigureAwait(false);

                SetCandidates(results);

                if (results.Count == 0)
                {
                    LogWarn("No geocoding result for \"" + query + "\".");
                    StatusReport.Set(StatusKind.ErrorResolvingCity,
                        Translation.Get(LocaleKeys.ErrorCityNotFound, "No matching city found"));
                    return;
                }

                Log("City search returned " + results.Count + " candidate(s) for \"" + query + "\".");
                StatusReport.Set(StatusKind.CandidatesReady,
                    results.Count.ToString(System.Globalization.CultureInfo.CurrentCulture));
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Log("City search for \"" + query + "\" was cancelled.");
            }
            catch (OperationCanceledException)
            {
                LogWarn("City search for \"" + query + "\" timed out.");
                StatusReport.Set(StatusKind.ErrorResolvingCity,
                    Translation.Get(LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo"));
            }
            catch (WeatherProviderException e)
            {
                LogWarn("City search failed: " + e.Message);
                StatusReport.Set(StatusKind.ErrorResolvingCity, DescribeProviderError(e));
            }
            catch (Exception e)
            {
                LogError("Unexpected error while searching \"" + query + "\"", e);
                StatusReport.Set(StatusKind.ErrorResolvingCity, e.Message);
            }
            finally
            {
                lock (_gate)
                {
                    _searchInProgress = false;
                }
            }
        }

        private void SetCandidates(IReadOnlyList<LocationResult> results)
        {
            lock (_gate)
            {
                _candidates = results ?? new List<LocationResult>();
                _candidatesVersion++;
            }
        }

        /// <summary>Player pressed "Refresh Weather Now".</summary>
        public void RequestImmediateRefresh()
        {
            LocationResult location;
            lock (_gate)
            {
                location = _location;
            }

            if (location == null)
            {
                StatusReport.Set(StatusKind.CityNotConfigured);
                Log("Manual refresh ignored: no location resolved.");
                return;
            }

            Log("Manual weather refresh requested.");
            Enqueue(token => RefreshAsync(location, token), true);
        }

        /// <summary>
        /// Called once per game update from the main thread. Starts an automatic
        /// refresh when the configured interval (or the current backoff) has elapsed.
        /// </summary>
        public void Tick(double updateIntervalSeconds)
        {
            if (_disposed != 0)
            {
                return;
            }

            LocationResult location;
            double due;
            var now = NowSeconds;

            lock (_gate)
            {
                _intervalSeconds = Math.Max(60.0, updateIntervalSeconds);

                if (!_currentTask.IsCompleted)
                {
                    return;
                }

                location = _location;
                due = _nextAttemptSeconds;

                if (location == null)
                {
                    return;
                }

                if (due >= 0.0 && now < due)
                {
                    return;
                }

                // Reserve the next slot before starting, so a request that somehow
                // never reaches its continuation cannot turn into a tight retry loop.
                // The operation overwrites this with the real value when it finishes.
                _nextAttemptSeconds = now + _intervalSeconds;
            }

            if (due < 0.0)
            {
                Log("First weather refresh for " + location.DisplayName + ".");
            }

            Enqueue(token => RefreshAsync(location, token), false);
        }

        /// <summary>
        /// Main thread pickup for a newly resolved location, so it can be written
        /// into the settings. Returns false when nothing new has arrived.
        /// </summary>
        public bool TryTakeLocation(out LocationResult location)
        {
            lock (_gate)
            {
                if (_locationSequence == _locationTakenSequence)
                {
                    location = null;
                    return false;
                }

                _locationTakenSequence = _locationSequence;
                location = _location;
                return location != null;
            }
        }

        /// <summary>
        /// Main thread pickup for a newly received weather snapshot. Returns false
        /// when the last snapshot has already been consumed.
        /// </summary>
        public bool TryTakeSnapshot(out WeatherSnapshot snapshot)
        {
            lock (_gate)
            {
                if (_snapshotSequence == _snapshotTakenSequence)
                {
                    snapshot = null;
                    return false;
                }

                _snapshotTakenSequence = _snapshotSequence;
                snapshot = _snapshot;
                return snapshot != null;
            }
        }

        /// <summary>
        /// Forces the next <see cref="Tick"/> to refresh, e.g. after the player
        /// re-enabled the mod.
        /// </summary>
        public void InvalidateSchedule()
        {
            lock (_gate)
            {
                _nextAttemptSeconds = -1.0;
                _consecutiveFailures = 0;
            }
        }

        /// <summary>Makes the current snapshot available for pickup again after a reset.</summary>
        public void RearmSnapshotPickup()
        {
            lock (_gate)
            {
                if (_snapshot != null)
                {
                    _snapshotTakenSequence = _snapshotSequence - 1;
                }
            }
        }

        private async Task ResolveThenRefreshAsync(string query, CancellationToken token)
        {
            LocationResult resolved;

            try
            {
                resolved = await _locationService.ResolveLocationAsync(query, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Log("City resolution for \"" + query + "\" was cancelled.");
                return;
            }
            catch (OperationCanceledException)
            {
                // The token was not cancelled, so this is the HttpClient timeout.
                LogWarn("City resolution for \"" + query + "\" timed out.");
                StatusReport.Set(StatusKind.ErrorResolvingCity,
                    Translation.Get(LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo"));
                return;
            }
            catch (WeatherProviderException e)
            {
                LogWarn("City resolution failed: " + e.Message);
                StatusReport.Set(StatusKind.ErrorResolvingCity, DescribeProviderError(e));
                return;
            }
            catch (Exception e)
            {
                LogError("Unexpected error while resolving \"" + query + "\"", e);
                StatusReport.Set(StatusKind.ErrorResolvingCity, e.Message);
                return;
            }

            if (resolved == null)
            {
                // A failed lookup must not throw away a location that already works.
                LogWarn("No geocoding result for \"" + query + "\"; keeping the previous location.");
                StatusReport.Set(StatusKind.ErrorResolvingCity,
                    Translation.Get(LocaleKeys.ErrorCityNotFound, "No matching city found"));
                return;
            }

            lock (_gate)
            {
                _location = resolved;
                _locationSequence++;
                _consecutiveFailures = 0;
                _nextAttemptSeconds = -1.0;
            }

            StatusReport.SetLocation(resolved.DisplayName, resolved.Latitude, resolved.Longitude);
            Log("City resolved: " + resolved.DisplayName + " (" + Format(resolved) +
                ", timezone " + (string.IsNullOrEmpty(resolved.Timezone) ? "unknown" : resolved.Timezone) + ")");

            if (token.IsCancellationRequested)
            {
                return;
            }

            await RefreshAsync(resolved, token).ConfigureAwait(false);
        }

        private async Task RefreshAsync(LocationResult location, CancellationToken token)
        {
            if (location == null || !location.HasValidCoordinates)
            {
                return;
            }

            int shift;
            bool antipode;
            lock (_gate)
            {
                shift = _timeShiftHours;
                antipode = _antipodeMode;
            }

            // Mirroring happens only for the request; the resolved city stays what the player chose.
            var requestPoint = antipode ? location.CreateAntipode() : location;

            StatusReport.Set(StatusKind.Refreshing);
            Log("Weather refresh started for " + requestPoint.DisplayName +
                (antipode ? " [antipode of " + location.DisplayName + "]" : string.Empty) +
                (shift == 0 ? "." : " (time shift " + shift.ToString(System.Globalization.CultureInfo.InvariantCulture) + " h)."));

            WeatherSnapshot snapshot;
            try
            {
                snapshot = await _weatherService
                    .GetWeatherAsync(requestPoint.Latitude, requestPoint.Longitude, shift, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Log("Weather refresh cancelled.");
                return;
            }
            catch (OperationCanceledException)
            {
                // Not our cancellation: the HttpClient timeout elapsed. That is a
                // failure and must feed the backoff, not be silently swallowed.
                OnRefreshFailed(Translation.Get(LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo"), false);
                LogWarn("Weather refresh timed out.");
                return;
            }
            catch (WeatherProviderException e)
            {
                OnRefreshFailed(DescribeProviderError(e), e.RateLimited);
                LogWarn("Weather refresh failed: " + e.Message);
                return;
            }
            catch (Exception e)
            {
                OnRefreshFailed(e.Message, false);
                LogError("Unexpected error during weather refresh", e);
                return;
            }

            if (snapshot == null)
            {
                OnRefreshFailed("empty response", false);
                return;
            }

            lock (_gate)
            {
                _snapshot = snapshot;
                _snapshotSequence++;
                _consecutiveFailures = 0;
                _nextAttemptSeconds = NowSeconds + _intervalSeconds;
            }

            StatusReport.RecordSuccess(snapshot);
            StatusReport.Set(StatusKind.Connected);
            Log("Weather received for " + location.DisplayName + ": " + snapshot);
        }

        private void OnRefreshFailed(string detail, bool rateLimited)
        {
            int failures;
            lock (_gate)
            {
                _consecutiveFailures++;
                failures = _consecutiveFailures;
            }

            var index = Math.Min(failures - 1, BackoffSeconds.Length - 1);
            var delay = BackoffSeconds[index];

            // Rate limiting deserves a longer pause than a transient network error.
            if (rateLimited)
            {
                delay = Math.Max(delay, BackoffSeconds[BackoffSeconds.Length - 1]);
            }

            lock (_gate)
            {
                _nextAttemptSeconds = NowSeconds + delay;
            }

            StatusReport.Set(StatusKind.Offline, detail);
            LogWarn("Weather refresh failure #" + failures + "; retrying in " +
                    delay.ToString(System.Globalization.CultureInfo.InvariantCulture) + " s.");
        }

        private static string DescribeProviderError(WeatherProviderException e)
        {
            return e.RateLimited
                ? Translation.Get(LocaleKeys.ErrorRateLimited, "Open-Meteo is rate limiting requests")
                : Translation.Get(LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo");
        }

        /// <summary>
        /// Serialises operations. <paramref name="userInitiated"/> requests cancel
        /// whatever is running; automatic ones are simply skipped while busy.
        /// </summary>
        private void Enqueue(Func<CancellationToken, Task> work, bool userInitiated)
        {
            if (_disposed != 0 || _shutdown.IsCancellationRequested)
            {
                return;
            }

            lock (_gate)
            {
                if (!_currentTask.IsCompleted)
                {
                    if (!userInitiated)
                    {
                        return;
                    }

                    if (_operationCts != null)
                    {
                        try
                        {
                            _operationCts.Cancel();
                        }
                        catch (ObjectDisposedException)
                        {
                            // Already finished; nothing to cancel.
                        }
                    }
                }

                var cts = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token);
                _operationCts = cts;
                _currentTask = RunChainedAsync(_currentTask, work, cts);
            }
        }

        private async Task RunChainedAsync(Task previous, Func<CancellationToken, Task> work, CancellationTokenSource cts)
        {
            try
            {
                await previous.ConfigureAwait(false);
            }
            catch (Exception)
            {
                // The previous operation already reported its own failure.
            }

            try
            {
                if (cts.IsCancellationRequested || _shutdown.IsCancellationRequested || _disposed != 0)
                {
                    return;
                }

                await work(cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown or when superseded by a newer request.
            }
            catch (Exception e)
            {
                // Last line of defence: an operation body must never surface an
                // unobserved task exception.
                LogError("Weather operation faulted", e);
            }
            finally
            {
                try
                {
                    cts.Dispose();
                }
                catch (Exception)
                {
                    // Nothing sensible to do here.
                }
            }
        }

        private static string Format(LocationResult location)
        {
            var ci = System.Globalization.CultureInfo.InvariantCulture;
            return location.Latitude.ToString("0.####", ci) + ", " + location.Longitude.ToString("0.####", ci);
        }

        private void Log(string message)
        {
            if (_log != null)
            {
                _log.Info(message);
            }
        }

        private void LogWarn(string message)
        {
            if (_log != null)
            {
                _log.Warn(message);
            }
        }

        private void LogError(string message, Exception e)
        {
            if (_log != null)
            {
                _log.Error(e, message);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            try
            {
                _shutdown.Cancel();
            }
            catch (Exception)
            {
                // Cancellation callbacks are not ours; ignore whatever they raise.
            }

            // No blocking wait here: in-flight requests observe the cancellation and
            // unwind on their own. Disposing the token source is left to the runtime
            // so a racing continuation cannot hit a disposed source.
        }
    }
}
