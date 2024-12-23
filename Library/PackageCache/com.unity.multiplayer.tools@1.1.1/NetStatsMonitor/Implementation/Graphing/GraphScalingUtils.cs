// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    internal static class GraphScalingUtils
    {
        /// A list of numbers to satisfy the following constraints:
        /// 1. All should be "round" numbers that don't take up too much horizontal
        ///    space as graph axis labels
        /// 2. The ratio between each pair of adjacent numbers should be as similar
        ///    as possible, so that the graph scales by reasonably consistent intervals.
        ///    The ratios between the numbers chosen here are:
        ///        1.5, 1.3̅3̅, 1.5, 1.3̅3̅, 1.25, 1.2, 1.3̅3̅, 1.25
        static readonly float[] k_RoundNumbers = { 1, 1.5f, 2, 3, 4, 5, 6, 8, 10 };

        public static MantissaAndExponent NextLargestRoundNumber(float value)
        {
            if (value == 0)
            {
                // The next highest random number is infinitely small, so also zero
                return new MantissaAndExponent();
            }

            // Operate on the absolute value and store the sign for later
            var sign = MathF.Sign(value);
            value = MathF.Abs(value);

            var exponentOf10 = MathF.Floor(MathF.Log10(value));

            var powerOf10 = MathF.Pow(10, exponentOf10);

            var mantissa = value / powerOf10;

            var nextRoundNumber = k_RoundNumbers[0];
            for (int i = 0; i < k_RoundNumbers.Length; ++i)
            {
                var roundNumber = k_RoundNumbers[i];
                if (roundNumber >= mantissa)
                {
                    nextRoundNumber = roundNumber;
                    break;
                }
            }

            if (nextRoundNumber == 10f)
            {
                nextRoundNumber = 1f;
                exponentOf10 += 1;
            }
            return new MantissaAndExponent
            {
                Mantissa = sign * nextRoundNumber,
                Exponent = (int)exponentOf10,
            };
        }
    }
}
#endif