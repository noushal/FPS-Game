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
    public static class DisplayElementConfigurationExtensions
    {
        /// Returns a hash of display element configuration fields related to history requirements,
        /// for the purpose of determining if history requirements have changed and need to be updated
        internal static int GetHistoryRequirementsHash(this DisplayElementConfiguration config)
        {
            var hashCode = 0;
            foreach (var metricId in config.Stats)
            {
                hashCode = HashCode.Combine(hashCode, metricId.GetHashCode());
            }
            hashCode = HashCode.Combine(hashCode, config.SampleCount, config.SampleRate, config.HalfLife);
            return hashCode;
        }
    }
}
#endif