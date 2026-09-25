using System;
using Game.Simulation;

namespace RealWeatherSync.Systems
{
    /// <summary>
    /// Owns one bit of game state: whether <c>Game.Simulation.WeatherHazardSystem</c> is allowed
    /// to run.
    ///
    /// That system's <c>OnUpdate</c> does exactly one thing - schedule the job that creates
    /// weather phenomenon events - so switching it off stops those events and nothing else. No
    /// entity is created or destroyed and nothing reaches the save, which is what keeps this
    /// inside the mod's promise. Switching it back on restores the game's own behaviour at once.
    ///
    /// The gate only ever undoes what it did itself. If the system was already disabled when we
    /// arrived - the player's own choice, or another mod's - we leave it alone and never switch
    /// it back on.
    ///
    /// Must only be used from the main thread.
    /// </summary>
    public sealed class WeatherHazardGate
    {
        private readonly WeatherHazardSystem _hazardSystem;

        private bool _suppressedByUs;

        public WeatherHazardGate(WeatherHazardSystem hazardSystem)
        {
            if (hazardSystem == null)
            {
                throw new ArgumentNullException("hazardSystem");
            }

            _hazardSystem = hazardSystem;
        }

        /// <summary>True while this mod is the reason the generator is off.</summary>
        public bool IsSuppressing
        {
            get { return _suppressedByUs; }
        }

        /// <summary>
        /// Stops the game creating weather events. Does nothing if the generator is already off
        /// for somebody else's reasons, so that we never end up claiming their decision.
        /// </summary>
        /// <returns>True when this call actually switched it off.</returns>
        public bool Suppress()
        {
            if (_suppressedByUs || !_hazardSystem.Enabled)
            {
                return false;
            }

            _hazardSystem.Enabled = false;
            _suppressedByUs = true;
            return true;
        }

        /// <summary>
        /// Gives the generator back to the game. Idempotent, and safe from teardown paths.
        /// </summary>
        /// <returns>True when something was actually restored.</returns>
        public bool Release()
        {
            if (!_suppressedByUs)
            {
                return false;
            }

            _hazardSystem.Enabled = true;
            _suppressedByUs = false;
            return true;
        }
    }
}
