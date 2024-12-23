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
    internal static class NetStatsMonitorConfigurationExtensions
    {

        /// Returns a hash of display element fields in the configuration related to history requirements,
        /// for the purpose of determining if the history requirements have changed and need to be updated
        internal static int GetHistoryRequirementsHash(this NetStatsMonitorConfiguration config)
        {
            int hashCode = 0;
            foreach (var displayElement in config.DisplayElements)
            {
                hashCode = HashCode.Combine(hashCode, displayElement.GetHistoryRequirementsHash());
            }
            return hashCode;
        }
    }
}
#endif