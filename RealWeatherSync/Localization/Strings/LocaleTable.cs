using System;
using System.Collections.Generic;

namespace RealWeatherSync.Localization.Strings
{
    /// <summary>
    /// One language's strings, as slot name to text.
    ///
    /// Deliberately free of game types. The slot names here are not locale ids: they are
    /// short symbolic names that <c>LocaleSource</c> turns into the real ids at runtime,
    /// because option ids can only be produced by the game's own
    /// <c>ModSetting.GetOption*LocaleID</c> methods. Keeping the text game-free is what
    /// lets the offline test suite check all twelve languages against each other.
    ///
    /// Slot naming, all resolved by <c>LocaleSource</c>:
    /// <list type="bullet">
    ///   <item><c>mod.name</c> - the settings page title.</item>
    ///   <item><c>tab.X</c> / <c>group.X</c> - option tab and group headers.</item>
    ///   <item><c>label.X</c> / <c>desc.X</c> / <c>warn.X</c> - option text, X being the property name.</item>
    ///   <item><c>enum.UpdateInterval.X</c> / <c>enum.ExtremeLocation.X</c> - dropdown values.</item>
    ///   <item><c>key.X</c> - a <see cref="LocaleKeys"/> id, X being the part after the prefix.</item>
    ///   <item><c>wmo.X</c> - a WMO condition name, X being the suffix used by <c>WeatherCodes</c>.</item>
    /// </list>
    ///
    /// Two tokens are expanded by <c>LocaleSource</c> when the entry is read:
    /// <c>\n</c> becomes <see cref="Environment.NewLine"/>, and <c>{VERSION}</c> becomes the
    /// mod version. Everything else, including <c>{0}</c>, is passed through untouched so
    /// <c>string.Format</c> call sites keep working.
    /// </summary>
    public sealed class LocaleTable
    {
        private readonly Dictionary<string, string> _entries;

        public LocaleTable(string localeId, string[,] table)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("A locale table needs a locale id.", "localeId");
            }

            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            LocaleId = localeId;
            _entries = new Dictionary<string, string>(table.GetLength(0), StringComparer.Ordinal);

            for (var i = 0; i < table.GetLength(0); i++)
            {
                // A duplicated slot would silently shadow one of the two texts, so it is an error.
                _entries[table[i, 0]] = table[i, 1];
            }
        }

        /// <summary>The game locale id this table is registered under, for example "de-DE".</summary>
        public string LocaleId { get; private set; }

        public int Count
        {
            get { return _entries.Count; }
        }

        public IEnumerable<KeyValuePair<string, string>> Entries
        {
            get { return _entries; }
        }

        public bool TryGet(string slot, out string text)
        {
            return _entries.TryGetValue(slot, out text);
        }
    }
}
