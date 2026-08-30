using System;
using System.Threading.Tasks;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Runs every suite and returns a non-zero exit code if anything failed.
    ///
    ///   dotnet run --project tests/RealWeatherSync.Tests
    ///   dotnet run --project tests/RealWeatherSync.Tests -- --offline
    ///
    /// --offline skips the tests that call the real Open-Meteo API.
    /// </summary>
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var offline = Array.IndexOf(args, "--offline") >= 0;

            Console.WriteLine("Real Weather Sync - test suite");
            Console.WriteLine(offline
                ? "Mode: offline (live Open-Meteo tests skipped)"
                : "Mode: full (includes live Open-Meteo calls)");

            Assert.Reset();

            MapperTests.Run();
            TimelineTests.Run();
            LocationTests.Run();

            if (offline)
            {
                Assert.Section("Live Open-Meteo tests");
                Console.WriteLine("  skipped (--offline)");
            }
            else
            {
                try
                {
                    await OpenMeteoLiveTests.RunAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // A dead connection should read as "could not run", not as a silent pass.
                    Assert.Section("Live Open-Meteo tests");
                    Assert.True("the live suite completed (" + e.GetType().Name + ": " + e.Message + ")", false);
                    Console.WriteLine();
                    Console.WriteLine("  Hint: no internet? Re-run with --offline to skip the live tests.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("=====================================");
            Console.WriteLine("  passed: " + Assert.Passed);
            Console.WriteLine("  failed: " + Assert.Failed);
            Console.WriteLine(Assert.Failed == 0 ? "  RESULT: PASS" : "  RESULT: FAIL");
            Console.WriteLine("=====================================");

            return Assert.Failed == 0 ? 0 : 1;
        }
    }
}
