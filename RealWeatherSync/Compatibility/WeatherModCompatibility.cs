using System;
using System.Collections.Generic;
using System.Text;
using Game.SceneFlow;

namespace RealWeatherSync.Compatibility
{
    /// <summary>
    /// Best effort detection of other mods that also drive
    /// Game.Simulation.ClimateSystem's overridable properties.
    ///
    /// Two mods writing temperature / cloudiness / precipitation / fog every frame
    /// will simply overwrite each other, and the visible result depends on system
    /// ordering. Rather than let that happen, Real Weather Sync releases its own
    /// overrides and says so.
    ///
    /// Limitations, stated plainly:
    ///   - Detection is by mod name and assembly name. A mod that is renamed, or a
    ///     new weather mod that is not in the list below, will not be detected.
    ///   - Presence is not proof of conflict: a detected mod that never enables its
    ///     own overrides would not actually fight us, and we cannot tell.
    ///   - "Ignore mod conflicts" in the settings exists for exactly those cases.
    /// </summary>
    public static class WeatherModCompatibility
    {
        /// <summary>
        /// Lower case fragments matched against the mod name and assembly name.
        /// Kept deliberately specific to reduce false positives.
        /// </summary>
        private static readonly string[] KnownWeatherOverridingMods =
        {
            "timeweatheranarchy",
            "weatherplus",
            "niceweather",
            "daynightswitch",
            "weathercontrol",
            "realisticweather"
        };

        /// <summary>Our own identifiers, so we never flag ourselves.</summary>
        private static readonly string[] SelfIdentifiers =
        {
            "realweathersync"
        };

        /// <summary>
        /// Scans the loaded mods.
        /// </summary>
        /// <param name="conflicts">Display names of the detected mods; never null.</param>
        /// <returns>True when at least one known weather-overriding mod is loaded.</returns>
        public static bool TryDetectConflicts(out List<string> conflicts)
        {
            conflicts = new List<string>();

            try
            {
                var gameManager = GameManager.instance;
                if (gameManager == null)
                {
                    return false;
                }

                var modManager = gameManager.modManager;
                if (modManager == null)
                {
                    return false;
                }

                foreach (var modInfo in modManager)
                {
                    if (modInfo == null || !modInfo.isLoaded)
                    {
                        continue;
                    }

                    var name = modInfo.name ?? string.Empty;
                    var assembly = modInfo.assemblyFullName ?? string.Empty;

                    if (IsSelf(name) || IsSelf(assembly))
                    {
                        continue;
                    }

                    if (IsKnownConflict(name) || IsKnownConflict(assembly))
                    {
                        var label = string.IsNullOrEmpty(name) ? assembly : name;
                        if (!conflicts.Contains(label))
                        {
                            conflicts.Add(label);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // The mod manager shape is not part of a stable public contract.
                // If enumerating it ever fails, treat that as "no conflict detected"
                // rather than blocking the player; the README documents the risk.
                return false;
            }

            return conflicts.Count > 0;
        }

        public static string Describe(List<string> conflicts)
        {
            if (conflicts == null || conflicts.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (var i = 0; i < conflicts.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(conflicts[i]);
            }

            return sb.ToString();
        }

        private static bool IsKnownConflict(string value)
        {
            var normalised = Normalise(value);
            if (normalised.Length == 0)
            {
                return false;
            }

            foreach (var fragment in KnownWeatherOverridingMods)
            {
                if (normalised.IndexOf(fragment, StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSelf(string value)
        {
            var normalised = Normalise(value);
            if (normalised.Length == 0)
            {
                return false;
            }

            foreach (var fragment in SelfIdentifiers)
            {
                if (normalised.IndexOf(fragment, StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Lower cases and strips spaces / punctuation so "Time &amp; Weather Anarchy" matches.</summary>
        private static string Normalise(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            return sb.ToString();
        }
    }
}
