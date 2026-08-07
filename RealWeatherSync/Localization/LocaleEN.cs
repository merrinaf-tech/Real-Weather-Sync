using System;
using System.Collections.Generic;
using Colossal;
using RealWeatherSync.Settings;

namespace RealWeatherSync.Localization
{
    /// <summary>
    /// en-US strings for Real Weather Sync. Every player facing string the mod
    /// produces has an entry here, including the ones the code builds at runtime
    /// (see <see cref="LocaleKeys"/>), so additional languages only need another
    /// <see cref="IDictionarySource"/> with the same ids.
    /// </summary>
    public class LocaleEN : IDictionarySource
    {
        private readonly RealWeatherSettings _settings;

        public LocaleEN(RealWeatherSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), Mod.Name },

                { _settings.GetOptionTabLocaleID(RealWeatherSettings.MainSection), "Main" },

                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.GeneralGroup), "General" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.ActionsGroup), "Actions" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.StatusGroup), "Status" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.AdvancedGroup), "Advanced" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.AboutGroup), "About" },

                // -- General ------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.EnableRealWeather)),
                    "Enable Real Weather"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.EnableRealWeather)),
                    "Match the visual weather of your city to the current real weather of the city below. " +
                    "Purely cosmetic: the simulation, the clock, the date and the season are never changed. " +
                    "Turning this off immediately hands the weather back to the game."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.CityQuery)),
                    "City"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.CityQuery)),
                    "The real city to copy the weather from. Examples: Lyon - Lyon, France - Milazzo, Italy - " +
                    "New York, United States. Add a country or a region after a comma if the name is ambiguous. " +
                    "Press Apply City afterwards."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SmoothTransitions)),
                    "Smooth Weather Transitions"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SmoothTransitions)),
                    "Fade gradually to each new weather reading over about two minutes of real time instead of " +
                    "switching instantly. The fade uses real time, so it is unaffected by the simulation speed."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.UpdateInterval)),
                    "Update Interval"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.UpdateInterval)),
                    "How often to ask Open-Meteo for fresh conditions. Open-Meteo updates its data roughly " +
                    "every 15 minutes, so shorter intervals gain little."
                },
                {
                    _settings.GetEnumValueLocaleID(UpdateIntervalOption.FifteenMinutes),
                    "15 minutes"
                },
                {
                    _settings.GetEnumValueLocaleID(UpdateIntervalOption.ThirtyMinutes),
                    "30 minutes"
                },
                {
                    _settings.GetEnumValueLocaleID(UpdateIntervalOption.SixtyMinutes),
                    "60 minutes"
                },

                // -- Actions ------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ApplyCity)),
                    "Apply City"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ApplyCity)),
                    "Look the city up, store its coordinates, and fetch its weather straight away. " +
                    "If the city cannot be found, the previously resolved location is kept."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.RefreshWeatherNow)),
                    "Refresh Weather Now"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.RefreshWeatherNow)),
                    "Fetch the current conditions immediately instead of waiting for the next update."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Reset to Game Weather"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Release every climate override and let the game control the weather again. " +
                    "Real weather resumes when you press Apply City or Refresh Weather Now."
                },
                {
                    _settings.GetOptionWarningLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Release all weather overrides and return control to the game?"
                },

                // -- Status -------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.StatusText)),
                    "Status"
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ResolvedLocationText)),
                    "Resolved location"
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.LastUpdateText)),
                    "Last update"
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.CurrentWeatherText)),
                    "Current weather"
                },

                // -- Advanced -----------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SyncFog)),
                    "Synchronise fog"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SyncFog)),
                    "Derive fog from the reported fog codes and from visibility. Cloud cover never produces fog. " +
                    "Turn this off to leave the game's own fog alone."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ForceSnowAppearance)),
                    "Show snow when it is really snowing"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ForceSnowAppearance)),
                    "Cities: Skylines II decides between rain and snow from the visual temperature. When the real " +
                    "weather is snow but the real temperature is above freezing, lower the visual temperature just " +
                    "below freezing so snow is drawn. The displayed temperature will then differ from the real one. " +
                    "The season and the date are still never changed."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.IgnoreModConflicts)),
                    "Ignore mod conflicts"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.IgnoreModConflicts)),
                    "By default Real Weather Sync switches itself off when another known weather mod is loaded, " +
                    "because two mods writing the same climate values will fight. Enable this only if you are sure " +
                    "the other mod is not overriding the weather."
                },

                // -- About --------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.AboutText)),
                    "About"
                },
                {
                    LocaleKeys.AboutText,
                    "Real Weather Sync " + Mod.Version + Environment.NewLine +
                    "Weather data by Open-Meteo (open-meteo.com), licensed CC BY 4.0. No account and no API key required." +
                    Environment.NewLine +
                    "Only the city name you type and the coordinates resolved from it are sent to Open-Meteo."
                },

                // -- Runtime status strings ---------------------------------
                { LocaleKeys.StatusDisabled, "Disabled" },
                { LocaleKeys.StatusCityNotConfigured, "City not configured" },
                { LocaleKeys.StatusResolvingLocation, "Resolving location" },
                { LocaleKeys.StatusRefreshing, "Refreshing weather" },
                { LocaleKeys.StatusConnected, "Connected" },
                { LocaleKeys.StatusOffline, "Offline - using last valid weather" },
                { LocaleKeys.StatusErrorResolvingCity, "Error resolving city" },
                { LocaleKeys.StatusIncompatibleMod, "Incompatible weather mod active" },
                { LocaleKeys.StatusReleased, "Overrides released - using game weather" },
                { LocaleKeys.StatusWaitingForGame, "Waiting for a city to be loaded" },

                { LocaleKeys.LocationNotResolved, "No location resolved yet" },

                { LocaleKeys.LastUpdateNever, "Never" },
                { LocaleKeys.LastUpdateJustNow, "just now" },
                { LocaleKeys.LastUpdateMinutesAgo, "{0} min ago" },

                { LocaleKeys.WeatherNoData, "No weather data received yet." },
                { LocaleKeys.WeatherObserved, "Observed" },
                { LocaleKeys.WeatherApplied, "Applied" },
                { LocaleKeys.WeatherClouds, "clouds" },
                { LocaleKeys.WeatherPrecipitation, "precipitation" },
                { LocaleKeys.WeatherSnow, "snow" },
                { LocaleKeys.WeatherCode, "WMO code" },
                { LocaleKeys.WeatherVisibility, "visibility" },
                { LocaleKeys.WeatherFog, "fog" },

                { LocaleKeys.OverridesActive, "Climate overrides are active." },
                { LocaleKeys.OverridesInactive, "Climate overrides are not active." },

                { LocaleKeys.ErrorCityNotFound, "No matching city found" },
                { LocaleKeys.ErrorEmptyCity, "Enter a city name first" },
                { LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo" },
                { LocaleKeys.ErrorRateLimited, "Open-Meteo is rate limiting requests" }
            };
        }

        public void Unload()
        {
        }
    }
}
