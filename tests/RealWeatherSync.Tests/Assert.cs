using System;
using System.Globalization;

namespace RealWeatherSync.Tests
{
    /// <summary>
    /// Minimal assertion helpers. No test framework on purpose: the harness has to compile
    /// against linked mod sources on net8.0 with as few moving parts as possible, and its whole
    /// job is to print a readable pass/fail list and return a non-zero exit code on failure.
    /// </summary>
    public static class Assert
    {
        /// <summary>Values closer than this count as equal. Roughly float display precision.</summary>
        private const float Tolerance = 0.0011f;

        public static int Passed { get; private set; }
        public static int Failed { get; private set; }

        public static void Section(string title)
        {
            Console.WriteLine();
            Console.WriteLine("--- " + title + " ---");
        }

        /// <summary>Floating point comparison within <see cref="Tolerance"/>.</summary>
        public static void Near(string label, float actual, float expected)
        {
            Report(Math.Abs(actual - expected) < Tolerance, string.Format(CultureInfo.InvariantCulture,
                "{0,-46} actual={1,10:0.000}  expected={2,10:0.000}", label, actual, expected));
        }

        public static void Equal(string label, string actual, string expected)
        {
            Report(actual == expected, label + "  actual=\"" + actual + "\"  expected=\"" + expected + "\"");
        }

        public static void Equal(string label, int actual, int expected)
        {
            Report(actual == expected, label + "  actual=" + actual + "  expected=" + expected);
        }

        public static void Equal(string label, DateTime actual, DateTime expected)
        {
            Report(actual == expected, label +
                                       "  actual=" + actual.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) +
                                       "  expected=" + expected.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }

        public static void True(string label, bool condition)
        {
            Report(condition, label);
        }

        public static void Reset()
        {
            Passed = 0;
            Failed = 0;
        }

        private static void Report(bool ok, string text)
        {
            if (ok)
            {
                Passed++;
            }
            else
            {
                Failed++;
            }

            Console.WriteLine((ok ? "  ok   " : "  FAIL ") + text);
        }
    }
}
