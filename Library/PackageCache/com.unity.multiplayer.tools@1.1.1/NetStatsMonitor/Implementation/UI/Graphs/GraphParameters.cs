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
    struct GraphParameters
    {
        public int StatCount { get; set; }
        public int SamplesPerStat { get; set; }
    }

    struct GraphBufferParameters
    {
        public const float k_MaxPointsPerPixel = 1.0f;

        public int StatCount { get; set; }
        public int GraphWidthPoints { get; set; }

        internal GraphBufferParameters(in GraphParameters graphParams, float graphContentWidth, float maxPointsPerPixel)
        {
            StatCount = graphParams.StatCount;
            GraphWidthPoints = Math.Min((int)(maxPointsPerPixel * graphContentWidth), graphParams.SamplesPerStat);
        }
    }
}
#endif
