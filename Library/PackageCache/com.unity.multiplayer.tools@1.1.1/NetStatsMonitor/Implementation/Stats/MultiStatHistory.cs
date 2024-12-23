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
using UnityEngine.Assertions;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// A class for storing multiple RNSM stat histories over multiple frames,
    /// to facilitate moving averages and graphs over time
    class MultiStatHistory
    {
        [NotNull]
        readonly Dictionary<MetricId, StatHistory> m_Data = new();

        [NotNull]
        public IReadOnlyDictionary<MetricId, StatHistory> Data => m_Data;

        /// Record sample time-stamps for graphs and counters, so that
        /// we can gracefully handle irregularly-timed metric dispatches.
        [NotNull]
        public EnumMap<SampleRate, RingBuffer<double>> TimeStamps { get; } = new EnumMap<SampleRate, RingBuffer<double>>()
        {
            { SampleRate.PerFrame,  new(0) },
            { SampleRate.PerSecond, new(0) },
        };

        public void Clear()
        {
            m_Data.Clear();
        }

        public void Collect(SampleRate rate, StatsAccumulator statsAccumulator, double time)
        {
            TimeStamps[rate].PushBack(time);
            foreach (var metricId in statsAccumulator.RequiredMetrics)
            {
                var history = m_Data[metricId];
                Assert.IsNotNull(history);
                var collectedValue = statsAccumulator.Collect(metricId);
                history.Update(metricId, rate, collectedValue, time);
            }
            statsAccumulator.LastCollectionTime = time;
        }

        /// Updates the requirements, while preserving all existing data that is still required.
        internal void UpdateRequirements(MultiStatHistoryRequirements requirements)
        {
            var allStatRequirements = requirements.Data;

            // Remove existing data that is no longer required
            var statsToRemove = m_Data.Keys
                .Where(metricId => !allStatRequirements.ContainsKey(metricId))
                .ToList();
            foreach (var statName in statsToRemove)
            {
                m_Data.Remove(statName);
            }

            // Add and update stats according to the requirements
            var maxSampleCounts = new EnumMap<SampleRate, int>(0);
            foreach (var (statName, statRequirements) in allStatRequirements)
            {
                for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
                {
                    maxSampleCounts[rate] = Math.Max(maxSampleCounts[rate], statRequirements.SampleCounts[rate]);
                }
                if (m_Data.ContainsKey(statName))
                {
                    m_Data[statName].UpdateRequirements(statRequirements);
                }
                else
                {
                    m_Data[statName] = new StatHistory(statRequirements);
                }
            }

            // Include a surplus of one timestamp so that the duration of the oldest sample
            // can still be computed
            const int k_TimeStampSurplus = 1;

            for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
            {
                TimeStamps[rate].Capacity = maxSampleCounts[rate] + k_TimeStampSurplus;
                for (int i = 0; i < k_TimeStampSurplus; ++i)
                {
                    TimeStamps[rate].PushBack(0);
                }
            }
        }

        double? GetSimpleMovingAverageForCounter(
            MetricId metricId,
            SampleRate sampleRate,
            int maxSampleCount,
            double time)
        {
            if (!Data.TryGetValue(metricId, out StatHistory statHistory))
            {
                return null;
            }

            var samples = statHistory.SampleBuffers[sampleRate];
            var sampleCount = Math.Min(maxSampleCount, samples.Length);
            if (sampleCount <= 1)
            {
                return null;
            }
            var sampleSum = samples.SumLastN(sampleCount);

            var timeSpan = TimeSpanOfLastNSamples(sampleRate, sampleCount);

            var rate = sampleSum / timeSpan;
            return rate;
        }

        double? GetSimpleMovingAverageForGauge(
            MetricId metricId,
            SampleRate sampleRate,
            int maxSampleCount)
        {
            if (!Data.TryGetValue(metricId, out StatHistory statHistory))
            {
                return null;
            }

            var samples = statHistory.SampleBuffers[sampleRate];

            var sampleCount = Math.Min(maxSampleCount, samples.Length);
            if (sampleCount <= 0)
            {
                return null;
            }
            var sampleSum = samples.SumLastN(sampleCount);

            var average = sampleSum / sampleCount;
            return average;
        }

        public double? GetSimpleMovingAverage(
            MetricId metricId,
            SampleRate sampleRate,
            int maxSampleCount,
            double time)
        {
            var metricKind = metricId.MetricKind;
            switch (metricKind)
            {
                case MetricKind.Counter:
                    return GetSimpleMovingAverageForCounter(metricId, sampleRate, maxSampleCount, time);
                case MetricKind.Gauge:
                    return GetSimpleMovingAverageForGauge(metricId, sampleRate, maxSampleCount);
                default:
                    throw new NotSupportedException($"Unhandled {nameof(MetricKind)} {metricKind}");
            }
        }

        /// The length of the history in seconds
        internal double TimeSpanOfLastNSamples(SampleRate sampleRate, int sampleCount)
        {
            var timeStamps = TimeStamps[sampleRate];

            var validSampleCount = Math.Min(sampleCount + 1, timeStamps.Length);
            if (validSampleCount <= 1)
            {
                return 0;
            }

            var firstTimeStamp = timeStamps[^validSampleCount];
            var lastTimeStamp = timeStamps.MostRecent;
            return lastTimeStamp - firstTimeStamp;
        }
    }
}
#endif