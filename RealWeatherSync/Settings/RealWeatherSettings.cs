using System;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game.Modding;
using Game.Settings;
using RealWeatherSync.Diagnostics;
using RealWeatherSync.Localization;
using RealWeatherSync.Models;

namespace RealWeatherSync.Settings
{
    /// <summary>
    /// Options page for Real Weather Sync.
    ///
    /// The persisted state is deliberately small: the city the player typed, the
    /// coordinates that were resolved from it, and a handful of preferences.
    /// Nothing here ever ends up in a city save.
    /// </summary>
    [FileLocation("ModsSettings/RealWeatherSync/RealWeatherSync")]
    [SettingsUIGroupOrder(GeneralGroup, ActionsGroup, StatusGroup, AdvancedGroup, AboutGroup)]
    [SettingsUIShowGroupName(GeneralGroup, ActionsGroup, StatusGroup, AdvancedGroup, AboutGroup)]
    public class RealWeatherSettings : ModSetting
    {
        public const string MainSection = "Main";

        public const string GeneralGroup = "GeneralGroup";
        public const string ActionsGroup = "ActionsGroup";
        public const string StatusGroup = "StatusGroup";
        public const string AdvancedGroup = "AdvancedGroup";
        public const string AboutGroup = "AboutGroup";

        /// <summary>Transition length in seconds when smoothing is enabled.</summary>
        public const float TransitionDurationSeconds = 120f;

        private bool _enableRealWeather = true;
        private string _cityQuery = string.Empty;
        private bool _smoothTransitions = true;
        private UpdateIntervalOption _updateInterval = UpdateIntervalOption.FifteenMinutes;
        private bool _syncFog = true;
        private bool _forceSnowAppearance = true;
        private bool _ignoreModConflicts;

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

        [SettingsUISection(MainSection, GeneralGroup)]
        [SettingsUITextInput]
        public string CityQuery
        {
            get { return _cityQuery; }
            set { _cityQuery = value ?? string.Empty; }
        }

        [SettingsUISection(MainSection, GeneralGroup)]
        public bool SmoothTransitions
        {
            get { return _smoothTransitions; }
            set { _smoothTransitions = value; }
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

        [SettingsUISection(MainSection, ActionsGroup)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetToGameWeather
        {
            set { Mod.OnResetToGameWeatherPressed(); }
        }

        /// <summary>Referenced by <see cref="SettingsUIDisableByConditionAttribute"/> above.</summary>
        public bool IsRefreshUnavailable()
        {
            return !HasResolvedLocation;
        }

        // ------------------------------------------------------------------
        // Status (read-only display; get-only string properties render as
        // read-only fields and refresh automatically while the page is open)
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

            ApplyAndSave();
        }

        public override void SetDefaults()
        {
            _enableRealWeather = true;
            _cityQuery = string.Empty;
            _smoothTransitions = true;
            _updateInterval = UpdateIntervalOption.FifteenMinutes;
            _syncFog = true;
            _forceSnowAppearance = true;
            _ignoreModConflicts = false;

            HasResolvedLocation = false;
            ResolvedName = string.Empty;
            ResolvedAdmin1 = string.Empty;
            ResolvedCountry = string.Empty;
            ResolvedCountryCode = string.Empty;
            ResolvedTimezone = string.Empty;
            ResolvedQuery = string.Empty;
            ResolvedLatitude = 0f;
            ResolvedLongitude = 0f;
        }
    }
}
