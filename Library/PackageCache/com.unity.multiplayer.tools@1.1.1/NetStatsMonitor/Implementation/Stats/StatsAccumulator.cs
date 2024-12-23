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
using System.Linq;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// <summary>
    /// The <see cref="StatsAccumulator"/> accumulates high-level summary statistics from the <see cref="StatsAggregator"/>.
    /// These high-level stats are accumulated until they are collected by the <see cref="MultiStatHistory"/>.
    /// </summary>
    /// <remarks>
    /// By accumulating and then collecting all stats received within a sample period (such as a frame),
    /// we can synchronise the gathering of multiple stats across time, even if different stats are received
    /// at inconsistent, random, or unpredictable intervals.
    /// <br/> <br/>
    /// This synchronization allows us to plot these values together consistently. It also allows us to store only one
    /// 64-bit time stamp per sample period as opposed to one 64-bit time stamp per 32-bit sample, which can reduce our
    /// memory usage by as much as 1 - (8 + N * 4) / (N * 16), where N is the number of stats. For N = 8 this is a ~69%
    /// reduction. For arbitrarily large N this is a 75% reduction.
    /// </remarks>
    class StatsAccumulator
    {
        [NotNull]
        readonly Dictionary<MetricId, float> m_Sums = new();

        [NotNull]
        readonly Dictionary<MetricId, int> m_GaugeCounts = new();

        internal bool HasAccumulatedStats => LastAccumulationTime > LastCollectionTime;
        internal double LastAccumulationTime { get; set; } = Constants.k_DefaultTimestamp;
        internal double LastCollectionTime { get; set; } = Constants.k_DefaultTimestamp;

        internal bool Contains(MetricId metricId)
        {
            return m_Sums.ContainsKey(metricId);
        }

        /// <summary>
        /// Array of Metrics that are required by this accumulator,
        /// according to the MultiStatHistoryRequirements.
        /// </summary>
        /// <remarks>
        /// This is stored separately from the keys of the dictionary
        /// so that it is possible to iterate over them while updating
        /// the contents without allocating a new list of the keys each
        /// time.
        /// </remarks>
        internal MetricId[] RequiredMetrics { get; private set; } = Array.Empty<MetricId>();

        /// <exception cref="KeyNotFoundException">
        /// Thrown if the accumulator does not contain an entry for this StatName
        /// </exception>
        internal void Accumulate(MetricId metricId, float value)
        {
            m_Sums[metricId] += value;
            if (m_GaugeCounts.TryGetValue(metricId, out var gaugeCount))
            {
                m_GaugeCounts[metricId] = gaugeCount + 1;
            }
        }

        internal float Collect(MetricId metric)
        {
            if (m_Sums.TryGetValue(metric, out var sum))
            {
                m_Sums[metric] = 0;

                if (m_GaugeCounts.TryGetValue(metric, out var count))
                {
                    m_GaugeCounts[metric] = 0;

                    // Avoid division by zero in cases where no data has been received
                    return sum / Math.Max(count, 1);
                }
                return sum;
            }
            return 0;
        }

        /// <summary>
        /// Update the data stored in this accumulator based on the overall
        /// requirements and the sample rate of this accumulator
        /// </summary>
        /// <param name="requirements"> The overall requirements of the history </param>
        /// <param name="sampleRate"> The sample rate of this particular accumulator </param>
        internal void UpdateRequirements(MultiStatHistoryRequirements requirements, SampleRate sampleRate)
        {
            bool Required(MetricId metric)
            {
                // Whether a metric is required depends not only on whether it appears in the requirements,
                // but also on the sample rate that is required
                if (!requirements.Data.TryGetValue(metric, out StatHistoryRequirements statRequirements))
                {
                    return false;
                }
                return (statRequirements.SampleCounts[sampleRate] > 0)
                    || (statRequirements.DecayConstants.Count > 0 && sampleRate == SampleRate.PerFrame);
            }
            var unrequiredSums = m_Sums.Keys
                .Where(key => !Required(key))
                .ToList();
            foreach (var key in unrequiredSums)
            {
                m_Sums.Remove(key);
            }
            var unrequiredGauges = m_GaugeCounts.Keys
                .Where(key => !Required(key))
                .ToList();
            foreach (var key in unrequiredGauges)
            {
                m_GaugeCounts.Remove(key);
            }
            foreach (var metricId in requirements.Data.Keys)
            {
                if (!Required(metricId))
                {
                    continue;
                }
                if (!m_Sums.ContainsKey(metricId))
                {
                    m_Sums.Add(metricId, 0f);
                }
                if (metricId.MetricKind == MetricKind.Gauge &&
                    !m_GaugeCounts.ContainsKey(metricId))
                {
                    m_GaugeCounts.Add(metricId, 0);
                }
            }
            RequiredMetrics = m_Sums.Keys.ToArray();
        }
    }
}
#endif
