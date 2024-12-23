// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation.Graphing
{
    internal static class GraphColorUtils
    {
        /// Set of colors for labelling quantitative data from www.colorbrewer2.org,
        /// that can be distinguished by those with red-green color blindness
        static readonly Color32[] k_ColorSeries_ColorBlindSafe = new[]
        {
            new Color32(166, 206, 227, 255),
            new Color32(031, 120, 180, 255),
            new Color32(178, 223, 138, 255),
            new Color32(051, 160, 044, 255),
        };

        /// A larger series of colors which can display more values from www.colorbrewer2.org,
        /// but may not be easily distinguishable for those with color blindness.
        /// Users can customize the colors on the plot so if this series isn't
        /// suitable for their purposes they can create their own.
        static readonly Color32[] k_ColorSeries_Larger = new[]
        {
            new Color32(141, 211, 199, 255),
            new Color32(255, 255, 179, 255),
            new Color32(190, 186, 218, 255),
            new Color32(251, 128, 114, 255),
            new Color32(128, 177, 211, 255),
            new Color32(253, 180, 098, 255),
            new Color32(179, 222, 105, 255),
            new Color32(252, 205, 229, 255),
            new Color32(217, 217, 217, 255),
            new Color32(188, 128, 189, 255),
            new Color32(204, 235, 197, 255),
            new Color32(255, 237, 111, 255),
        };

        const float k_ColorDefaultSaturation = 0.8f;

        /// Value chosen to match the average value of k_ColorSeriesLarger
        /// so that any random colors needed beyond the first 12 won't be
        /// jarringly different
        const float k_ColorDefaultValue = 0.72f;

        static Color32 GetRandomColorForIndex(int statIndex)
        {
            const int k_randomPrime0 = 17;
            const int k_randomPrime1 = 67;
            const int k_randomPrime2 = 419;
            var hue = (((statIndex + k_randomPrime0) * k_randomPrime1) % k_randomPrime2) / k_randomPrime2;
            return Color.HSVToRGB(hue, k_ColorDefaultSaturation, k_ColorDefaultValue);
        }

        public static Color32 GetColorForIndex(int statIndex, int totalStatCount)
        {
            if (totalStatCount <= k_ColorSeries_ColorBlindSafe.Length)
            {
                return k_ColorSeries_ColorBlindSafe[statIndex];
            }

            // Fall back on the larger list if we don't have enough color-blind safe colors to support their data set
            if (statIndex < k_ColorSeries_Larger.Length)
            {
                return k_ColorSeries_Larger[statIndex];
            }

            // Fall back on random colors if we don't have enough colors in any series to support their data set
            return GetRandomColorForIndex(statIndex);
        }
    }
}
#endif