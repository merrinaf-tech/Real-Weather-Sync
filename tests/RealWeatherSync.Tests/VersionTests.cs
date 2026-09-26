using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// The version is written in four places, and only the publish configuration is checked by
    /// anything else. Mod.Version is shown in the About section and sent as the User-Agent to
    /// Open-Meteo and NOAA, and it still said 1.5.0 while 1.6.0 was live. Mod.cs cannot be linked
    /// here (it needs the game), so the files are read as text.
    /// </summary>
    public static class VersionTests
    {
        public static void Run()
        {
            Assert.Section("Version - one number everywhere");

            var root = FindRoot();
            if (root == null)
            {
                Assert.True("RealWeatherSync.sln found above " + AppContext.BaseDirectory, false);
                return;
            }

            var published = Read(root, "<ModVersion Value=\"([^\"]+)\"",
                "RealWeatherSync", "Properties", "PublishConfiguration.xml");

            Assert.Equal("Mod.Version matches the published version",
                Read(root, "const string Version = \"([^\"]+)\"", "RealWeatherSync", "Mod.cs"), published);
            Assert.Equal("csproj Version matches the published version",
                Read(root, "<Version>([^<]+)</Version>", "RealWeatherSync", "RealWeatherSync.csproj"), published);
            Assert.Equal("csproj AssemblyVersion matches the published version",
                Read(root, "<AssemblyVersion>([^<]+)</AssemblyVersion>", "RealWeatherSync", "RealWeatherSync.csproj"),
                published + ".0");
        }

        private static string FindRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "RealWeatherSync.sln")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName;
        }

        private static string Read(string root, string pattern, params string[] path)
        {
            var match = Regex.Match(File.ReadAllText(Path.Combine(root, Path.Combine(path))), pattern);
            return match.Success ? match.Groups[1].Value : "(not found in " + Path.Combine(path) + ")";
        }
    }
}
