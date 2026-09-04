namespace RealWeatherSync.Localization.Strings
{
    /// <summary>
    /// en-US - the reference language.
    ///
    /// This table defines the slot set every other language must match; the offline test
    /// suite fails a translation that adds, drops or renames a slot. When adding a string,
    /// add it here first, then to all eleven other tables.
    /// </summary>
    public static class StringsEn
    {
        public const string LocaleId = "en-US";

        public static readonly string[,] Table =
        {
            { "mod.name", "Real Weather Sync" },

            { "tab.Main", "Main" },

            { "group.GeneralGroup", "General" },
            { "group.SearchGroup", "City" },
            { "group.ActionsGroup", "Actions" },
            { "group.StatusGroup", "Status" },
            { "group.AdvancedGroup", "Advanced" },
            { "group.SillyGroup", "Options nobody asked for" },
            { "group.AboutGroup", "About" },

            // -- General ------------------------------------------------------------
            { "label.EnableRealWeather", "Enable Real Weather" },
            { "desc.EnableRealWeather",
                "Match the visual weather of your city to the current real weather of the city below. " +
                "The clock, the date and the season are never changed - but the game does read the " +
                "weather values back, so see \"What the game reads back\" under Advanced. " +
                "Turning this off immediately hands the weather back to the game." },

            { "label.FollowGameClock", "Follow the in-game clock" },
            { "desc.FollowGameClock",
                "Instead of one frozen reading, walk through the city's last 24 hours of real weather " +
                "using the in-game hour. If it is 15:00 in your city, you get the real weather that the " +
                "chosen city had at its most recent 15:00 - so the weather changes as your day goes by. " +
                "The in-game clock is only read, never changed: the time, date and season stay exactly " +
                "as the game set them. Overrides the manual time shift while it is on." },

            { "label.SmoothTransitions", "Smooth Weather Transitions" },
            { "desc.SmoothTransitions",
                "Fade gradually to each new weather reading instead of switching instantly. " +
                "The fade uses real time, so it is unaffected by pausing or the simulation speed." },

            { "label.TransitionSeconds", "Transition length (seconds)" },
            { "desc.TransitionSeconds",
                "How long a fade between two weather readings takes, in real seconds. " +
                "Only used when smooth transitions are enabled." },

            { "label.UpdateInterval", "Update Interval" },
            { "desc.UpdateInterval",
                "How often to ask Open-Meteo for fresh conditions. Open-Meteo updates its data roughly " +
                "every 15 minutes, so shorter intervals gain little." },

            { "enum.UpdateInterval.FifteenMinutes", "15 minutes" },
            { "enum.UpdateInterval.ThirtyMinutes", "30 minutes" },
            { "enum.UpdateInterval.SixtyMinutes", "60 minutes" },

            // -- City ---------------------------------------------------------------
            { "label.CityQuery", "City" },
            { "desc.CityQuery",
                "The real city to copy the weather from. Examples: Lyon - Lyon, France - Milazzo, Italy - " +
                "New York, United States. Add a country or a region after a comma to narrow it down." },

            { "label.SearchCity", "Search" },
            { "desc.SearchCity",
                "Look the name up and list every matching city below, so you can confirm the right one " +
                "instead of trusting a single guess." },

            { "label.SelectedSearchResult", "Search results" },
            { "desc.SelectedSearchResult",
                "Cities matching your search, best match first, with their region, country and coordinates. " +
                "Picking one applies it straight away." },

            { "label.SelectedFavourite", "Recent cities" },
            { "desc.SelectedFavourite",
                "Cities you have used before. Picking one switches to it immediately, with no lookup." },

            // -- Actions ------------------------------------------------------------
            { "label.ApplyCity", "Apply City" },
            { "desc.ApplyCity",
                "Use the best match for the name typed above, without picking from the list. " +
                "If the city cannot be found, the previously resolved location is kept." },

            { "label.RefreshWeatherNow", "Refresh Weather Now" },
            { "desc.RefreshWeatherNow",
                "Fetch the current conditions immediately instead of waiting for the next update." },

            { "label.ApplyImmediately", "Apply Immediately" },
            { "desc.ApplyImmediately",
                "Refresh and jump straight to the new weather, skipping the fade. Also cuts short a " +
                "transition that is already running." },

            { "label.ResetToGameWeather", "Reset to Game Weather" },
            { "desc.ResetToGameWeather",
                "Release every climate override and let the game control the weather again. " +
                "Real weather resumes when you apply a city or force a refresh." },
            { "warn.ResetToGameWeather",
                "Release all weather overrides and return control to the game?" },

            // -- Status -------------------------------------------------------------
            { "label.StatusText", "Status" },
            { "label.ResolvedLocationText", "Resolved location" },
            { "label.LastUpdateText", "Last update" },
            { "label.CurrentWeatherText", "Current weather" },

            // -- Advanced -----------------------------------------------------------
            { "label.SyncFog", "Synchronise fog" },
            { "desc.SyncFog",
                "Derive fog from the reported fog codes and from visibility. Cloud cover never produces fog. " +
                "Turn this off to leave the game's own fog alone." },

            { "label.SyncTemperature", "Synchronise temperature" },
            { "desc.SyncTemperature",
                "Drive the visual temperature from the real city. Temperature is the value the largest " +
                "number of game systems read back - heating demand, upkeep, fire risk, tourism, snow on the " +
                "ground - so turning this off is the single most effective way to keep the mod's influence " +
                "to a minimum. The cost is that the game can no longer tell rain from snow, and the " +
                "temperature you see is the game's own." },

            { "label.SimulationImpactNote", "What the game reads back" },

            { "label.ForceSnowAppearance", "Show snow when it is really snowing" },
            { "desc.ForceSnowAppearance",
                "Cities: Skylines II decides between rain and snow from the visual temperature. When the real " +
                "weather is snow but the real temperature is above freezing, lower the visual temperature just " +
                "below freezing so snow is drawn. The displayed temperature will then differ from the real one. " +
                "The season and the date are still never changed." },

            { "label.IgnoreModConflicts", "Ignore mod conflicts" },
            { "desc.IgnoreModConflicts",
                "By default Real Weather Sync switches itself off when another known weather mod is loaded, " +
                "because two mods writing the same climate values will fight. Enable this only if you are sure " +
                "the other mod is not overriding the weather." },

            // -- Options nobody asked for -------------------------------------------
            { "label.TimeShiftHours", "Time shift (hours)" },
            { "desc.TimeShiftHours",
                "Read the weather from a different hour: negative for the past, positive for the forecast. " +
                "At -24 your city lives yesterday's weather; at +24 it gets tomorrow's, a day early. " +
                "Only the weather reading moves - the game clock, date and season are untouched." },

            { "label.AntipodeMode", "Antipode mode" },
            { "desc.AntipodeMode",
                "Take the weather from the point on the exact opposite side of the planet from your chosen " +
                "city. For most of Europe that is the middle of the South Pacific, so expect a lot of grey " +
                "ocean drizzle. Your city stays the one you picked - only the weather comes from the far side." },

            { "label.ExtremeLocation", "Take me somewhere awful" },
            { "desc.ExtremeLocation",
                "Jump straight to a famously miserable place. Also the fastest way to see snow, fog or a " +
                "downpour without waiting for the weather at home to oblige. Resets to None when you restart, " +
                "but the city it picked is kept like any other." },

            { "label.OppositeDay", "Opposite day" },
            { "desc.OppositeDay",
                "Mirror the weather. Warm becomes cold, clear becomes overcast, dry becomes soaking. " +
                "Fog is left alone, because permanent fog hides the city and stops being funny immediately." },

            { "enum.ExtremeLocation.None", "Stay where you are" },
            { "enum.ExtremeLocation.Yakutsk", "Yakutsk - the coldest city on Earth" },
            { "enum.ExtremeLocation.Longyearbyen", "Longyearbyen - polar night, polar day, polar bears" },
            { "enum.ExtremeLocation.Ushuaia", "Ushuaia - the end of the world, and it shows" },
            { "enum.ExtremeLocation.Reykjavik", "Reykjavik - wind and rain, sideways" },
            { "enum.ExtremeLocation.MountWashington", "Mount Washington - fog, and the worst weather in America" },
            { "enum.ExtremeLocation.DeathValley", "Death Valley - the hottest place ever recorded" },
            { "enum.ExtremeLocation.Cherrapunji", "Cherrapunji - one of the wettest places on Earth" },

            // -- About --------------------------------------------------------------
            { "label.AboutText", "About" },
            { "key.About.Text",
                "Real Weather Sync {VERSION}\n" +
                "Weather data by Open-Meteo (open-meteo.com), licensed CC BY 4.0. No account and no API key required.\n" +
                "Only the city name you type and the coordinates resolved from it are sent to Open-Meteo." },

            { "key.About.SimulationImpact",
                "Real Weather Sync writes the same four climate values the game's own developer weather " +
                "tools write, and parts of the game read those values back. Heating and cooling demand, " +
                "building upkeep, fire risk, leisure, tourism, snow on the ground, surface wetness and " +
                "weather events all respond to temperature and precipitation - exactly as they respond to " +
                "the game's own weather.\n" +
                "The mod adds no systems, changes no rules, and writes nothing into your save.\n" +
                "Turning off \"Synchronise temperature\" removes the largest part of this, at the cost of " +
                "rain-versus-snow accuracy. Solar output and groundwater are never affected, and fog " +
                "affects nothing outside the visuals." },

            // -- Runtime status strings ---------------------------------------------
            { "key.Status.Disabled", "Disabled" },
            { "key.Status.CityNotConfigured", "City not configured" },
            { "key.Status.ResolvingLocation", "Resolving location" },
            { "key.Status.Refreshing", "Refreshing weather" },
            { "key.Status.CandidatesReady", "Pick a city from the search results" },
            { "key.Status.Connected", "Connected" },
            { "key.Status.Offline", "Offline - using last valid weather" },
            { "key.Status.ErrorResolvingCity", "Error resolving city" },
            { "key.Status.IncompatibleMod", "Incompatible weather mod active" },
            { "key.Status.Released", "Overrides released - using game weather" },
            { "key.Status.WaitingForGame", "Waiting for a city to be loaded" },

            { "key.Location.NotResolved", "No location resolved yet" },

            { "key.LastUpdate.Never", "Never" },
            { "key.LastUpdate.JustNow", "just now" },
            { "key.LastUpdate.MinutesAgo", "{0} min ago" },

            { "key.Weather.NoData", "No weather data received yet." },
            { "key.Weather.Observed", "Observed" },
            { "key.Weather.Applied", "Applied" },
            { "key.Weather.Clouds", "clouds" },
            { "key.Weather.Precipitation", "precipitation" },
            { "key.Weather.Snow", "snow" },
            { "key.Weather.Code", "WMO code" },
            { "key.Weather.Visibility", "visibility" },
            { "key.Weather.Fog", "fog" },
            { "key.Weather.Conditions", "Conditions" },
            { "key.Weather.TimeShiftPast", "{0} h in the past" },
            { "key.Weather.TimeShiftFuture", "{0} h ahead - forecast" },
            { "key.Weather.OppositeDay", "Opposite day" },
            { "key.Weather.Antipode", "Antipode" },

            { "key.Search.NoResults", "No results - press Search" },
            { "key.Search.PickOne", "Select a city..." },
            { "key.Favourites.Empty", "No recent cities yet" },

            { "key.Overrides.Active", "Climate overrides are active." },
            { "key.Overrides.Inactive", "Climate overrides are not active." },

            { "key.Error.CityNotFound", "No matching city found" },
            { "key.Error.EmptyCity", "Enter a city name first" },
            { "key.Error.Network", "Could not reach Open-Meteo" },
            { "key.Error.RateLimited", "Open-Meteo is rate limiting requests" },

            // -- WMO condition names -------------------------------------------------
            { "wmo.Clear", "Clear sky" },
            { "wmo.MainlyClear", "Mainly clear" },
            { "wmo.PartlyCloudy", "Partly cloudy" },
            { "wmo.Overcast", "Overcast" },
            { "wmo.Fog", "Fog" },
            { "wmo.RimeFog", "Depositing rime fog" },
            { "wmo.DrizzleLight", "Light drizzle" },
            { "wmo.DrizzleModerate", "Moderate drizzle" },
            { "wmo.DrizzleDense", "Dense drizzle" },
            { "wmo.FreezingDrizzleLight", "Light freezing drizzle" },
            { "wmo.FreezingDrizzleDense", "Dense freezing drizzle" },
            { "wmo.RainSlight", "Slight rain" },
            { "wmo.RainModerate", "Moderate rain" },
            { "wmo.RainHeavy", "Heavy rain" },
            { "wmo.FreezingRainLight", "Light freezing rain" },
            { "wmo.FreezingRainHeavy", "Heavy freezing rain" },
            { "wmo.SnowSlight", "Slight snow" },
            { "wmo.SnowModerate", "Moderate snow" },
            { "wmo.SnowHeavy", "Heavy snow" },
            { "wmo.SnowGrains", "Snow grains" },
            { "wmo.ShowersSlight", "Slight rain showers" },
            { "wmo.ShowersModerate", "Moderate rain showers" },
            { "wmo.ShowersViolent", "Violent rain showers" },
            { "wmo.SnowShowersSlight", "Slight snow showers" },
            { "wmo.SnowShowersHeavy", "Heavy snow showers" },
            { "wmo.Thunderstorm", "Thunderstorm" },
            { "wmo.ThunderstormHailSlight", "Thunderstorm with slight hail" },
            { "wmo.ThunderstormHailHeavy", "Thunderstorm with heavy hail" },
            { "wmo.Unknown", "Unknown conditions" }
        };
    }
}
