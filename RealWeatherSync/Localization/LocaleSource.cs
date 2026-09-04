using System;
using System.Collections.Generic;
using Colossal;
using RealWeatherSync.Localization.Strings;
using RealWeatherSync.Mapping;
using RealWeatherSync.Settings;

namespace RealWeatherSync.Localization
{
    /// <summary>
    /// Registers one <see cref="LocaleTable"/> with the game, translating the table's
    /// symbolic slot names into the locale ids the game actually uses.
    ///
    /// This is the only game-aware part of the localisation: option ids can be produced
    /// only by <c>ModSetting.GetOption*LocaleID</c>, which needs the live settings object.
    /// The tables themselves stay free of game types so the offline test suite can check
    /// every language against en-US.
    /// </summary>
    public class LocaleSource : IDictionarySource
    {
        private readonly RealWeatherSettings _settings;
        private readonly LocaleTable _table;

        public LocaleSource(RealWeatherSettings settings, LocaleTable table)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            _settings = settings;
            _table = table;
        }

        /// <summary>The game locale id this source is registered under.</summary>
        public string LocaleId
        {
            get { return _table.LocaleId; }
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>(_table.Count);

            foreach (var pair in _table.Entries)
            {
                var id = ResolveId(pair.Key);
                if (id == null)
                {
                    // A slot the resolver does not understand would otherwise vanish silently.
                    Mod.Log.Warn("Unknown locale slot \"" + pair.Key + "\" in " + _table.LocaleId + "; skipped.");
                    continue;
                }

                entries[id] = Expand(pair.Value);
            }

            return entries;
        }

        public void Unload()
        {
        }

        /// <summary>
        /// Turns a slot name into the locale id the game expects, or null when the slot
        /// uses a prefix this resolver does not know.
        /// </summary>
        private string ResolveId(string slot)
        {
            if (string.IsNullOrEmpty(slot))
            {
                return null;
            }

            var dot = slot.IndexOf('.');
            if (dot <= 0 || dot == slot.Length - 1)
            {
                return null;
            }

            var prefix = slot.Substring(0, dot);
            var rest = slot.Substring(dot + 1);

            try
            {
                switch (prefix)
                {
                    case "mod":
                        return rest == "name" ? _settings.GetSettingsLocaleID() : null;
                    case "tab":
                        return _settings.GetOptionTabLocaleID(rest);
                    case "group":
                        return _settings.GetOptionGroupLocaleID(rest);
                    case "label":
                        return _settings.GetOptionLabelLocaleID(rest);
                    case "desc":
                        return _settings.GetOptionDescLocaleID(rest);
                    case "warn":
                        return _settings.GetOptionWarningLocaleID(rest);
                    case "enum":
                        return ResolveEnumId(rest);
                    case "key":
                        return LocaleKeys.Prefix + rest;
                    case "wmo":
                        return WeatherCodes.Prefix + rest;
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                Mod.Log.Warn("Could not resolve locale slot \"" + slot + "\": " + e.Message);
                return null;
            }
        }

        /// <summary>Resolves "UpdateInterval.SixtyMinutes" and "ExtremeLocation.Ushuaia".</summary>
        private string ResolveEnumId(string rest)
        {
            var dot = rest.IndexOf('.');
            if (dot <= 0 || dot == rest.Length - 1)
            {
                return null;
            }

            var enumName = rest.Substring(0, dot);
            var valueName = rest.Substring(dot + 1);

            switch (enumName)
            {
                case "UpdateInterval":
                    return _settings.GetEnumValueLocaleID(
                        (UpdateIntervalOption)Enum.Parse(typeof(UpdateIntervalOption), valueName));
                case "ExtremeLocation":
                    return _settings.GetEnumValueLocaleID(
                        (ExtremeLocationOption)Enum.Parse(typeof(ExtremeLocationOption), valueName));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Expands the two tokens the tables are allowed to use. Anything else, including
        /// the <c>{0}</c> placeholders consumed by string.Format call sites, is left alone.
        /// </summary>
        private static string Expand(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.IndexOf("{VERSION}", StringComparison.Ordinal) >= 0)
            {
                text = text.Replace("{VERSION}", Mod.Version);
            }

            if (Environment.NewLine != "\n" && text.IndexOf('\n') >= 0)
            {
                text = text.Replace("\n", Environment.NewLine);
            }

            return text;
        }
    }
}
