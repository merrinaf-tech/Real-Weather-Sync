using System.Globalization;

namespace RealWeatherSync.Models
{
    /// <summary>
    /// The four visual climate values Real Weather Sync is allowed to override,
    /// already expressed in the units Game.Simulation.ClimateSystem expects.
    /// </summary>
    public struct ClimateTarget
    {
        /// <summary>Visual temperature in degrees Celsius (game range -50..50).</summary>
        public float TemperatureCelsius;

        /// <summary>Cloudiness, 0 (clear) .. 1 (overcast).</summary>
        public float Cloudiness;

        /// <summary>Precipitation intensity, 0 (dry) .. 1 (heaviest).</summary>
        public float Precipitation;

        /// <summary>Fog density, 0 (none) .. 1 (thickest).</summary>
        public float Fog;

        public static ClimateTarget Lerp(ClimateTarget from, ClimateTarget to, float t)
        {
            if (t <= 0f)
            {
                return from;
            }

            if (t >= 1f)
            {
                return to;
            }

            ClimateTarget result;
            result.TemperatureCelsius = from.TemperatureCelsius + (to.TemperatureCelsius - from.TemperatureCelsius) * t;
            result.Cloudiness = from.Cloudiness + (to.Cloudiness - from.Cloudiness) * t;
            result.Precipitation = from.Precipitation + (to.Precipitation - from.Precipitation) * t;
            result.Fog = from.Fog + (to.Fog - from.Fog) * t;
            return result;
        }

        public override string ToString()
        {
            var ci = CultureInfo.InvariantCulture;
            return string.Concat(
                "temperature=", TemperatureCelsius.ToString("0.0", ci),
                " cloudiness=", Cloudiness.ToString("0.000", ci),
                " precipitation=", Precipitation.ToString("0.000", ci),
                " fog=", Fog.ToString("0.000", ci));
        }
    }
}
