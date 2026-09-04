using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RealWeatherSync.Localization.Strings;
using RealWeatherSync.Mapping;
using RealWeatherSync.Settings;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Checks the twelve translation tables against each other.
    ///
    /// A missing or misspelled slot cannot be caught by the compiler - the tables are plain
    /// string pairs - and in the game it would only show as a raw locale id in the options
    /// page, in a language the author probably does not read. So it is caught here instead.
    /// </summary>
    public static class LocalizationTests
    {
        /// <summary>The locale ids Cities: Skylines II itself ships, taken from its Locale.cok.</summary>
        private static readonly string[] GameLocales =
        {
            "en-US", "de-DE", "es-ES", "fr-FR", "it-IT", "ja-JP",
            "ko-KR", "pl-PL", "pt-BR", "ru-RU", "zh-HANS", "zh-HANT"
        };

        private static readonly string[] KnownPrefixes =
        {
            "mod", "tab", "group", "label", "desc", "warn", "enum", "key", "wmo"
        };

        public static void Run()
        {
            Assert.Section("Localisation - table structure");

            var tables = LocaleTables.All().ToList();

            Assert.Equal("twelve languages are registered", tables.Count, 12);

            var ids = tables.Select(t => t.LocaleId).ToList();
            Assert.True("no locale id is registered twice", ids.Distinct().Count() == ids.Count);

            foreach (var id in GameLocales)
            {
                Assert.True("the game locale " + id + " has a table", ids.Contains(id));
            }

            foreach (var id in ids)
            {
                Assert.True("locale " + id + " is one the game supports", GameLocales.Contains(id));
            }

            Assert.Section("Localisation - slots match en-US");

            var reference = LocaleTables.Reference();
            var referenceSlots = reference.Entries.Select(e => e.Key).ToList();

            Assert.True("the reference table is not empty", referenceSlots.Count > 0);
            Assert.True("every reference slot uses a known prefix",
                referenceSlots.All(s => KnownPrefixes.Contains(PrefixOf(s))));

            foreach (var table in tables)
            {
                if (table.LocaleId == reference.LocaleId)
                {
                    continue;
                }

                Assert.Equal(table.LocaleId + " has the same number of slots",
                    table.Count, reference.Count);

                var missing = referenceSlots.Where(slot =>
                {
                    string ignored;
                    return !table.TryGet(slot, out ignored);
                }).ToList();

                Assert.True(table.LocaleId + " is missing no slot" + Describe(missing),
                    missing.Count == 0);
            }

            Assert.Section("Localisation - text quality");

            foreach (var table in tables)
            {
                var blank = table.Entries.Where(e => string.IsNullOrEmpty(e.Value.Trim()))
                    .Select(e => e.Key).ToList();
                Assert.True(table.LocaleId + " has no empty text" + Describe(blank), blank.Count == 0);

                // A translation that drops or invents a {0} breaks string.Format at runtime.
                var mismatched = new List<string>();
                foreach (var pair in reference.Entries)
                {
                    string translated;
                    if (!table.TryGet(pair.Key, out translated))
                    {
                        continue;
                    }

                    if (Placeholders(translated) != Placeholders(pair.Value))
                    {
                        mismatched.Add(pair.Key);
                    }
                }

                Assert.True(table.LocaleId + " keeps every {0} placeholder" + Describe(mismatched),
                    mismatched.Count == 0);

                // Only en-US may still read as English for the long option descriptions;
                // an untranslated paragraph elsewhere is a copy-paste slip.
                if (table.LocaleId != reference.LocaleId)
                {
                    var untouched = reference.Entries
                        .Where(e => e.Key.StartsWith("desc.", StringComparison.Ordinal) &&
                                    e.Value.Length > 80)
                        .Where(e =>
                        {
                            string translated;
                            return table.TryGet(e.Key, out translated) && translated == e.Value;
                        })
                        .Select(e => e.Key)
                        .ToList();

                    Assert.True(table.LocaleId + " translated every long description" + Describe(untouched),
                        untouched.Count == 0);
                }
            }

            Assert.Section("Localisation - generated tables are covered");

            // WeatherCodes and ExtremeLocations own their own tables; a code added there
            // without a matching slot would silently show its locale id in every language.
            foreach (var pair in WeatherCodes.All())
            {
                var slot = "wmo." + pair.Key.Substring(WeatherCodes.Prefix.Length);
                string ignored;
                Assert.True("en-US covers WMO condition " + slot, reference.TryGet(slot, out ignored));
            }

            foreach (ExtremeLocationOption option in Enum.GetValues(typeof(ExtremeLocationOption)))
            {
                var slot = "enum.ExtremeLocation." + option;
                string ignored;
                Assert.True("en-US covers preset " + slot, reference.TryGet(slot, out ignored));
            }

            var wmoSlots = referenceSlots.Count(s => s.StartsWith("wmo.", StringComparison.Ordinal));
            Assert.Equal("no stale WMO slots remain", wmoSlots, WeatherCodes.All().Count());

            var presetSlots = referenceSlots.Count(s =>
                s.StartsWith("enum.ExtremeLocation.", StringComparison.Ordinal));
            Assert.Equal("no stale preset slots remain",
                presetSlots, Enum.GetValues(typeof(ExtremeLocationOption)).Length);
        }

        private static string PrefixOf(string slot)
        {
            var dot = slot.IndexOf('.');
            return dot <= 0 ? slot : slot.Substring(0, dot);
        }

        /// <summary>The set of {0}, {1}... placeholders in a string, order independent.</summary>
        private static string Placeholders(string text)
        {
            var found = Regex.Matches(text, @"\{\d+\}")
                .Cast<Match>()
                .Select(m => m.Value)
                .Distinct()
                .OrderBy(v => v, StringComparer.Ordinal);

            return string.Join(",", found);
        }

        private static string Describe(ICollection<string> slots)
        {
            if (slots.Count == 0)
            {
                return string.Empty;
            }

            var shown = slots.Take(5).ToList();
            var suffix = slots.Count > shown.Count ? ", ... (+" + (slots.Count - shown.Count) + ")" : string.Empty;
            return " (" + string.Join(", ", shown) + suffix + ")";
        }
    }
}
