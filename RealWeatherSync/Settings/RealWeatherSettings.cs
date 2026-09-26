using System;
using System.Collections.Generic;
using System.Globalization;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game.Modding;
using Game.Settings;
using Game.UI.Localization;
using Game.UI.Widgets;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Localization;
using RealWeatherSync.Models;
using RealWeatherSync.Services;

namespace RealWeatherSync.Settings
{
    /// <summary>
    /// Options page for Real Weather Sync.
    ///
    /// The persisted state is deliberately small: the city the player typed, the
    /// coordinates that were resolved from it, a short most-recently-used list, and a
    /// handful of preferences. Nothing here ever ends up in a city save.
    /// </summary>
    [FileLocation("ModsSettings/RealWeatherSync/RealWeatherSync")]
    [SettingsUIGroupOrder(GeneralGroup, SearchGroup, ActionsGroup, StatusGroup, AdvancedGroup, SillyGroup, AboutGroup)]
    [SettingsUIShowGroupName(GeneralGroup, SearchGroup, ActionsGroup, StatusGroup, AdvancedGroup, SillyGroup, AboutGroup)]
    public class RealWeatherSettings : ModSetting
    {
        public const string MainSection = "Main";

        public const string GeneralGroup = "GeneralGroup";
        public const string SearchGroup = "SearchGroup";
        public const string ActionsGroup = "ActionsGroup";
        public const string StatusGroup = "StatusGroup";
        public const string AdvancedGroup = "AdvancedGroup";
        public const string SillyGroup = "SillyGroup";
        public const string AboutGroup = "AboutGroup";

        /// <summary>
        /// Buttons sharing a group name are laid out on one row. Without the attribute the game
        /// gives each button a row of its own, which is what left the four actions in a column.
        /// </summary>
        private const string ApplyButtonRow = "ApplyButtonRow";
        private const string ResetButtonRow = "ResetButtonRow";

        public const int MinTransitionSeconds = 0;
        public const int MaxTransitionSeconds = 600;
        public const int DefaultTransitionSeconds = 120;

        /// <summary>
        /// Sentinel used by the dropdowns when nothing is selected. It must not be empty: the
        /// game's dropdown treats an empty string as a missing value and renders a blank field
        /// instead of the matching item's display name.
        /// </summary>
        private const string NoSelection = "-1";

        private bool _enableRealWeather = true;
        private string _cityQuery = string.Empty;
        private bool _smoothTransitions = true;
        private int _transitionSeconds = DefaultTransitionSeconds;
        private UpdateIntervalOption _updateInterval = UpdateIntervalOption.FifteenMinutes;
        private bool _syncFog = true;
        private bool _syncTemperature = true;
        private bool _syncAurora;
        private bool _syncSunPosition;
        private bool _forceSnowAppearance = true;
        private bool _ignoreModConflicts;
        private bool _limitWeatherEvents;
        private bool _oppositeDay;
        private int _timeShiftHours;
        private bool _followGameClock;
        private bool _antipodeMode;
        private ExtremeLocationOption _extremeLocation = ExtremeLocationOption.None;
        private string _selectedCandidate = NoSelection;
        private string _selectedFavourite = NoSelection;

        public RealWeatherSettings(IMod mod)
            : base(mod)
        {
        }

        // ------------------------------------------------------------------
        // General
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, GeneralGroup)]
        public bool EnableRealWeather
        {
            get { return _enableRealWeather; }
            set
            {
                if (_enableRealWeather == value)
                {
                    return;
                }

                _enableRealWeather = value;
                Mod.OnEnabledChanged(value);
            }
        }

        /// <summary>
        /// Opt-in. Instead of one frozen reading, walk the last 24 hours of real weather using
        /// the in-game hour: at 15:00 in game you get the city's real weather from the most
        /// recent 15:00. The in-game clock is only *read* - never set.
        /// </summary>
        [SettingsUISection(MainSection, GeneralGroup)]
        public bool FollowGameClock
        {
            get { return _followGameClock; }
            set
            {
                if (_followGameClock == value)
                {
                    return;
                }

                _followGameClock = value;
                Mod.OnFollowGameClockChanged(value);
            }
        }

        [SettingsUISection(MainSection, GeneralGroup)]
        public bool SmoothTransitions
        {
            get { return _smoothTransitions; }
            set { _smoothTransitions = value; }
        }

        [SettingsUISection(MainSection, GeneralGroup)]
        [SettingsUISlider(min = MinTransitionSeconds, max = MaxTransitionSeconds, step = 10, unit = "integer", scalarMultiplier = 1)]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsTransitionLengthUnavailable))]
        public int TransitionSeconds
        {
            get { return _transitionSeconds; }
            set { _transitionSeconds = value < MinTransitionSeconds ? MinTransitionSeconds : (value > MaxTransitionSeconds ? MaxTransitionSeconds : value); }
        }

        [SettingsUISection(MainSection, GeneralGroup)]
        public UpdateIntervalOption UpdateInterval
        {
            get { return _updateInterval; }
            set
            {
                if (_updateInterval == value)
                {
                    return;
                }

                _updateInterval = value;
                Mod.OnUpdateIntervalChanged();
            }
        }

        /// <summary>The transition length actually used, honouring the smoothing toggle.</summary>
        public float EffectiveTransitionSeconds
        {
            get { return _smoothTransitions ? _transitionSeconds : 0f; }
        }

        public bool IsTransitionLengthUnavailable()
        {
            // Following the in-game clock produces a continuously interpolated value, so the
            // fade machinery is bypassed and its length means nothing.
            return !_smoothTransitions || _followGameClock;
        }

        /// <summary>
        /// The manual time shift and the clock-following mode both decide "which hour", so
        /// letting them stack would just be confusing. Clock-following wins.
        /// </summary>
        public bool IsTimeShiftUnavailable()
        {
            return _followGameClock;
        }

        // ------------------------------------------------------------------
        // City search
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, SearchGroup)]
        [SettingsUITextInput]
        public string CityQuery
        {
            get { return _cityQuery; }
            set { _cityQuery = value ?? string.Empty; }
        }

        [SettingsUISection(MainSection, SearchGroup)]
        [SettingsUIButton]
        public bool SearchCity
        {
            set { Mod.OnSearchCityPressed(); }
        }

        /// <summary>
        /// The candidates returned by the last search. Backed by a dropdown whose item list is
        /// refreshed through <see cref="GetSearchResultsVersion"/>, so results appear without
        /// reopening the options page.
        /// </summary>
        [SettingsUISection(MainSection, SearchGroup)]
        [SettingsUIDropdown(typeof(RealWeatherSettings), nameof(GetSearchResultItems))]
        [SettingsUIValueVersion(typeof(RealWeatherSettings), nameof(GetSearchResultsVersion))]
        [Exclude]
        public string SelectedSearchResult
        {
            get { return _selectedCandidate; }
            set
            {
                _selectedCandidate = value ?? NoSelection;
                Mod.OnSearchResultSelected(_selectedCandidate);
            }
        }

        public DropdownItem<string>[] GetSearchResultItems()
        {
            var coordinator = Mod.Coordinator;
            var candidates = coordinator != null ? coordinator.Candidates : null;

            if (candidates == null || candidates.Count == 0)
            {
                return new[]
                {
                    new DropdownItem<string>
                    {
                        value = NoSelection,
                        displayName = Translation.Get(LocaleKeys.SearchNoResults, "No results - press Search")
                    }
                };
            }

            var items = new DropdownItem<string>[candidates.Count + 1];
            items[0] = new DropdownItem<string>
            {
                value = NoSelection,
                displayName = Translation.Get(LocaleKeys.SearchPickOne, "Select a city...")
            };

            for (var i = 0; i < candidates.Count; i++)
            {
                items[i + 1] = new DropdownItem<string>
                {
                    value = i.ToString(CultureInfo.InvariantCulture),
                    displayName = Describe(candidates[i])
                };
            }

            return items;
        }

        public int GetSearchResultsVersion()
        {
            var coordinator = Mod.Coordinator;
            return coordinator != null ? coordinator.CandidatesVersion : 0;
        }

        // ------------------------------------------------------------------
        // Favourites / recent
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, SearchGroup)]
        [SettingsUIDropdown(typeof(RealWeatherSettings), nameof(GetFavouriteItems))]
        [SettingsUIValueVersion(typeof(RealWeatherSettings), nameof(GetFavouritesVersion))]
        [Exclude]
        public string SelectedFavourite
        {
            get { return _selectedFavourite; }
            set
            {
                _selectedFavourite = value ?? NoSelection;
                Mod.OnFavouriteSelected(_selectedFavourite);
            }
        }

        public DropdownItem<string>[] GetFavouriteItems()
        {
            var favourites = Favourites;

            if (favourites.Count == 0)
            {
                return new[]
                {
                    new DropdownItem<string>
                    {
                        value = NoSelection,
                        displayName = Translation.Get(LocaleKeys.FavouritesEmpty, "No recent cities yet")
                    }
                };
            }

            var items = new DropdownItem<string>[favourites.Count + 1];
            items[0] = new DropdownItem<string>
            {
                value = NoSelection,
                displayName = Translation.Get(LocaleKeys.SearchPickOne, "Select a city...")
            };

            for (var i = 0; i < favourites.Count; i++)
            {
                items[i + 1] = new DropdownItem<string>
                {
                    value = i.ToString(CultureInfo.InvariantCulture),
                    displayName = Describe(favourites[i])
                };
            }

            return items;
        }

        public int GetFavouritesVersion()
        {
            return _favouritesVersion;
        }

        private int _favouritesVersion;

        /// <summary>Most-recently-used cities, newest first.</summary>
        [Exclude]
        public List<LocationResult> Favourites
        {
            get { return FavouriteCities.Parse(FavouritesRaw); }
        }

        /// <summary>Moves a location to the front of the recent list and persists it.</summary>
        public void RememberCity(LocationResult location)
        {
            if (location == null || !location.HasValidCoordinates)
            {
                return;
            }

            FavouritesRaw = FavouriteCities.Serialise(FavouriteCities.Promote(Favourites, location));
            _favouritesVersion++;
        }

        private static string Describe(LocationResult location)
        {
            if (location == null)
            {
                return string.Empty;
            }

            var ci = CultureInfo.InvariantCulture;
            return location.DisplayName + "  ·  " +
                   location.Latitude.ToString("0.##", ci) + ", " +
                   location.Longitude.ToString("0.##", ci);
        }

        // ------------------------------------------------------------------
        // Actions
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(ApplyButtonRow)]
        public bool ApplyCity
        {
            set { Mod.OnApplyCityPressed(); }
        }

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(ApplyButtonRow)]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsRefreshUnavailable))]
        public bool RefreshWeatherNow
        {
            set { Mod.OnRefreshNowPressed(); }
        }

        /// <summary>Refreshes and skips straight to the new values, ignoring the transition.</summary>
        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(ResetButtonRow)]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsRefreshUnavailable))]
        public bool ApplyImmediately
        {
            set { Mod.OnApplyImmediatelyPressed(); }
        }

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(ResetButtonRow)]
        [SettingsUIConfirmation]
        public bool ResetToGameWeather
        {
            set { Mod.OnResetToGameWeatherPressed(); }
        }

        public bool IsRefreshUnavailable()
        {
            return !HasResolvedLocation;
        }

        /// <summary>Forcing snow works by nudging the visual temperature, so it needs that override.</summary>
        public bool IsSnowForcingUnavailable()
        {
            return !_syncTemperature;
        }

        // ------------------------------------------------------------------
        // Status (read-only display)
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, StatusGroup)]
        [Exclude]
        public string StatusText
        {
            get { return StatusReport.DescribeStatus(); }
        }

        [SettingsUISection(MainSection, StatusGroup)]
        [Exclude]
        public string ResolvedLocationText
        {
            get { return StatusReport.DescribeLocation(); }
        }

        [SettingsUISection(MainSection, StatusGroup)]
        [Exclude]
        public string LastUpdateText
        {
            get { return StatusReport.DescribeLastUpdate(); }
        }

        /// <summary>
        /// The live weather description.
        ///
        /// <c>Game.UI.Widgets.MultilineText</c> is a <c>NamedWidget</c> with an icon: it carries a
        /// display name and no value, so a multiline property's own string never reaches the UI.
        /// Until 1.5.0 this row therefore showed the words "Current weather" and nothing else. The
        /// text has to arrive through the display-name action instead, which the widget re-reads,
        /// and which is also what makes it update as the weather does.
        /// </summary>
        [SettingsUISection(MainSection, StatusGroup)]
        [SettingsUIMultilineText]
        [SettingsUIDisplayName(typeof(RealWeatherSettings), nameof(GetCurrentWeatherDisplay))]
        [Exclude]
        public string CurrentWeatherText
        {
            get { return StatusReport.DescribeWeather(); }
        }

        public static LocalizedString GetCurrentWeatherDisplay()
        {
            return LocalizedString.Value(StatusReport.DescribeWeather());
        }

        // ------------------------------------------------------------------
        // Advanced
        //
        // [SettingsUIAdvanced] is the game's own mechanism, not ours: OptionsUISystem drops these
        // properties from the page entirely while the player's "Show Advanced" toggle is off, and
        // the options screen only offers that toggle on a page that declares at least one such
        // property. The label comes from the game's strings, so it needs no entry in the twelve
        // locale tables. The toggle's state lives in OptionsUISystem for the session; it is not
        // persisted, so every launch starts with these hidden.
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool SyncFog
        {
            get { return _syncFog; }
            set
            {
                if (_syncFog == value)
                {
                    return;
                }

                _syncFog = value;
                Mod.OnMappingOptionsChanged();
            }
        }

        /// <summary>
        /// Temperature is the value the largest number of game systems read back, so it gets its
        /// own opt-out for players who want to keep the mod's influence to a minimum.
        /// </summary>
        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool SyncTemperature
        {
            get { return _syncTemperature; }
            set
            {
                if (_syncTemperature == value)
                {
                    return;
                }

                _syncTemperature = value;
                Mod.OnMappingOptionsChanged();
            }
        }

        /// <summary>Opt-in NOAA OVATION aurora forecast. Off for existing installations.</summary>
        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool SyncAurora
        {
            get { return _syncAurora; }
            set { _syncAurora = value; }
        }

        /// <summary>
        /// Opt-in visual override. The rendering system uses the selected location and current
        /// UTC time; it never assigns PlanetarySystem time, date, latitude or longitude.
        /// </summary>
        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool SyncSunPosition
        {
            get { return _syncSunPosition; }
            set { _syncSunPosition = value; }
        }

        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsSnowForcingUnavailable))]
        public bool ForceSnowAppearance
        {
            get { return _forceSnowAppearance; }
            set
            {
                if (_forceSnowAppearance == value)
                {
                    return;
                }

                _forceSnowAppearance = value;
                Mod.OnMappingOptionsChanged();
            }
        }

        /// <summary>
        /// Withholds the game's weather event generator unless the real city is actually having a
        /// thunderstorm.
        ///
        /// Off by default because it changes what the game does rather than how it looks, which
        /// is a different promise from the rest of the mod. What it does NOT do is create events:
        /// see <see cref="Systems.WeatherHazardGate"/> for why that line is not crossed.
        /// </summary>
        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool LimitWeatherEvents
        {
            get { return _limitWeatherEvents; }
            set { _limitWeatherEvents = value; }
        }

        [SettingsUISection(MainSection, AdvancedGroup)]
        [SettingsUIAdvanced]
        public bool IgnoreModConflicts
        {
            get { return _ignoreModConflicts; }
            set
            {
                if (_ignoreModConflicts == value)
                {
                    return;
                }

                _ignoreModConflicts = value;
                Mod.OnMappingOptionsChanged();
            }
        }

        // ------------------------------------------------------------------
        // "Options nobody asked for"
        // ------------------------------------------------------------------

        /// <summary>
        /// Reads the weather from a past or future hour instead of now. Only the weather reading
        /// moves - the game clock, date and season are untouched, as always.
        /// </summary>
        [SettingsUISection(MainSection, SillyGroup)]
        [SettingsUIAdvanced]
        [SettingsUISlider(min = -OpenMeteoClient.MaxTimeShiftHours, max = OpenMeteoClient.MaxTimeShiftHours, step = 1, unit = "integer", scalarMultiplier = 1)]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsTimeShiftUnavailable))]
        public int TimeShiftHours
        {
            get { return _timeShiftHours; }
            set
            {
                if (_timeShiftHours == value)
                {
                    return;
                }

                _timeShiftHours = value;
                Mod.OnTimeShiftChanged(value);
            }
        }

        /// <summary>
        /// Fetch the weather for the point diametrically opposite the chosen city. Usually the
        /// middle of an ocean, which is exactly the point.
        /// </summary>
        [SettingsUISection(MainSection, SillyGroup)]
        [SettingsUIAdvanced]
        public bool AntipodeMode
        {
            get { return _antipodeMode; }
            set
            {
                if (_antipodeMode == value)
                {
                    return;
                }

                _antipodeMode = value;
                Mod.OnAntipodeModeChanged(value);
            }
        }

        /// <summary>
        /// One-click jumps to famously miserable places. Not persisted: it is a "take me there"
        /// action, not a stored preference, so it resets to None on restart while the location it
        /// applied is kept like any other city.
        /// </summary>
        [SettingsUISection(MainSection, SillyGroup)]
        [SettingsUIAdvanced]
        [Exclude]
        public ExtremeLocationOption ExtremeLocation
        {
            get { return _extremeLocation; }
            set
            {
                if (_extremeLocation == value)
                {
                    return;
                }

                _extremeLocation = value;
                Mod.OnExtremeLocationSelected(value);
            }
        }

        [SettingsUISection(MainSection, SillyGroup)]
        [SettingsUIAdvanced]
        public bool OppositeDay
        {
            get { return _oppositeDay; }
            set
            {
                if (_oppositeDay == value)
                {
                    return;
                }

                _oppositeDay = value;
                Mod.OnMappingOptionsChanged();
            }
        }

        // ------------------------------------------------------------------
        // About
        // ------------------------------------------------------------------

        /// <summary>
        /// Shown through a display-name action for the same reason as
        /// <see cref="CurrentWeatherText"/>: the multiline widget draws its name, never a value.
        /// </summary>
        [SettingsUISection(MainSection, AboutGroup)]
        [SettingsUIMultilineText]
        [SettingsUIDisplayName(typeof(RealWeatherSettings), nameof(GetAboutDisplay))]
        [Exclude]
        public string AboutText
        {
            get
            {
                return Translation.Get(LocaleKeys.AboutText,
                    "Real Weather Sync " + Mod.Version + Environment.NewLine +
                    "Weather data by Open-Meteo (open-meteo.com), CC BY 4.0. No account or API key required." +
                    Environment.NewLine +
                    "Only the city name and the coordinates resolved from it are sent to Open-Meteo." +
                    Environment.NewLine + "Optional aurora sync downloads NOAA's global forecast without sending your location.");
            }
        }

        public static LocalizedString GetAboutDisplay()
        {
            var settings = Mod.Settings;
            return LocalizedString.Value(settings != null ? settings.AboutText : string.Empty);
        }

        // ------------------------------------------------------------------
        // Persisted resolution result (hidden from the UI)
        // ------------------------------------------------------------------

        [SettingsUIHidden]
        public bool HasResolvedLocation { get; set; }

        [SettingsUIHidden]
        public string ResolvedName { get; set; } = string.Empty;

        [SettingsUIHidden]
        public string ResolvedAdmin1 { get; set; } = string.Empty;

        [SettingsUIHidden]
        public string ResolvedCountry { get; set; } = string.Empty;

        [SettingsUIHidden]
        public string ResolvedCountryCode { get; set; } = string.Empty;

        [SettingsUIHidden]
        public string ResolvedTimezone { get; set; } = string.Empty;

        [SettingsUIHidden]
        public string ResolvedQuery { get; set; } = string.Empty;

        /// <summary>
        /// Stored as float: roughly two metres of precision, which is irrelevant for
        /// a weather lookup and keeps the settings file compatible with the value
        /// types the game's settings serialiser handles everywhere else.
        /// </summary>
        [SettingsUIHidden]
        public float ResolvedLatitude { get; set; }

        [SettingsUIHidden]
        public float ResolvedLongitude { get; set; }

        /// <summary>Recent cities, flat-encoded. See <see cref="FavouriteCities"/>.</summary>
        [SettingsUIHidden]
        public string FavouritesRaw { get; set; } = string.Empty;

        /// <summary>Refresh interval expressed in seconds.</summary>
        public double UpdateIntervalSeconds
        {
            get { return (int)_updateInterval * 60.0; }
        }

        /// <summary>Rebuilds the persisted location, or null when none was stored.</summary>
        public LocationResult BuildStoredLocation()
        {
            if (!HasResolvedLocation)
            {
                return null;
            }

            var location = new LocationResult(
                ResolvedQuery,
                ResolvedName,
                ResolvedAdmin1,
                ResolvedCountry,
                ResolvedCountryCode,
                ResolvedTimezone,
                ResolvedLatitude,
                ResolvedLongitude);

            return location.HasValidCoordinates ? location : null;
        }

        /// <summary>Persists a freshly resolved location. Called on the main thread.</summary>
        public void StoreLocation(LocationResult location)
        {
            if (location == null || !location.HasValidCoordinates)
            {
                return;
            }

            ResolvedQuery = location.Query;
            ResolvedName = location.Name;
            ResolvedAdmin1 = location.Admin1;
            ResolvedCountry = location.Country;
            ResolvedCountryCode = location.CountryCode;
            ResolvedTimezone = location.Timezone;
            ResolvedLatitude = (float)location.Latitude;
            ResolvedLongitude = (float)location.Longitude;
            HasResolvedLocation = true;

            RememberCity(location);
            ApplyAndSave();
        }

        public override void SetDefaults()
        {
            _enableRealWeather = true;
            _cityQuery = string.Empty;
            _smoothTransitions = true;
            _transitionSeconds = DefaultTransitionSeconds;
            _updateInterval = UpdateIntervalOption.FifteenMinutes;
            _syncFog = true;
            _syncTemperature = true;
            _syncAurora = false;
            _syncSunPosition = false;
            _forceSnowAppearance = true;
            _ignoreModConflicts = false;
            _limitWeatherEvents = false;
            _oppositeDay = false;
            _timeShiftHours = 0;
            _followGameClock = false;
            _antipodeMode = false;
            _extremeLocation = ExtremeLocationOption.None;
            _selectedCandidate = NoSelection;
            _selectedFavourite = NoSelection;

            HasResolvedLocation = false;
            ResolvedName = string.Empty;
            ResolvedAdmin1 = string.Empty;
            ResolvedCountry = string.Empty;
            ResolvedCountryCode = string.Empty;
            ResolvedTimezone = string.Empty;
            ResolvedQuery = string.Empty;
            ResolvedLatitude = 0f;
            ResolvedLongitude = 0f;
            FavouritesRaw = string.Empty;
        }
    }
}
