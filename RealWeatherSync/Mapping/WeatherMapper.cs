using RealWeatherSync.Models;

namespace RealWeatherSync.Mapping
{
    /// <summary>
    /// Options that influence the mapping. Passed in rather than read from the
    /// settings object so this class stays free of game dependencies and is
    /// straightforward to exercise in isolation.
    /// </summary>
    public struct WeatherMappingOptions
    {
        /// <summary>When false, <see cref="ClimateTarget.Fog"/> is always 0 and fog is not overridden.</summary>
        public bool SyncFog;

        /// <summary>
        /// When true and the provider reports snow, pull the visual temperature just
        /// below the game's freezing point so the engine renders snow instead of rain.
        /// </summary>
        public bool ForceSnowAppearance;

        /// <summary>The game's freezing temperature, read from ClimateSystem.freezingTemperature.</summary>
        public float FreezingTemperatureCelsius;

        /// <summary>
        /// Novelty toggle: mirror the mapped result. Warm becomes cold, clear becomes overcast,
        /// dry becomes wet. Fog is deliberately left alone - see <see cref="WeatherMapper.ApplyOppositeDay"/>.
        /// </summary>
        public bool OppositeDay;

        public static WeatherMappingOptions Default
        {
            get
            {
                WeatherMappingOptions o;
                o.SyncFog = true;
                o.ForceSnowAppearance = true;
                o.FreezingTemperatureCelsius = 0f;
                o.OppositeDay = false;
                return o;
            }
        }
    }

    /// <summary>
    /// Converts a real world <see cref="WeatherSnapshot"/> into the visual
    /// <see cref="ClimateTarget"/> that Cities: Skylines II understands.
    ///
    /// All game facing ranges and every conversion constant live here so the
    /// simulation system never has to carry magic numbers.
    ///
    /// Verified game ranges (Game.Simulation.ClimateSystem, game build inspected
    /// while writing this mod - see README "Weather mapping"):
    ///   temperature   : degrees Celsius, developer UI exposes -50 .. 50
    ///   cloudiness    : 0 .. 1
    ///   precipitation : 0 .. 1
    ///   fog           : 0 .. 1
    /// </summary>
    public static class WeatherMapper
    {
        public const float MinTemperatureCelsius = -50f;
        public const float MaxTemperatureCelsius = 50f;

        public const float MinCloudiness = 0f;
        public const float MaxCloudiness = 1f;

        public const float MinPrecipitation = 0f;
        public const float MaxPrecipitation = 1f;

        public const float MinFog = 0f;
        public const float MaxFog = 1f;

        /// <summary>
        /// How far below freezing the visual temperature is pushed when
        /// <see cref="WeatherMappingOptions.ForceSnowAppearance"/> kicks in.
        /// Small on purpose: it should read as "just below zero", not as arctic.
        /// </summary>
        private const float SnowTemperatureMargin = 1.5f;

        /// <summary>
        /// Rain intensity curve. Breakpoints are millimetres of liquid precipitation
        /// accumulated over the preceding hour (the unit Open-Meteo reports), mapped
        /// onto the game's 0..1 visual scale. Piecewise linear between breakpoints.
        ///
        /// The curve is deliberately compressed at the bottom: meteorological
        /// "light rain" starts around 0.5 mm/h and a drizzle of 0.1 mm/h should look
        /// like a drizzle, not like a downpour.
        ///
        ///   0.0 mm/h -> 0.00  dry
        ///   0.1 mm/h -> 0.10  very light / drizzle
        ///   0.5 mm/h -> 0.25  light rain
        ///   2.5 mm/h -> 0.50  moderate rain
        ///   7.5 mm/h -> 0.75  heavy rain
        ///  20.0 mm/h -> 1.00  violent rain
        /// </summary>
        private static readonly float[] RainMillimetres = { 0f, 0.1f, 0.5f, 2.5f, 7.5f, 20f };
        private static readonly float[] RainIntensity = { 0f, 0.10f, 0.25f, 0.50f, 0.75f, 1.00f };

        /// <summary>
        /// Snow intensity curve, in centimetres of fresh snow per hour. Snow is far
        /// more visible per millimetre of water than rain is, so it gets its own,
        /// steeper curve rather than being folded into the rain one.
        ///
        ///   0.0 cm/h -> 0.00  dry
        ///   0.1 cm/h -> 0.15  very light snow
        ///   0.5 cm/h -> 0.35  light snow
        ///   1.0 cm/h -> 0.55  moderate snow
        ///   3.0 cm/h -> 0.80  heavy snow
        ///   6.0 cm/h -> 1.00  blizzard
        /// </summary>
        private static readonly float[] SnowCentimetres = { 0f, 0.1f, 0.5f, 1.0f, 3.0f, 6.0f };
        private static readonly float[] SnowIntensity = { 0f, 0.15f, 0.35f, 0.55f, 0.80f, 1.00f };

        /// <summary>
        /// Visibility to fog curve, metres to 0..1. Conservative: fog only becomes
        /// pronounced once visibility is genuinely poor. Cloud cover never feeds
        /// into this - an overcast sky is not fog.
        ///
        ///  20000 m -> 0.00
        ///  10000 m -> 0.00
        ///   4000 m -> 0.20
        ///   1000 m -> 0.50
        ///    500 m -> 0.70
        ///    100 m -> 0.90
        /// </summary>
        private static readonly float[] VisibilityMetres = { 100f, 500f, 1000f, 4000f, 10000f, 20000f };
        private static readonly float[] VisibilityFog = { 0.90f, 0.70f, 0.50f, 0.20f, 0f, 0f };

        /// <summary>
        /// Fog density implied by the WMO fog codes, used when no visibility reading
        /// is available (and as a floor when one is).
        /// </summary>
        public const int WmoFog = 45;
        public const int WmoDepositingRimeFog = 48;
        private const float CodeFogDensity = 0.72f;
        private const float CodeRimeFogDensity = 0.82f;

        /// <summary>
        /// Maps a real world observation onto the game's visual climate values.
        /// The result is always inside the supported ranges.
        /// </summary>
        public static ClimateTarget Map(WeatherSnapshot snapshot, WeatherMappingOptions options)
        {
            ClimateTarget target;
            target.TemperatureCelsius = 0f;
            target.Cloudiness = 0f;
            target.Precipitation = 0f;
            target.Fog = 0f;

            if (snapshot == null)
            {
                return target;
            }

            target.TemperatureCelsius = MapTemperature(snapshot, options);
            target.Cloudiness = MapCloudiness(snapshot.CloudCoverPercent);
            target.Precipitation = MapPrecipitation(snapshot);
            target.Fog = options.SyncFog ? MapFog(snapshot) : 0f;

            if (options.OppositeDay)
            {
                target = ApplyOppositeDay(target);
            }

            return target;
        }

        /// <summary>The temperature that <see cref="ApplyOppositeDay"/> mirrors around.</summary>
        public const float OppositeDayPivotCelsius = 15f;

        /// <summary>
        /// Mirrors temperature around <see cref="OppositeDayPivotCelsius"/> and flips cloudiness
        /// and precipitation.
        ///
        /// Fog is intentionally NOT inverted: a clear day would become permanent dense fog, which
        /// hides the city entirely and stops being funny within seconds. The joke has to remain
        /// something you can actually look at.
        /// </summary>
        public static ClimateTarget ApplyOppositeDay(ClimateTarget target)
        {
            ClimateTarget flipped;
            flipped.TemperatureCelsius = Clamp(
                2f * OppositeDayPivotCelsius - target.TemperatureCelsius,
                MinTemperatureCelsius, MaxTemperatureCelsius);
            flipped.Cloudiness = Clamp(MaxCloudiness - target.Cloudiness, MinCloudiness, MaxCloudiness);
            flipped.Precipitation = Clamp(MaxPrecipitation - target.Precipitation, MinPrecipitation, MaxPrecipitation);
            flipped.Fog = target.Fog;
            return flipped;
        }

        /// <summary>Real Celsius, clamped to the range the game accepts.</summary>
        public static float MapTemperature(WeatherSnapshot snapshot, WeatherMappingOptions options)
        {
            var celsius = Clamp(snapshot.TemperatureCelsius, MinTemperatureCelsius, MaxTemperatureCelsius);

            // Cities: Skylines II decides between rain and snow from the visual
            // temperature, not from anything we can set directly. If it is really
            // snowing but the air temperature is above the game's freezing point the
            // engine would draw rain. Nudging the visual temperature just below
            // freezing is the only way to render the real condition.
            if (options.ForceSnowAppearance && IsSnowing(snapshot))
            {
                var snowCeiling = options.FreezingTemperatureCelsius - SnowTemperatureMargin;
                if (celsius > snowCeiling)
                {
                    celsius = Clamp(snowCeiling, MinTemperatureCelsius, MaxTemperatureCelsius);
                }
            }

            return celsius;
        }

        /// <summary>0-100 % cloud cover to the game's 0..1 cloudiness.</summary>
        public static float MapCloudiness(float cloudCoverPercent)
        {
            return Clamp(cloudCoverPercent / 100f, MinCloudiness, MaxCloudiness);
        }

        /// <summary>
        /// Combines the liquid and the frozen curves and takes whichever produces the
        /// stronger visual, then applies a floor derived from the WMO code so a
        /// reported shower is never drawn as a dry sky just because the hourly
        /// accumulation bucket is still empty.
        /// </summary>
        public static float MapPrecipitation(WeatherSnapshot snapshot)
        {
            var snowCm = Max(snapshot.SnowfallCm, 0f);
            var snowWaterEquivalentMm = snowCm * SnowWaterEquivalentMmPerCm;

            // "precipitation" already includes rain + showers + snow water equivalent,
            // but fall back to the individual components if the provider omitted it.
            var liquidMm = Max(snapshot.PrecipitationMm, 0f);
            var componentsMm = Max(snapshot.RainMm, 0f) + Max(snapshot.ShowersMm, 0f) + snowWaterEquivalentMm;
            if (componentsMm > liquidMm)
            {
                liquidMm = componentsMm;
            }

            var rainIntensity = Interpolate(RainMillimetres, RainIntensity, liquidMm);
            var snowIntensity = Interpolate(SnowCentimetres, SnowIntensity, snowCm);

            var intensity = Max(rainIntensity, snowIntensity);
            intensity = Max(intensity, MinimumIntensityForCode(snapshot.WeatherCode));

            return Clamp(intensity, MinPrecipitation, MaxPrecipitation);
        }

        /// <summary>
        /// Open-Meteo documents roughly 0.7 mm of water per centimetre of fresh snow.
        /// </summary>
        public const float SnowWaterEquivalentMmPerCm = 0.7f;

        /// <summary>
        /// Fog from the WMO code first, then from visibility. Never from cloud cover.
        /// </summary>
        public static float MapFog(WeatherSnapshot snapshot)
        {
            var fog = 0f;

            if (snapshot.WeatherCode == WmoFog)
            {
                fog = CodeFogDensity;
            }
            else if (snapshot.WeatherCode == WmoDepositingRimeFog)
            {
                fog = CodeRimeFogDensity;
            }

            if (snapshot.VisibilityMeters.HasValue)
            {
                var visibility = snapshot.VisibilityMeters.Value;
                if (visibility >= 0f)
                {
                    var fromVisibility = Interpolate(VisibilityMetres, VisibilityFog, visibility);

                    // Guard against a low visibility reading caused by heavy rain or
                    // snow rather than by actual fog: without a fog code, and with
                    // meaningful precipitation, do not manufacture thick fog.
                    if (!IsFogCode(snapshot.WeatherCode) && IsPrecipitating(snapshot))
                    {
                        fromVisibility = Min(fromVisibility, 0.25f);
                    }

                    fog = Max(fog, fromVisibility);
                }
            }

            return Clamp(fog, MinFog, MaxFog);
        }

        public static bool IsFogCode(int weatherCode)
        {
            return weatherCode == WmoFog || weatherCode == WmoDepositingRimeFog;
        }

        /// <summary>True when the provider indicates frozen precipitation.</summary>
        public static bool IsSnowing(WeatherSnapshot snapshot)
        {
            if (snapshot.SnowfallCm > 0f)
            {
                return true;
            }

            var code = snapshot.WeatherCode;

            // 71/73/75 snow fall, 77 snow grains, 85/86 snow showers,
            // 66/67 freezing rain, 56/57 freezing drizzle.
            return code == 71 || code == 73 || code == 75 || code == 77
                   || code == 85 || code == 86
                   || code == 66 || code == 67
                   || code == 56 || code == 57;
        }

        public static bool IsPrecipitating(WeatherSnapshot snapshot)
        {
            return snapshot.PrecipitationMm > 0.05f
                   || snapshot.RainMm > 0.05f
                   || snapshot.ShowersMm > 0.05f
                   || snapshot.SnowfallCm > 0.02f;
        }

        /// <summary>
        /// Minimum visual intensity implied by a WMO code. Kept low: the code only
        /// establishes that something is falling, the measured amounts decide how hard.
        /// </summary>
        public static float MinimumIntensityForCode(int weatherCode)
        {
            switch (weatherCode)
            {
                // Light drizzle / light freezing drizzle / light snow / snow grains / slight snow showers.
                case 51:
                case 56:
                case 71:
                case 77:
                case 85:
                    return 0.08f;

                // Moderate drizzle / dense freezing drizzle / slight rain / moderate snow / slight rain showers / heavy snow showers.
                case 53:
                case 57:
                case 61:
                case 73:
                case 80:
                case 86:
                    return 0.20f;

                // Dense drizzle / moderate rain / light freezing rain / heavy snow / moderate rain showers.
                case 55:
                case 63:
                case 66:
                case 75:
                case 81:
                    return 0.38f;

                // Heavy rain / heavy freezing rain / violent rain showers.
                case 65:
                case 67:
                case 82:
                    return 0.60f;

                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Piecewise linear lookup. <paramref name="xs"/> must be sorted ascending;
        /// values outside the table clamp to the first / last entry.
        /// </summary>
        internal static float Interpolate(float[] xs, float[] ys, float x)
        {
            if (xs == null || ys == null || xs.Length == 0 || xs.Length != ys.Length)
            {
                return 0f;
            }

            if (x <= xs[0])
            {
                return ys[0];
            }

            var last = xs.Length - 1;
            if (x >= xs[last])
            {
                return ys[last];
            }

            for (var i = 1; i <= last; i++)
            {
                if (x > xs[i])
                {
                    continue;
                }

                var span = xs[i] - xs[i - 1];
                if (span <= 0f)
                {
                    return ys[i];
                }

                var t = (x - xs[i - 1]) / span;
                return ys[i - 1] + (ys[i] - ys[i - 1]) * t;
            }

            return ys[last];
        }

        internal static float Clamp(float value, float min, float max)
        {
            if (float.IsNaN(value))
            {
                return min;
            }

            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        private static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        private static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
    }
}
