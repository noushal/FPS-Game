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
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// The StatsAggregator computes high-level summary statistics from the MetricCollection.
    /// These high-level stats are stored in MultiStatHistory for display in the RNSM.
    /// https://en.wikipedia.org/wiki/Aggregate_data
    internal static class StatsAggregator
    {
        /// Updates the history with high-level summary statistics computed from the metrics collection
        /// <param name="metrics"> <see cref="MetricCollection"/>  to be aggregated into summary statistics</param>
        /// <param name="statsAccumulator"> <see cref="StatsAccumulator"/> to accumulate the summary statistics</param>
        internal static void UpdateAccumulatorWithStatsFromMetrics(
            MetricCollection metrics,
            StatsAccumulator statsAccumulator,
            double time)
        {
            foreach (var metricId in statsAccumulator.RequiredMetrics)
            {
                var metricKind = metricId.MetricKind;
                switch (metricKind)
                {
                    case MetricKind.Counter:
                    {
                        if (metrics.TryGetCounter(metricId, out var counter))
                        {
                            statsAccumulator.Accumulate(metricId, counter.Value);
                        }
                        else
                        {
                            var eventCount = metrics.GetEventCount(metricId);
                            statsAccumulator.Accumulate(metricId, eventCount);
                        }
                        break;
                    }
                    case MetricKind.Gauge:
                    {
                        if (metrics.TryGetGauge(metricId, out var gauge))
                        {
                            statsAccumulator.Accumulate(metricId, (float)gauge.Value);
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unhandled {nameof(MetricKind)} {metricKind}");
                }
            }
            statsAccumulator.LastAccumulationTime = time;
        }
    }
}
#endif
