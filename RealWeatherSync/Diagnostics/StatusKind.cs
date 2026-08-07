namespace RealWeatherSync.Diagnostics
{
    /// <summary>
    /// The states Real Weather Sync reports back to the player in the options page.
    /// </summary>
    public enum StatusKind
    {
        /// <summary>The mod is switched off in its own settings.</summary>
        Disabled,

        /// <summary>No city has been applied yet.</summary>
        CityNotConfigured,

        /// <summary>A geocoding request is in flight.</summary>
        ResolvingLocation,

        /// <summary>A weather request is in flight.</summary>
        Refreshing,

        /// <summary>A city is resolved and at least one weather response has been received.</summary>
        Connected,

        /// <summary>The last refresh failed; the previous valid weather is still being used.</summary>
        Offline,

        /// <summary>The entered city could not be resolved.</summary>
        ErrorResolvingCity,

        /// <summary>Another mod that overrides visual climate values is active.</summary>
        IncompatibleModActive,

        /// <summary>Overrides were released on the player's request.</summary>
        ReleasedByPlayer,

        /// <summary>Waiting for a normal game to be loaded (main menu, editor, ...).</summary>
        WaitingForGame
    }
}
