using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using RealWeatherSync.Models;

namespace RealWeatherSync.Settings
{
    /// <summary>
    /// Most-recently-used list of resolved cities, persisted as a single delimited string.
    ///
    /// A flat string is used on purpose: the game's settings serialiser is reliable for simple
    /// scalars, and one string keeps the settings file readable and impossible to half-load.
    /// Fields are separated by '|' and entries by ';', both stripped from the stored values.
    /// </summary>
    public static class FavouriteCities
    {
        public const int MaxEntries = 10;

        private const char FieldSeparator = '|';
        private const char EntrySeparator = ';';
        private const int FieldCount = 7;

        /// <summary>Parses the persisted string. Never throws; unreadable entries are skipped.</summary>
        public static List<LocationResult> Parse(string raw)
        {
            var list = new List<LocationResult>();
            if (string.IsNullOrEmpty(raw))
            {
                return list;
            }

            var entries = raw.Split(new[] { EntrySeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                var parts = entry.Split(FieldSeparator);
                if (parts.Length < FieldCount)
                {
                    continue;
                }

                double latitude;
                double longitude;
                if (!double.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) ||
                    !double.TryParse(parts[6], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                {
                    continue;
                }

                var location = new LocationResult(
                    parts[0], parts[1], parts[2], parts[3], parts[4], string.Empty, latitude, longitude);

                if (location.HasValidCoordinates)
                {
                    list.Add(location);
                }

                if (list.Count >= MaxEntries)
                {
                    break;
                }
            }

            return list;
        }

        public static string Serialise(IEnumerable<LocationResult> locations)
        {
            var sb = new StringBuilder();
            var written = 0;

            foreach (var location in locations)
            {
                if (location == null || !location.HasValidCoordinates)
                {
                    continue;
                }

                if (written >= MaxEntries)
                {
                    break;
                }

                if (written > 0)
                {
                    sb.Append(EntrySeparator);
                }

                sb.Append(Clean(location.Query)).Append(FieldSeparator)
                  .Append(Clean(location.Name)).Append(FieldSeparator)
                  .Append(Clean(location.Admin1)).Append(FieldSeparator)
                  .Append(Clean(location.Country)).Append(FieldSeparator)
                  .Append(Clean(location.CountryCode)).Append(FieldSeparator)
                  .Append(location.Latitude.ToString("0.#####", CultureInfo.InvariantCulture)).Append(FieldSeparator)
                  .Append(location.Longitude.ToString("0.#####", CultureInfo.InvariantCulture));

                written++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Puts <paramref name="location"/> at the front, removing any previous entry for the
        /// same coordinates, and trims the list to <see cref="MaxEntries"/>.
        /// </summary>
        public static List<LocationResult> Promote(List<LocationResult> existing, LocationResult location)
        {
            var result = new List<LocationResult>();
            if (location != null && location.HasValidCoordinates)
            {
                result.Add(location);
            }

            if (existing != null)
            {
                foreach (var candidate in existing)
                {
                    if (candidate == null || IsSamePlace(candidate, location))
                    {
                        continue;
                    }

                    result.Add(candidate);
                    if (result.Count >= MaxEntries)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>Coordinates within ~100 m are treated as the same place.</summary>
        public static bool IsSamePlace(LocationResult a, LocationResult b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return Math.Abs(a.Latitude - b.Latitude) < 0.001
                   && Math.Abs(a.Longitude - b.Longitude) < 0.001;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace(FieldSeparator, ' ').Replace(EntrySeparator, ' ').Trim();
        }
    }
}
