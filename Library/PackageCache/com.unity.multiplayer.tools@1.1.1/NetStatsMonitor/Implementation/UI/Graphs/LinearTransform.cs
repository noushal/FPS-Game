// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    struct LinearTransform
    {
        /// As in y = a * x + b
        public float A { get; set; }

        /// As in y = a * x + b
        public float B { get; set; }

        public float Apply(float x)
        {
            // Math.FusedMultiplyAdd doesn't seem to be available for some reason.
            // Hopefully this optimization is still possible with Burst
            return A * x + B;
        }

        public bool IsIdentity => A == 1f && B == 0f;

        public static LinearTransform Identity => new LinearTransform { A = 1, B = 0 };
    }
}
#endif
