using System;
using System.Collections.Generic;
using System.Globalization;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game.Modding;
using Game.Settings;
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

        public const int MinTransitionSeconds = 0;
        public const int MaxTransitionSeconds = 600;
        public const int DefaultTransitionSeconds = 120;

        /// <summary>Sentinel used by the dropdowns when nothing is selected.</summary>
        private const string NoSelection = "";

        private bool _enableRealWeather = true;
        private string _cityQuery = string.Empty;
        private bool _smoothTransitions = true;
        private int _transitionSeconds = DefaultTransitionSeconds;
        private UpdateIntervalOption _updateInterval = UpdateIntervalOption.FifteenMinutes;
        private bool _syncFog = true;
        private bool _forceSnowAppearance = true;
        private bool _ignoreModConflicts;
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
        public bool ApplyCity
        {
            set { Mod.OnApplyCityPressed(); }
        }

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsRefreshUnavailable))]
        public bool RefreshWeatherNow
        {
            set { Mod.OnRefreshNowPressed(); }
        }

        /// <summary>Refreshes and skips straight to the new values, ignoring the transition.</summary>
        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(RealWeatherSettings), nameof(IsRefreshUnavailable))]
        public bool ApplyImmediately
        {
            set { Mod.OnApplyImmediatelyPressed(); }
        }

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetToGameWeather
        {
            set { Mod.OnResetToGameWeatherPressed(); }
        }

        public bool IsRefreshUnavailable()
        {
            return !HasResolvedLocation;
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

        [SettingsUISection(MainSection, StatusGroup)]
        [SettingsUIMultilineText]
        [Exclude]
        public string CurrentWeatherText
        {
            get { return StatusReport.DescribeWeather(); }
        }

        // ------------------------------------------------------------------
        // Advanced
        // ------------------------------------------------------------------

        [SettingsUISection(MainSection, AdvancedGroup)]
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

        [SettingsUISection(MainSection, AdvancedGroup)]
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

        [SettingsUISection(MainSection, AdvancedGroup)]
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

        [SettingsUISection(MainSection, AboutGroup)]
        [SettingsUIMultilineText]
        [Exclude]
        public string AboutText
        {
            get
            {
                return Translation.Get(LocaleKeys.AboutText,
                    "Real Weather Sync " + Mod.Version + Environment.NewLine +
                    "Weather data by Open-Meteo (open-meteo.com), CC BY 4.0. No account or API key required." +
                    Environment.NewLine +
                    "Only the city name and the coordinates resolved from it are sent to Open-Meteo.");
            }
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
            _forceSnowAppearance = true;
            _ignoreModConflicts = false;
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
