using System;
using System.Collections.Generic;
using Colossal;
using RealWeatherSync.Mapping;
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
            var entries = new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), Mod.Name },

                { _settings.GetOptionTabLocaleID(RealWeatherSettings.MainSection), "Main" },

                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.GeneralGroup), "General" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.SearchGroup), "City" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.ActionsGroup), "Actions" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.StatusGroup), "Status" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.AdvancedGroup), "Advanced" },
                { _settings.GetOptionGroupLocaleID(RealWeatherSettings.SillyGroup), "Options nobody asked for" },
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
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.FollowGameClock)),
                    "Follow the in-game clock"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.FollowGameClock)),
                    "Instead of one frozen reading, walk through the city's last 24 hours of real weather " +
                    "using the in-game hour. If it is 15:00 in your city, you get the real weather that the " +
                    "chosen city had at its most recent 15:00 - so the weather changes as your day goes by. " +
                    "The in-game clock is only read, never changed: the time, date and season stay exactly " +
                    "as the game set them. Overrides the manual time shift while it is on."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SmoothTransitions)),
                    "Smooth Weather Transitions"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SmoothTransitions)),
                    "Fade gradually to each new weather reading instead of switching instantly. " +
                    "The fade uses real time, so it is unaffected by pausing or the simulation speed."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.TransitionSeconds)),
                    "Transition length (seconds)"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.TransitionSeconds)),
                    "How long a fade between two weather readings takes, in real seconds. " +
                    "Only used when smooth transitions are enabled."
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
                { _settings.GetEnumValueLocaleID(UpdateIntervalOption.FifteenMinutes), "15 minutes" },
                { _settings.GetEnumValueLocaleID(UpdateIntervalOption.ThirtyMinutes), "30 minutes" },
                { _settings.GetEnumValueLocaleID(UpdateIntervalOption.SixtyMinutes), "60 minutes" },

                // -- City ---------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.CityQuery)),
                    "City"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.CityQuery)),
                    "The real city to copy the weather from. Examples: Lyon - Lyon, France - Milazzo, Italy - " +
                    "New York, United States. Add a country or a region after a comma to narrow it down."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SearchCity)),
                    "Search"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SearchCity)),
                    "Look the name up and list every matching city below, so you can confirm the right one " +
                    "instead of trusting a single guess."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SelectedSearchResult)),
                    "Search results"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SelectedSearchResult)),
                    "Cities matching your search, best match first, with their region, country and coordinates. " +
                    "Picking one applies it straight away."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SelectedFavourite)),
                    "Recent cities"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SelectedFavourite)),
                    "Cities you have used before. Picking one switches to it immediately, with no lookup."
                },

                // -- Actions ------------------------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ApplyCity)),
                    "Apply City"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ApplyCity)),
                    "Use the best match for the name typed above, without picking from the list. " +
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
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ApplyImmediately)),
                    "Apply Immediately"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ApplyImmediately)),
                    "Refresh and jump straight to the new weather, skipping the fade. Also cuts short a " +
                    "transition that is already running."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Reset to Game Weather"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Release every climate override and let the game control the weather again. " +
                    "Real weather resumes when you apply a city or force a refresh."
                },
                {
                    _settings.GetOptionWarningLocaleID(nameof(RealWeatherSettings.ResetToGameWeather)),
                    "Release all weather overrides and return control to the game?"
                },

                // -- Status -------------------------------------------------
                { _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.StatusText)), "Status" },
                { _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ResolvedLocationText)), "Resolved location" },
                { _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.LastUpdateText)), "Last update" },
                { _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.CurrentWeatherText)), "Current weather" },

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
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SyncTemperature)),
                    "Synchronise temperature"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.SyncTemperature)),
                    "Drive the visual temperature from the real city. Temperature is the value the largest " +
                    "number of game systems read back - heating demand, upkeep, fire risk, tourism, snow on the " +
                    "ground - so turning this off is the single most effective way to keep the mod's influence " +
                    "to a minimum. The cost is that the game can no longer tell rain from snow, and the " +
                    "temperature you see is the game's own."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.SimulationImpactNote)),
                    "What the game reads back"
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

                // -- Options nobody asked for -------------------------------
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.TimeShiftHours)),
                    "Time shift (hours)"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.TimeShiftHours)),
                    "Read the weather from a different hour: negative for the past, positive for the forecast. " +
                    "At -24 your city lives yesterday's weather; at +24 it gets tomorrow's, a day early. " +
                    "Only the weather reading moves - the game clock, date and season are untouched."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.AntipodeMode)),
                    "Antipode mode"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.AntipodeMode)),
                    "Take the weather from the point on the exact opposite side of the planet from your chosen " +
                    "city. For most of Europe that is the middle of the South Pacific, so expect a lot of grey " +
                    "ocean drizzle. Your city stays the one you picked - only the weather comes from the far side."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.ExtremeLocation)),
                    "Take me somewhere awful"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.ExtremeLocation)),
                    "Jump straight to a famously miserable place. Also the fastest way to see snow, fog or a " +
                    "downpour without waiting for the weather at home to oblige. Resets to None when you restart, " +
                    "but the city it picked is kept like any other."
                },
                {
                    _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.OppositeDay)),
                    "Opposite day"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(RealWeatherSettings.OppositeDay)),
                    "Mirror the weather. Warm becomes cold, clear becomes overcast, dry becomes soaking. " +
                    "Fog is left alone, because permanent fog hides the city and stops being funny immediately."
                },

                // -- About --------------------------------------------------
                { _settings.GetOptionLabelLocaleID(nameof(RealWeatherSettings.AboutText)), "About" },
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
                { LocaleKeys.StatusCandidatesReady, "Pick a city from the search results" },
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
                { LocaleKeys.WeatherConditions, "Conditions" },
                { LocaleKeys.WeatherTimeShiftPast, "{0} h in the past" },
                { LocaleKeys.WeatherTimeShiftFuture, "{0} h ahead - forecast" },
                { LocaleKeys.WeatherOppositeDay, "Opposite day" },
                { LocaleKeys.WeatherAntipode, "Antipode" },

                {
                    LocaleKeys.SimulationImpactNote,
                    "Real Weather Sync writes the same four climate values the game's own developer weather " +
                    "tools write, and parts of the game read those values back. Heating and cooling demand, " +
                    "building upkeep, fire risk, leisure, tourism, snow on the ground, surface wetness and " +
                    "weather events all respond to temperature and precipitation - exactly as they respond to " +
                    "the game's own weather." + Environment.NewLine +
                    "The mod adds no systems, changes no rules, and writes nothing into your save." +
                    Environment.NewLine +
                    "Turning off \"Synchronise temperature\" removes the largest part of this, at the cost of " +
                    "rain-versus-snow accuracy. Solar output and groundwater are never affected, and fog " +
                    "affects nothing outside the visuals."
                },

                { LocaleKeys.SearchNoResults, "No results - press Search" },
                { LocaleKeys.SearchPickOne, "Select a city..." },
                { LocaleKeys.FavouritesEmpty, "No recent cities yet" },

                { LocaleKeys.OverridesActive, "Climate overrides are active." },
                { LocaleKeys.OverridesInactive, "Climate overrides are not active." },

                { LocaleKeys.ErrorCityNotFound, "No matching city found" },
                { LocaleKeys.ErrorEmptyCity, "Enter a city name first" },
                { LocaleKeys.ErrorNetwork, "Could not reach Open-Meteo" },
                { LocaleKeys.ErrorRateLimited, "Open-Meteo is rate limiting requests" }
            };

            // WMO condition names, from the single table in WeatherCodes.
            foreach (var pair in WeatherCodes.All())
            {
                entries[pair.Key] = pair.Value;
            }

            // Extreme-location presets, from the single table in ExtremeLocations.
            foreach (ExtremeLocationOption option in Enum.GetValues(typeof(ExtremeLocationOption)))
            {
                entries[_settings.GetEnumValueLocaleID(option)] = ExtremeLocations.DescribeEnglish(option);
            }

            return entries;
        }

        public void Unload()
        {
        }
    }
}
