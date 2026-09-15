using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RealWeatherSync.ApiScan
{
    /// <summary>
    /// Answers one question, mechanically: which parts of the game read the four climate values
    /// this mod overrides, and do they see the mod's number or the game's own?
    ///
    /// This matters because the mod's public honesty claim depends on it. `ClimateSystem` exposes
    /// each value as an `OverridableProperty&lt;float&gt;`. Reading it through `op_Implicit` returns
    /// the override when one is active - so that caller sees the mod's weather. Reading `.value`
    /// returns the game's own number regardless. The README and the store listing both state which
    /// systems fall on which side, and a game update can silently move one across.
    ///
    /// So the tool compares against a checked-in baseline and fails when the picture changes,
    /// rather than printing a wall of text for a human to eyeball.
    /// </summary>
    internal static class Program
    {
        private const string ClimateSystem = "Game.Simulation.ClimateSystem";

        /// <summary>The four values the mod writes. Nothing else is scanned on purpose.</summary>
        private static readonly string[] Overridden = { "temperature", "cloudiness", "precipitation", "fog" };

        /// <summary>Computed from the overridden temperature and precipitation, so readers of these are affected too.</summary>
        private static readonly string[] DerivedFlags = { "isRaining", "isSnowing", "isPrecipitating" };

        private static int Main(string[] args)
        {
            try
            {
                return Run(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("api-scan failed: " + e.Message);
                return 2;
            }
        }

        private static int Run(string[] args)
        {
            var gamePath = ValueOf(args, "--game") ?? FindGame();
            var baselinePath = ValueOf(args, "--baseline") ?? "baseline.txt";
            var update = args.Contains("--update-baseline");

            if (gamePath == null)
            {
                Console.Error.WriteLine(
                    "Could not locate Cities: Skylines II. Pass --game \"<game folder>\" or set CSII_GAMEPATH.");
                return 2;
            }

            var assembly = Path.Combine(gamePath, "Cities2_Data", "Managed", "Game.dll");
            if (!File.Exists(assembly))
            {
                Console.Error.WriteLine("Game.dll not found at: " + assembly);
                return 2;
            }

            var version = ReadGameVersion(gamePath) ?? "unknown";
            Console.WriteLine("Game:     " + gamePath);
            Console.WriteLine("Version:  " + version);
            Console.WriteLine("Baseline: " + Path.GetFullPath(baselinePath));
            Console.WriteLine();

            var found = Scan(assembly);

            if (update)
            {
                WriteBaseline(baselinePath, version, found);
                Console.WriteLine("Baseline rewritten with " + found.Count + " entries.");
                Console.WriteLine("Review the diff before committing - this file is what the README's");
                Console.WriteLine("\"What the game reads back\" section is checked against.");
                return 0;
            }

            if (!File.Exists(baselinePath))
            {
                Console.Error.WriteLine("No baseline at " + Path.GetFullPath(baselinePath) +
                                        ". Create one with --update-baseline.");
                return 2;
            }

            var expected = ReadBaseline(baselinePath);
            var added = found.Except(expected, StringComparer.Ordinal).OrderBy(s => s, StringComparer.Ordinal).ToList();
            var removed = expected.Except(found, StringComparer.Ordinal).OrderBy(s => s, StringComparer.Ordinal).ToList();

            if (added.Count == 0 && removed.Count == 0)
            {
                Console.WriteLine("UNCHANGED - " + found.Count + " entries match the baseline.");
                Console.WriteLine("The documented consumer table is still accurate.");
                return 0;
            }

            Console.WriteLine("CHANGED - the game now reads these values differently.");
            Console.WriteLine();

            foreach (var line in removed)
            {
                Console.WriteLine("  GONE   " + line);
            }

            foreach (var line in added)
            {
                Console.WriteLine("  NEW    " + line);
            }

            Console.WriteLine();
            Console.WriteLine("A line moving between 'implicit' and 'value' changes what the mod actually");
            Console.WriteLine("affects, so update the README's \"What the game reads back\" section and the");
            Console.WriteLine("store listing before shipping, then re-run with --update-baseline.");
            return 1;
        }

        /// <summary>
        /// One line per (caller, how it reads, which value). Sorted, so the baseline diffs cleanly
        /// in git and a reviewer can see exactly what moved.
        /// </summary>
        private static SortedSet<string> Scan(string assemblyPath)
        {
            var results = new SortedSet<string>(StringComparer.Ordinal);
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

            foreach (var type in assembly.MainModule.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                    {
                        continue;
                    }

                    var il = method.Body.Instructions;
                    for (var i = 0; i < il.Count; i++)
                    {
                        if (!(il[i].Operand is MethodReference callee) ||
                            callee.DeclaringType?.FullName != ClimateSystem)
                        {
                            continue;
                        }

                        var flag = DerivedFlags.FirstOrDefault(f => callee.Name == "get_" + f);
                        if (flag != null)
                        {
                            results.Add(Format("flag", Caller(type), flag));
                            continue;
                        }

                        var value = Overridden.FirstOrDefault(v => callee.Name == "get_" + v);
                        if (value == null)
                        {
                            continue;
                        }

                        // The property hands back an OverridableProperty; the very next call on it
                        // decides whether this caller sees the override or the game's own number.
                        var how = UnwrapKind(il, i);
                        if (how != null)
                        {
                            results.Add(Format(how, Caller(type), value));
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// "implicit" when the struct is unwrapped through op_Implicit (sees the mod's override),
        /// "value" when read through .value (always the game's own), null when neither appears
        /// close enough to attribute confidently.
        /// </summary>
        private static string UnwrapKind(IList<Instruction> il, int start)
        {
            for (var j = start + 1; j < Math.Min(start + 6, il.Count); j++)
            {
                if (!(il[j].Operand is MethodReference next) ||
                    next.DeclaringType?.Name.StartsWith("OverridableProperty", StringComparison.Ordinal) != true)
                {
                    continue;
                }

                if (next.Name == "op_Implicit")
                {
                    return "implicit";
                }

                if (next.Name == "get_value")
                {
                    return "value";
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Jobs are nested inside their system, and the system is the name the documentation uses,
        /// so report the outermost type and keep the nested name only when it adds something.
        /// </summary>
        private static string Caller(TypeDefinition type)
        {
            var outer = type;
            while (outer.DeclaringType != null)
            {
                outer = outer.DeclaringType;
            }

            return ReferenceEquals(outer, type) ? outer.Name : outer.Name + "." + type.Name;
        }

        private static string Format(string kind, string caller, string member)
        {
            return kind.PadRight(8) + " " + caller + " " + member;
        }

        private static void WriteBaseline(string path, string version, IEnumerable<string> lines)
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var text = new StringBuilder();
            text.AppendLine("# Which parts of Cities: Skylines II read the four climate values this mod overrides.");
            text.AppendLine("#");
            text.AppendLine("#   implicit  the caller sees the MOD's value (read through op_Implicit)");
            text.AppendLine("#   value     the caller sees the GAME's own value (read through .value)");
            text.AppendLine("#   flag      the caller reads isRaining / isSnowing / isPrecipitating,");
            text.AppendLine("#             which are computed from the overridden temperature and precipitation");
            text.AppendLine("#");
            text.AppendLine("# Generated by tools/ApiScan. Re-run after every game update:  .\\run-apiscan.ps1");
            text.AppendLine("# Rewrite after reviewing a real change:  .\\run-apiscan.ps1 -UpdateBaseline");
            text.AppendLine("#");
            text.AppendLine("# Last verified against game version: " + version);
            text.AppendLine();

            foreach (var line in lines)
            {
                text.AppendLine(line);
            }

            File.WriteAllText(path, text.ToString());
        }

        private static SortedSet<string> ReadBaseline(string path)
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                // Normalise the column padding so a formatting tweak is not reported as a change.
                set.Add(Normalise(line));
            }

            return set;
        }

        private static string Normalise(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 3 ? Format(parts[0], parts[1], parts[2]) : line;
        }

        /// <summary>
        /// The game's own version (for example "1.6.2f1"), read from the Unity asset bundle.
        ///
        /// Do not take this from the logs: they carry the version of the last launch, so straight
        /// after an update they report the version you no longer have - which is exactly the trap
        /// this tool exists to avoid. The binaries are no help either; Cities2.exe reports Unity's
        /// version, not Colossal's.
        ///
        /// globalgamemanagers holds both, in the same "1.2.3f4" shape. They are told apart by the
        /// major component: Unity versions are years (2022.3.71f1), the game's is not.
        /// </summary>
        private static string ReadGameVersion(string gamePath)
        {
            var file = Path.Combine(gamePath, "Cities2_Data", "globalgamemanagers");
            if (!File.Exists(file))
            {
                return null;
            }

            var text = Encoding.Latin1.GetString(File.ReadAllBytes(file));

            foreach (Match match in Regex.Matches(text, @"(\d+)\.\d+\.\d+f\d+"))
            {
                if (int.TryParse(match.Groups[1].Value, out var major) && major < 1000)
                {
                    return match.Value;
                }
            }

            return null;
        }

        /// <summary>Same search order the mod's csproj uses, so both agree on which install is meant.</summary>
        private static string FindGame()
        {
            var configured = Environment.GetEnvironmentVariable("CSII_GAMEPATH");
            if (!string.IsNullOrEmpty(configured) && Directory.Exists(configured))
            {
                return configured;
            }

            var candidates = new[]
            {
                @"C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II",
                @"C:\Program Files\Steam\steamapps\common\Cities Skylines II",
                @"D:\SteamLibrary\steamapps\common\Cities Skylines II"
            };

            return candidates.FirstOrDefault(c => File.Exists(Path.Combine(c, "Cities2_Data", "Managed", "Game.dll")));
        }

        private static string ValueOf(string[] args, string name)
        {
            var index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
