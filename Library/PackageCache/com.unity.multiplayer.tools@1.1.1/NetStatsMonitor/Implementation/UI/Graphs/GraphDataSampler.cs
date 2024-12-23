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
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// Computes the values of points on the graph, which may straddle
    /// an arbitrary number of samples in the history of a stat
    class GraphDataSampler
    {
        readonly Dictionary<MetricId, RingBuffer<float>> m_PointValues = new();

        /// A ring-buffer for each stat in the graph, with one sample for each point in the graph.
        /// In LineGraphs, these samples correspond to the vertical midpoint between each pair of vertices.
        /// In StackedAreaGraphs, these samples correspond to the vertical offset between this stat and the
        /// previous stat at this point.
        public IReadOnlyDictionary<MetricId, RingBuffer<float>> PointValues => m_PointValues;


        public void UpdateConfiguration(List<MetricId> stats)
        {
            var unrequiredStats = m_PointValues.Keys
                .Where(key => !stats.Contains(key))
                .ToList();
            foreach (var stat in unrequiredStats)
            {
                m_PointValues.Remove(stat);
            }
            foreach (var stat in stats)
            {
                if (!m_PointValues.ContainsKey(stat))
                {
                    // Initial capacity is zero; the buffer will be allocated in ResizeBuffersIfNeeded
                    // at which point we have more information about how much data needs to be stored.
                    // The motivation for this is that often when UpdateConfiguration is called during
                    // the first frame, we don't yet know the bounds of the graph in the UI, and thus
                    // don't know how many points need to be drawn and how many samples need to be stored.
                    const int k_InitialCapacity = 0;
                    m_PointValues.Add(stat, new RingBuffer<float>(k_InitialCapacity));
                }
            }
        }

        public void ResizeBuffersIfNeeded(
            in GraphBufferParameters bufferParams)
        {
            var graphWidthPoints = bufferParams.GraphWidthPoints;
            foreach (var buffer in m_PointValues.Values)
            {
                buffer.Capacity = graphWidthPoints;
                buffer.Length = graphWidthPoints;
            }
        }

        public void SampleNewPoints(
            MultiStatHistory history,
            List<MetricId> stats,
            SampleRate rate,
            int graphWidthPoints,
            int graphWidthSamples,
            float graphSamplesPerPoint,
            int pointsToAdvance)
        {
            if (pointsToAdvance <= 0)
            {
                return;
            }
            var timeStamps = history.TimeStamps[rate];
            foreach (var stat in stats)
            {
                var statData = history.Data[stat].SampleBuffers[rate];

                var sampleCount = statData.Length;

                var pointValues = m_PointValues[stat];

                // initialSampleIndex is the sample index corresponding to the 0th or left-most point in the graph
                // 1. It will be positive if there are excess samples outside the graph that will be skipped
                // 2. It will be negative if there are not enough samples to fill the graph
                // 3. It will be zero if there are exactly the right number of samples to fill the graph
                var initialSampleIndex = sampleCount - graphWidthSamples;

                var initialPointIndex = Math.Max(graphWidthPoints - pointsToAdvance, 0);

                var metricKind = stat.MetricKind;

                var sampleIndex = initialSampleIndex + initialPointIndex * graphSamplesPerPoint;

                switch (metricKind)
                {
                    case MetricKind.Counter:
                    {
                        var timeStampOffset = timeStamps.Length - sampleCount;
                        for (var pointIndex = initialPointIndex; pointIndex < graphWidthPoints; ++pointIndex)
                        {
                            var pointValue = SampleCounter(
                                timeStamps: timeStamps,
                                timeStampOffset: timeStampOffset,
                                statData: statData,
                                sampleCount: sampleCount,
                                graphSamplesPerPoint: graphSamplesPerPoint,
                                sampleIndex: ref sampleIndex);
                            pointValues.PushBack(pointValue);
                        }
                        break;
                    }
                    case MetricKind.Gauge:
                    {
                        for (var pointIndex = initialPointIndex; pointIndex < graphWidthPoints; ++pointIndex)
                        {
                            var pointValue = SampleGauge(
                                statData: statData,
                                sampleCount: sampleCount,
                                graphSamplesPerPoint: graphSamplesPerPoint,
                                sampleIndex: ref sampleIndex);
                            pointValues.PushBack(pointValue);
                        }
                        break;
                    }
                }
            }
        }

        public static float SampleCounter(
            RingBuffer<double> timeStamps,
            int timeStampOffset,
            RingBuffer<float> statData,
            int sampleCount,
            float graphSamplesPerPoint,
            ref float sampleIndex)
        {
            // Sum the statData on the continuous interval [a, b],
            // and divide it by the duration timeB - timeA

            var a = Math.Clamp(sampleIndex, 0f, sampleCount);
            var b = Math.Clamp(sampleIndex + graphSamplesPerPoint, 0f, sampleCount);
            if (b <= a)
            {
                return 0f;
            }

            var indexA = (int)Math.Floor(a);
            var indexB = (int)Math.Ceiling(b) - 1;

            var remainderA = a - indexA;
            var remainderB = b - indexB;

            var timeA =
                (1 - remainderA) * timeStamps[timeStampOffset + indexA - 1] +
                remainderA       * timeStamps[timeStampOffset + indexA];

            var timeB =
                (1 - remainderB) * timeStamps[timeStampOffset + indexB - 1] +
                remainderB       * timeStamps[timeStampOffset + indexB];

            var fractionOfSampleA = Math.Min(indexA + 1, b) - a;

            // 1. Add the fraction of the first sample of this point
            var sum = fractionOfSampleA * statData[indexA];

            // 2. Add all samples that fall completely within this point
            for (int i = indexA + 1; i < indexB; ++i)
            {
                sum += statData[i];
            }

            // 3. Add the fraction of the last sample of this point
            if (indexB > indexA)
            {
                sum += remainderB * statData[indexB];
            }

            // The value of the point is the sum of the samples divided by the duration
            var duration = timeB - timeA;
            var pointValue = sum / (float)duration;
            sampleIndex = b;
            return pointValue;
        }

        public static float SampleGauge(
            RingBuffer<float> statData,
            int sampleCount,
            float graphSamplesPerPoint,
            ref float sampleIndex)
        {
            // Sum the statData on the continuous interval [a, b],
            // and divide it by b - a

            var a = Math.Clamp(sampleIndex, 0f, sampleCount);
            var b = Math.Clamp(sampleIndex + graphSamplesPerPoint, 0f, sampleCount);
            if (b <= a)
            {
                return 0f;
            }

            var indexA = (int)Math.Floor(a);
            var indexB = (int)Math.Ceiling(b) - 1;

            var fractionOfSampleA = Math.Min(indexA + 1, b) - a;

            // 1. Add the fraction of the first partial sample of this point
            var sum = fractionOfSampleA * statData[indexA];

            // 2. Add all samples that fall completely within this point
            for (int i = indexA + 1; i < indexB; ++i)
            {
                sum += statData[i];
            }

            // 3. Add the fraction of the last partial sample of this point
            if (indexB > indexA)
            {
                var fractionOfSampleB = b - indexB;
                sum += fractionOfSampleB * statData[indexB];
            }

            // Normalize the point value to the average of the samples
            var pointValue = sum / (b - a);
            sampleIndex = b;
            return pointValue;
        }
    }
}
#endif
