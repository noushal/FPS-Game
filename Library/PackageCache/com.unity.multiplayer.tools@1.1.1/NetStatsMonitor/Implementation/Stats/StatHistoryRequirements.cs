// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// Combined storage requirements for a single stat across all widgets that display it.
    /// A single display widget would impose only one storage requirement, but multiple widgets
    /// displaying the same stat in different ways may impose multiple requirements.
    internal class StatHistoryRequirements
    {
        /// Decay Constants of any Continuous Exponential Moving Averages required.
        /// Stored in a HashSet to avoid duplicates, which could otherwise result in duplicate work.
        public HashSet<double> DecayConstants { get; }

        /// The number of past values required (may be zero if only exponential moving averages are used).
        /// In practice, this will be the capacity of the ring-buffer used to store past values.
        /// If multiple widgets need to display historical values of a stat, then the number of past
        /// values required will be the maximum of the individual requirements.
        public EnumMap<SampleRate, int> SampleCounts { get; set; }// = new();

        public StatHistoryRequirements()
        {
            DecayConstants = new HashSet<double>();
            SampleCounts = new();
        }

        public StatHistoryRequirements(
            HashSet<double> decayConstants,
            EnumMap<SampleRate, int> sampleCounts)
        {
            DecayConstants = decayConstants;
            SampleCounts = sampleCounts;
        }

        public StatHistoryRequirements(
            IEnumerable<double> decayConstants,
            EnumMap<SampleRate, int> sampleCounts)
        {
            DecayConstants = decayConstants.ToHashSet();
            SampleCounts = sampleCounts;
        }
    }
}
#endif
