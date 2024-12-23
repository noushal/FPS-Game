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
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// Combined history storage requirements for multiple stats across all RNSM display elements.
    internal class MultiStatHistoryRequirements
    {
        [NotNull]
        internal Dictionary<MetricId, StatHistoryRequirements> Data { get; } = new();

        internal static MultiStatHistoryRequirements FromConfiguration(NetStatsMonitorConfiguration configuration)
        {
            MultiStatHistoryRequirements multiStatHistoryRequirements = new();
            if (configuration == null)
            {
                return multiStatHistoryRequirements;
            }

            var allStatRequirements = multiStatHistoryRequirements.Data;
            foreach (var displayElement in configuration.DisplayElements)
            {
                var sampleCount = displayElement.SampleCount;
                var sampleRate = displayElement.SampleRate;
                var decayConstant = displayElement.DecayConstant;
                foreach (var metricId in displayElement.Stats)
                {
                    if (!allStatRequirements.ContainsKey(metricId))
                    {
                        allStatRequirements[metricId] = new StatHistoryRequirements(
                            decayConstants: new HashSet<double>(),
                            sampleCounts: new());
                    }
                    var requirements = multiStatHistoryRequirements.Data[metricId];

                    requirements.SampleCounts[sampleRate] = Math.Max(
                        requirements.SampleCounts[sampleRate],
                        sampleCount);

                    if (decayConstant.HasValue)
                    {
                        requirements.DecayConstants.Add(decayConstant.Value);
                    }
                }
            }
            return multiStatHistoryRequirements;
        }
    }
}
#endif
