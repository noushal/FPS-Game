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
    class LineGraphRenderer : IGraphRenderer
    {
        const float k_MaxPointsPerPixel = 0.5f;
        public float MaxPointsPerPixel => k_MaxPointsPerPixel;

        GraphBoundsTransformer m_BoundsTransformer;

        RingBuffer<float> m_MaxPointValues;
        float m_MaxPointValue = 0f;
        bool m_MustRecomputeMaxPointValue;

        internal float LineThickness { get; set; }

        public void UpdateConfiguration(DisplayElementConfiguration config)
        {
            LineThickness = config?.GraphConfiguration?.LineGraphConfiguration?.LineThickness ?? 1;
        }

        void ResizeInternalBuffersIfNeeded(in GraphBufferParameters bufferParams)
        {
            var pointCount = bufferParams.GraphWidthPoints;

            if (m_MaxPointValues == null)
            {
                m_MaxPointValues = new RingBuffer<float>(pointCount);
                m_MaxPointValues.Length = pointCount;
            }
            else if (m_MaxPointValues.Capacity != pointCount)
            {
                m_MaxPointValues.Capacity = pointCount;
                m_MaxPointValues.Length = pointCount;
                m_MustRecomputeMaxPointValue = true;
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
            // Example Inputs and Outputs 1
            // - Input points: P0, P1, P2, P3
            // - Output vertices: V00, V01, V10, V11, V20, V21, V30, V31
            //
            //        P0                                                            P3
            //  V00 *  *  * V01                                              V30 *  *  * V31
            //       \     \                                                    /     /
            //        \     \                                                  /     /
            //         \     \                                                /     /
            //          \     \                                              /     /
            //           \     \ V10                                    V20 /     /
            //            \     *------------------------------------------*     /
            //             \  * P1                                        P2 *  /
            //              *--------------------------------------------------*
            //            V11                                                  V21


            // Example Inputs and Outputs 2
            // - Input points: P0, P1, P2, P3
            // - Output vertices: V00, V01, V10, V11, V20, V21, V30, V31, V40, V41
            //
            //                     V10                       V30
            //  V00 *--------------*                           *------------------* V40
            //      * P0      P1 *  \                         /  * P3          P4 *
            //  V01 *----------*     \                       /     *--------------* V41
            //              V11 \     \                     /     / V30
            //                   \     \                   /     /
            //                    \     \                 /     /
            //                     \     \               /     /
            //                      \     \             /     /
            //                       \     \           /     /
            //                        \     \         /     /
            //                         \     \       /     /
            //                          \     \ V20 /     /
            //                           \     \   /     /
            //                            \     \ /     /
            //                             \     *     /
            //                              \         /
            //                               \   P2  /
            //                                \  *  /
            //                                 \   /
            //                                  \ /
            //                                   *
            //                                  V21

            if (bufferParams.GraphWidthPoints < 2)
            {
                return new MinAndMax();
            }
            if (LineThickness <= 0f)
            {
                return new MinAndMax();
            }

            ResizeInternalBuffersIfNeeded(bufferParams);

            var statCount = Math.Min(bufferParams.StatCount, stats.Count);

            var graphWidthPoints = bufferParams.GraphWidthPoints;
            var graphWidthSamples = graphParams.SamplesPerStat;
            var graphSamplesPerPoint = ((float)graphWidthSamples) / graphWidthPoints;
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

            // TODO: These probably need to be member variables
            var sampleValueMin = 0f;

            var xAxisChanged = !xAxisTransform.IsIdentity;
            var yAxisChanged = !yAxisTransform.IsIdentity;
            if (!xAxisChanged && !yAxisChanged && pointsToAdvance <= 0)
            {
                return new MinAndMax { Min = sampleValueMin, Max = m_MaxPointValue };
            }

            UpdateMaxPointValues(
                stats,
                dataSampler,
                graphWidthPoints: graphWidthPoints,
                pointsToAdvance: pointsToAdvance);

            var xScale = (renderBoundsXMax - renderBoundsXMin) / graphWidthPoints;
            var yScale = (renderBoundsYMax - renderBoundsYMin) / yAxisMax;
            var xScaleInverse = 1f / xScale;

            var verticesPerStat = GraphBuffers.k_VerticesPerPoint * graphWidthPoints;

            var pointsToCopy = ComputeNumberOfPointsToCopy(xAxisChanged, yAxisChanged, graphWidthPoints, pointsToAdvance);

            // This terminology is a bit confusing, but:
            // 1. renderBoundsYMin is the render bound corresponding to the minimum value
            // 2. renderBoundsYMinActual is the actual minimum render bound
            var renderBoundsYMinActual = Math.Min(renderBoundsYMin, renderBoundsYMax);
            var renderBoundsYMaxActual = Math.Max(renderBoundsYMin, renderBoundsYMax);

            var halfLineThickness = 0.5f * LineThickness;

            for (var statIndex = 0; statIndex < statCount; ++statIndex)
            {
                var statId = stats[statIndex];
                var statVerticesBegin = statIndex * verticesPerStat;

                var pointValues = dataSampler.PointValues[statId];

                ShiftExistingGeometry(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    xScale: xScale,
                    pointsToAdvance: pointsToAdvance,
                    pointsToCopy: pointsToCopy);

                ComputeNewGeometry(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    yAxisMin: yAxisMin,
                    yAxisMax: yAxisMax,
                    renderBoundsYMin: renderBoundsYMin,
                    graphWidthPoints: graphWidthPoints,
                    xScale: xScale,
                    yScale: yScale,
                    xScaleInverse: xScaleInverse,
                    pointsToCopy: pointsToCopy,
                    renderBoundsYMinActual: renderBoundsYMinActual,
                    renderBoundsYMaxActual: renderBoundsYMaxActual,
                    halfLineThickness: halfLineThickness,
                    pointValues: pointValues);
            }
            return new MinAndMax(sampleValueMin, m_MaxPointValue);
        }

        static int ComputeNumberOfPointsToCopy(bool xAxisChanged, bool yAxisChanged, int graphWidthPoints, int pointsToAdvance)
        {
            // The last point is a special case, as there is no point after it,
            // so ther vertices cannot be computed to line up with the future next point.
            // Therefore it shouldn't be copied, but should be recomputed in the next
            // call when the next point has become available.
            // Using a named constant rather than a magic number, as the rationale for
            // subtracting one would be non-obvious otherwise
            const int k_OffsetToAvoidCopyingLastPoint = 1;

            // Explanation of this ternary:
            // It's surprisingly complicated to rescale a line graph while maintaining
            // a constant thickness, and I haven't thought of a way to do so that's more
            // efficient than just recomputing the geometry for the new scale.
            // So, for the time being, just recompute the geometry when the graph changes size.
            var pointsToCopy = xAxisChanged || yAxisChanged
                ? 0
                : Math.Max(graphWidthPoints - pointsToAdvance - k_OffsetToAvoidCopyingLastPoint, 0);
            return pointsToCopy;
        }

        void UpdateMaxPointValues(
            List<MetricId> stats,
            GraphDataSampler sampler,
            int graphWidthPoints,
            int pointsToAdvance)
        {
            // The largest value dropped during this advance
            float maxValueDropped = 0f;

            // Shift out the values of the old points that have fallen outside the graph
            for (var i = 0; i < pointsToAdvance; ++i)
            {
                maxValueDropped = Math.Max(maxValueDropped, m_MaxPointValues.LeastRecent);
                m_MaxPointValues.PushBack(0);
            }

            var firstNewPointIndex = graphWidthPoints - pointsToAdvance;
            foreach (var stat in stats)
            {
                var pointValues = sampler.PointValues[stat];
                // Account for the new values received
                for (var pointIndex = firstNewPointIndex; pointIndex < graphWidthPoints; ++pointIndex)
                {
                    var pointValue = pointValues[pointIndex];
                    m_MaxPointValues[pointIndex] = Math.Max(m_MaxPointValues[pointIndex], pointValue);
                    m_MaxPointValue = Math.Max(m_MaxPointValue, pointValue);
                }
            }

            if (maxValueDropped >= m_MaxPointValue)
            {
                m_MustRecomputeMaxPointValue = true;
            }

            if (m_MustRecomputeMaxPointValue)
            {
                m_MaxPointValue = m_MaxPointValues.Max();
                m_MustRecomputeMaxPointValue = false;
            }
        }

        void ShiftExistingGeometry(
            Vertex[] vertices,
            int statVerticesBegin,

            float xScale,

            int pointsToAdvance,
            int pointsToCopy)
        {
            var xOffset = pointsToAdvance * xScale;

            var pointIndexToWrite = 0;
            var pointIndexToRead = pointsToAdvance;
            for (; pointIndexToWrite < pointsToCopy; ++pointIndexToWrite, ++pointIndexToRead)
            {
                var vertexToReadBegin  = statVerticesBegin + pointIndexToRead  * GraphBuffers.k_VerticesPerPoint;
                var vertexToWriteBegin = statVerticesBegin + pointIndexToWrite * GraphBuffers.k_VerticesPerPoint;

                ref var p0 = ref vertices[vertexToReadBegin + 0].position;
                ref var p1 = ref vertices[vertexToReadBegin + 1].position;

                var x0 = p0.x - xOffset;
                var y0 = p0.y;
                var x1 = p1.x - xOffset;
                var y1 = p1.y;

                vertices[vertexToWriteBegin + 0].position = new Vector3(x0, y0);
                vertices[vertexToWriteBegin + 1].position = new Vector3(x1, y1);
            }
        }

        void ComputeNewGeometry(
            Vertex[] vertices,
            int statVerticesBegin,
            float yAxisMin,
            float yAxisMax,
            float renderBoundsYMin,
            int graphWidthPoints,
            float xScale,
            float yScale,
            float xScaleInverse,
            int pointsToCopy,
            float renderBoundsYMinActual,
            float renderBoundsYMaxActual,
            float halfLineThickness,
            RingBuffer<float> pointValues)
        {
            // Definitions of names used
            // (some are used only in comments to describe/define other variables)
            //
            // Let Pₙ = (xₙ, yₙ)
            //    - Its index is iₙ
            //    - Its sample index is siₙ
            //    - Its sample value is sₙ
            //
            // Within the currenet iteration:
            //    - Let P0 be the previous point
            //    - Let P1 be the current point
            //    - Let P2 be the next point
            //
            // Let L01 be the line between P0 and P1
            //     - L01(x) = a01 * x + b01
            // Let L12 be the line between P1 and P2
            //     - L12(x) = a12 * x + b12
            //
            // Let vₙa be the vertex above Pₙ, vₙa = (xₙa, yₙa)
            // Let vₙb be the vertex below Pₙ, vₙb = (xₙb, yₙb)
            //
            // Let L01a be the line offset from L01 by half the thickness above (to the left)
            //     - L01a(x) = a01 * x + b01a
            // Let L01b be the line offset from L01 by half the thickness below (to the right)
            //     - L01b(x) = a01 * x + b01b
            //
            // Let L12a be the line offset from L01 by half the thickness above (to the left)
            //     - L12a(x) = a12 * x + b12a
            // Let L12b be the line offset from L01 by half the thickness below (to the right)
            //     - L12b(x) = a12 * x + b12b
            //
            // Let b01_delta be the vertical offset between L01 and L01a
            //     - b01_delta = b01a - b01 = b01 - b01b
            // Let b12_delta be the vertical offset between L12 and L12a
            //     - b12_delta = b12a - b12 = b12 - b12b


            // Precompute information about P1 before the first iteration of the loop
            // ------------------------------------------------------------------------------------
            var i1 = Math.Max(pointsToCopy, 0);  // Index of the current point
            var s1 = Math.Clamp(pointValues[i1], yAxisMin, yAxisMax);
            var x1 = i1 * xScale;                    // x-value of the current point
            var y1 = s1 * yScale + renderBoundsYMin; // y-value of the current point
            float a01;                               // Slope of the line between the current and previous point
            float b01_delta;                         // Vertical offset between L01 and L01a
            float b01a;                              // Vertical offset of L01a
            {
                // Precompute information about P0 and L01 before the first iteration of the loop
                // --------------------------------------------------------------------------------
                var i0 = Math.Max(i1 - 1, 0);
                var s0 = Math.Clamp(pointValues[i0], yAxisMin, yAxisMax);

                var y0 = s0 * yScale + renderBoundsYMin; // y-value of the previous point

                // L01(x) = a01 * x + b01
                a01 = (y1 - y0) * xScaleInverse; //  slope of L01
                var b01 = y1 - a01 * x1;         // offset of L01

                // Vertical offset between L01 and L01a
                b01_delta = halfLineThickness * MathF.Sqrt(1 + a01 * a01);

                // L01a(x) = a01 * x + b01a
                b01a = b01 + b01_delta; // b-value of L01a
            }

            // Need to skip the last point in the loop, and compute it separately.
            // The last point is a special case that can't look ahead.
            const int k_SkipLastPoint = 1;
            for (; i1 < graphWidthPoints - k_SkipLastPoint; ++i1)
            {
                var i2 = i1 + 1; // Index of the next point in the current iteration
                var s2 = Math.Clamp(pointValues[i2], yAxisMin, yAxisMax);
                var x2 = i2 * xScale;
                var y2 = s2 * yScale + renderBoundsYMin;

                var a12 = (y2 - y1) * xScaleInverse; //  slope of L12, according to y = a * x + b
                var b12 = y2 - a12 * x2;             // offset of L12, according to y = a * x + b

                // Vertical offset between L01 and L01a
                var b12_delta = halfLineThickness * MathF.Sqrt(1 + a12 * a12);

                var b12a = b12 + b12_delta; // b-value of L12a

                // x-value of the intersection between L01a and L12a
                var x1a = MathF.Abs(a01 - a12) < 1e-4f
                    ? (x1 - a12 / b12_delta) // Avoid division by zero if a01 ~= a12
                    : (b12a - b01a) / (a01 - a12);

                // y-value of the intersection between L01a and L12a
                var y1a = x1a * a12 + b12a;

                // We can reflect the vertex (x1a, y1a) over the (x1, y1)
                // to find the corresponding vertex (x1b, y1b) without needing
                // to compute the intersection between L01b and L12b directly
                var x1b = 2 * x1 - x1a; // x-value of the intersection between L01b and L12b
                var y1b = 2 * y1 - y1a; // y-value of the intersection between L01b and L12b

                WriteVertices(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    index: i1,
                    renderBoundsYMinActual: renderBoundsYMinActual,
                    renderBoundsYMaxActual: renderBoundsYMaxActual,
                    xa: x1a,
                    ya: y1a,
                    xb: x1b,
                    yb: y1b);

                x1 = x2;
                y1 = y2;
                a01 = a12;
                b01a = b12a;
                b01_delta = b12_delta;
            }

            {
                // Generating vertices for the last point is a special case
                // that must be handled outside of the main loop because
                // there is no next point to connect to.

                var y1a = y1 + b01_delta;
                var y1b = y1 - b01_delta;
                WriteVertices(
                    vertices: vertices,
                    statVerticesBegin: statVerticesBegin,
                    index: i1,
                    renderBoundsYMinActual: renderBoundsYMinActual,
                    renderBoundsYMaxActual: renderBoundsYMaxActual,
                    xa: x1,
                    ya: y1a,
                    xb: x1,
                    yb: y1b);
            }
        }

        void WriteVertices(
            Vertex[] vertices,
            int statVerticesBegin,
            int index,
            float renderBoundsYMinActual,
            float renderBoundsYMaxActual,
            float xa,
            float ya,
            float xb,
            float yb)
        {
            ya = Math.Clamp(ya, renderBoundsYMinActual, renderBoundsYMaxActual);
            yb = Math.Clamp(yb, renderBoundsYMinActual, renderBoundsYMaxActual);
            var p1VerticesBegin = statVerticesBegin + index * GraphBuffers.k_VerticesPerPoint;
            vertices[p1VerticesBegin + 0].position = new Vector3(xa, ya);
            vertices[p1VerticesBegin + 1].position = new Vector3(xb, yb);
        }
    }
}
#endif
