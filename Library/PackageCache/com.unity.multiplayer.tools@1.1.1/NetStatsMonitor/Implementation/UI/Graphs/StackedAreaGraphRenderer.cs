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

using UnityEngine;
using UnityEngine.UIElements;

using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    class StackedAreaGraphRenderer : IGraphRenderer
    {
        const float k_MaxPointsPerPixel = 1.0f;
        public float MaxPointsPerPixel => k_MaxPointsPerPixel;

        /// Sums used to "stack" the graphs
        /// Stored as a member variable rather than a local,
        /// so the buffer can be continually reused without
        /// any per-frame allocations
        RingBuffer<float> m_PointSums;

        GraphBoundsTransformer m_BoundsTransformer;

        float m_PointValueMax;

        void ResizePointSumsIfNeeded(in GraphBufferParameters bufferParams)
        {
            var pointCount = bufferParams.GraphWidthPoints;
            if (m_PointSums == null)
            {
                m_PointSums = new RingBuffer<float>(pointCount);
                m_PointSums.Length = pointCount;
            }
            else if (m_PointSums.Capacity != pointCount)
            {
                m_PointSums.Capacity = pointCount;
                m_PointSums.Length = pointCount;
            }
        }

        public MinAndMax UpdateVertices(
            List<MetricId> stats,
            GraphDataSampler dataSampler,
            int pointsToAdvance,
            float yAxisMin,
            float yAxisMax,
            in GraphParameters graphParams,
            in GraphBufferParameters bufferParams,
            float renderBoundsXMin,
            float renderBoundsXMax,
            float renderBoundsYMin,
            float renderBoundsYMax,
            Vertex[] vertices)
        {
            ResizePointSumsIfNeeded(bufferParams);

            var statCount = Math.Min(bufferParams.StatCount, stats.Count);
            var pointsPerStat = bufferParams.GraphWidthPoints;

            var xScale = (renderBoundsXMax - renderBoundsXMin) / (pointsPerStat - 1);
            var yScale = (renderBoundsYMax - renderBoundsYMin) / yAxisMax;

            var verticesPerStat = GraphBuffers.k_VerticesPerPoint * pointsPerStat;

            var graphWidthSamples = graphParams.SamplesPerStat;
            var graphSamplesPerPoint = ((float)graphWidthSamples) / pointsPerStat;
            var graphPointsPerSample = 1 / graphSamplesPerPoint;

            m_BoundsTransformer ??= new GraphBoundsTransformer(
                renderBoundsXMin, renderBoundsXMax, renderBoundsYMin, renderBoundsYMax,
                yAxisMin, yAxisMax);

            // X and Y axis transforms that must be applied to existing geometry
            // These transforms will be Identity if no transformation of the
            // existing geometry is required  along this axis
            var (xAxisTransform, yAxisTransform) = m_BoundsTransformer.ComputeTransformsForNewBounds(
                renderBoundsXMin, renderBoundsXMax, renderBoundsYMin, renderBoundsYMax,
                yAxisMin, yAxisMax,
                pointsToAdvance);

            if (pointsToAdvance <= 0)
            {
                // Rescale the existing geometry if needed.
                // This may be required if the render bounds or max graph value has changed.
                // No other changes are needed, as the graph has not advanced.
                RescaleExistingGeometryIfNeeded(xAxisTransform, yAxisTransform, vertices);
                return new MinAndMax { Min = 0f, Max = m_PointValueMax };
            }

            ShiftExistingPointSums(pointsToAdvance: pointsToAdvance);

            var pointsToCopy = Math.Max(pointsPerStat - pointsToAdvance, 0);

            for (var statIndex = 0; statIndex < statCount; ++statIndex)
            {
                var statId = stats[statIndex];
                var pointValues = dataSampler.PointValues[statId];
                var statVerticesBegin = statIndex * verticesPerStat;

                ShiftExistingGeometryAndRescaleIfNeeded(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    yAxisTransform: yAxisTransform,
                    xScale: xScale,
                    renderBoundsXMin: renderBoundsXMin,
                    pointsToCopy: pointsToCopy,
                    pointsToAdvance: pointsToAdvance);

                ComputeNewGeometry(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    yAxisMax: yAxisMax,
                    renderBoundsXMin: renderBoundsXMin,
                    renderBoundsYMin: renderBoundsYMin,
                    pointsPerStat: pointsPerStat,
                    xScale: xScale,
                    yScale: yScale,
                    pointsToCopy: pointsToCopy,
                    pointValues: pointValues
                );
            }

            return new MinAndMax { Min = 0f, Max = m_PointValueMax };
        }

        /// Rescale the existing geometry in place if needed,
        /// in the case that no points have been advanced.
        void RescaleExistingGeometryIfNeeded(
            LinearTransform xAxisTransform,
            LinearTransform yAxisTransform,
            Vertex[] vertices)
        {
            var xAxisChanged = !xAxisTransform.IsIdentity;
            var yAxisChanged = !yAxisTransform.IsIdentity;

            var vertexCount = vertices.Length;

            // As much as I would like to avoid the duplication between these branches,
            // the duplication is necessary to avoid branching in the loop
            if (xAxisChanged && yAxisChanged)
            {
                for (var vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
                {
                    var position = vertices[vertexIndex].position;
                    position.x = xAxisTransform.Apply(position.x);
                    position.y = yAxisTransform.Apply(position.y);
                    vertices[vertexIndex].position = position;
                }
            }
            else if (xAxisChanged)
            {
                for (var vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
                {
                    var position = vertices[vertexIndex].position;
                    position.x = xAxisTransform.Apply(position.x);
                    vertices[vertexIndex].position = position;
                }
            }
            else if (yAxisChanged)
            {
                for (var vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
                {
                    var position = vertices[vertexIndex].position;
                    position.y = yAxisTransform.Apply(position.y);
                    vertices[vertexIndex].position = position;
                }
            }
        }

        void ShiftExistingPointSums(int pointsToAdvance)
        {
            var mustRecomputeMaxPointValue = false;
            for (var pointIndex = 0; pointIndex < pointsToAdvance; ++pointIndex)
            {
                if (m_PointSums[0] >= m_PointValueMax - float.Epsilon)
                {
                    // The previous maximum value is being shifted out,
                    // and we need to recompute the maximum for the graph
                    // bounds
                    mustRecomputeMaxPointValue = true;
                }
                m_PointSums.PushBack(0);
            }
            if (mustRecomputeMaxPointValue)
            {
                m_PointValueMax = m_PointSums.Max();
            }
        }

        static void ShiftExistingGeometryAndRescaleIfNeeded(
            Vertex[] vertices,
            int statVerticesBegin,
            LinearTransform yAxisTransform,
            float xScale,
            float renderBoundsXMin,
            int pointsToCopy,
            int pointsToAdvance)
        {
            var yAxisChanged = !yAxisTransform.IsIdentity;

            // As much as I would like to avoid the duplication between these branches,
            // the duplication is necessary to avoid branching in the loop
            if (yAxisChanged)
            {
                for (var pointIndex = 0; pointIndex < pointsToCopy; ++pointIndex)
                {
                    var xValue = pointIndex * xScale + renderBoundsXMin;
                    var pointIndexToCopyFrom = pointIndex + pointsToAdvance;

                    var vertexToCopyFromBegin =
                        statVerticesBegin + pointIndexToCopyFrom * GraphBuffers.k_VerticesPerPoint;

                    var yValueBelow = yAxisTransform.Apply(vertices[vertexToCopyFromBegin + 0].position.y);
                    var yValueAbove = yAxisTransform.Apply(vertices[vertexToCopyFromBegin + 1].position.y);

                    var pointVerticesBegin = statVerticesBegin + pointIndex * GraphBuffers.k_VerticesPerPoint;
                    vertices[pointVerticesBegin + 0].position = new Vector3(xValue, yValueBelow);
                    vertices[pointVerticesBegin + 1].position = new Vector3(xValue, yValueAbove);
                }
            }
            else
            {
                for (var pointIndex = 0; pointIndex < pointsToCopy; ++pointIndex)
                {
                    var xValue = pointIndex * xScale + renderBoundsXMin;
                    var pointIndexToCopyFrom = pointIndex + pointsToAdvance;

                    var vertexToCopyFromBegin =
                        statVerticesBegin + pointIndexToCopyFrom * GraphBuffers.k_VerticesPerPoint;

                    var yValueBelow = vertices[vertexToCopyFromBegin + 0].position.y;
                    var yValueAbove = vertices[vertexToCopyFromBegin + 1].position.y;

                    var pointVerticesBegin = statVerticesBegin + pointIndex * GraphBuffers.k_VerticesPerPoint;
                    vertices[pointVerticesBegin + 0].position = new Vector3(xValue, yValueBelow);
                    vertices[pointVerticesBegin + 1].position = new Vector3(xValue, yValueAbove);
                }
            }
        }

        void ComputeNewGeometry(
            Vertex[] vertices,
            int statVerticesBegin,

            float yAxisMax,

            float renderBoundsXMin,
            float renderBoundsYMin,

            int pointsPerStat,

            float xScale,
            float yScale,

            int pointsToCopy,
            RingBuffer<float> pointValues)
        {
            for (var pointIndex = pointsToCopy; pointIndex < pointsPerStat; ++pointIndex)
            {
                var pointValue = pointValues[pointIndex];

                var prevSum = m_PointSums[pointIndex];
                var nextSumUnclamped = prevSum + pointValue;

                m_PointValueMax = MathF.Max(nextSumUnclamped, m_PointValueMax);

                // Clamp the sum to avoid drawing off of the plot
                var nextSum = MathF.Min(nextSumUnclamped, yAxisMax);

                m_PointSums[pointIndex] = nextSumUnclamped;

                // Consider using FMA, does it make a difference?
                var xValue = pointIndex * xScale + renderBoundsXMin;
                var yValueBelow = prevSum * yScale + renderBoundsYMin;
                var yValueAbove = nextSum * yScale + renderBoundsYMin;

                var pointVerticesBegin = statVerticesBegin + GraphBuffers.k_VerticesPerPoint * pointIndex;
                vertices[pointVerticesBegin + 1].position = new Vector3(xValue, yValueAbove);
                vertices[pointVerticesBegin + 0].position = new Vector3(xValue, yValueBelow);
            }
        }
    }
}
#endif
