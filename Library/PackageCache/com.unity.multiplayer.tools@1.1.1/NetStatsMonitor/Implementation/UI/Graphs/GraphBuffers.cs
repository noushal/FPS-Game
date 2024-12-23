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
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation.Graphing;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    class GraphBuffers
    {
        /// One vertex below each point and one vertex above
        public const int k_VerticesPerPoint = 2;
        const int k_TrisPerLine = 2;
        const int k_IndicesPerTri = 3;
        const int k_IndicesPerLineSegment = k_TrisPerLine * k_IndicesPerTri;

        public Vertex[] Vertices { get; private set; }
        ushort[] Indices { get; set; }

        public GraphBufferParameters Parameters { get; private set; }

        int m_VariableColorsHash;

        static int ComputeColorsHash(Color[] colors)
        {
            int hash = 0;
            for (int i = 0; i < colors.Length; ++i)
            {
                var colorHash = colors[i].GetHashCode();
                hash = HashCode.Combine(hash, colorHash);
            }
            return hash;
        }

        /// Updating may include:
        /// 1. Resizing/allocating buffers
        /// 2. Recomputing index buffer values
        /// 3. Recomputing vertex color buffer values
        public void UpdateIfNeeded(
            in GraphBufferParameters newParams,
            in Color[] variableColors)
        {
            var pointCount = newParams.StatCount * newParams.GraphWidthPoints;
            var vertexCount = k_VerticesPerPoint * pointCount;

            var linesSegmentsPerStat = Math.Max(0, newParams.GraphWidthPoints - 1);
            var lineSegmentCount = linesSegmentsPerStat * newParams.StatCount;
            var indexCount = k_IndicesPerLineSegment * lineSegmentCount;

            if ((Vertices?.Length ?? 0) != vertexCount)
            {
                Vertices = new Vertex[vertexCount];
            }
            if ((Indices?.Length ?? 0) != indexCount)
            {
                Indices = new ushort[indexCount];
            }

            var bufferParamsChanged =
                newParams.StatCount != Parameters.StatCount ||
                newParams.GraphWidthPoints != Parameters.GraphWidthPoints;
            if (bufferParamsChanged)
            {
                ComputeIndices(newParams.StatCount, newParams.GraphWidthPoints);
            }
            Parameters = newParams;

            var newColorsHash = ComputeColorsHash(variableColors);
            var colorsChanged = newColorsHash != m_VariableColorsHash;
            if (bufferParamsChanged || colorsChanged)
            {
                // Since the configuration has changed, it's possible the colors have changed
                SetVertexColors(newParams.StatCount, newParams.GraphWidthPoints, variableColors);
            }
            m_VariableColorsHash = newColorsHash;
        }

        void ComputeIndices(int statCount, int pointsPerStat)
        {
            var lineSegmentsPerStat = Math.Max(0, pointsPerStat - 1);
            var indicesPerStat = k_IndicesPerLineSegment * lineSegmentsPerStat;

            var verticesPerStat = k_VerticesPerPoint * pointsPerStat;

            for (var statIndex = 0; statIndex < statCount; ++statIndex)
            {
                var statIndicesBegin = statIndex * indicesPerStat;
                var statVerticesBegin = statIndex * verticesPerStat;
                for (var lineSegmentIndex = 0; lineSegmentIndex < lineSegmentsPerStat; ++lineSegmentIndex)
                {
                    var lineSegmentIndicesBegin = statIndicesBegin + k_IndicesPerLineSegment * lineSegmentIndex;
                    var lineSegmentVerticesBegin = statVerticesBegin + k_VerticesPerPoint * lineSegmentIndex;

                    // First tri
                    // V0 - V2
                    //  | /
                    // V1   V3
                    Indices[lineSegmentIndicesBegin + 0] = (ushort)(lineSegmentVerticesBegin + 0);
                    Indices[lineSegmentIndicesBegin + 1] = (ushort)(lineSegmentVerticesBegin + 1);
                    Indices[lineSegmentIndicesBegin + 2] = (ushort)(lineSegmentVerticesBegin + 2);

                    // Second tri
                    // V0   V2
                    //    / |
                    // V1 - V3
                    Indices[lineSegmentIndicesBegin + 3] = (ushort)(lineSegmentVerticesBegin + 3);
                    Indices[lineSegmentIndicesBegin + 4] = (ushort)(lineSegmentVerticesBegin + 2);
                    Indices[lineSegmentIndicesBegin + 5] = (ushort)(lineSegmentVerticesBegin + 1);
                }
            }
        }

        void SetVertexColors(int statCount, int pointsPerStat, Color[] variableColors)
        {
            var verticesPerStat = k_VerticesPerPoint * pointsPerStat;
            for (var statIndex = 0; statIndex < statCount; ++statIndex)
            {
                var statVerticesBegin = statIndex * verticesPerStat;
                Color32 statColor = (variableColors != null && statIndex < variableColors.Length)
                    ? variableColors[statIndex]
                    : GraphColorUtils.GetColorForIndex(statIndex, statCount);
                for (var vertexIndex = 0; vertexIndex < verticesPerStat; ++vertexIndex)
                {
                    var vertexIndexAbsolute = statVerticesBegin + vertexIndex;
                    Vertices[vertexIndexAbsolute].tint = statColor;
                }
            }
        }

        public void WriteToMeshGenerationContext(MeshGenerationContext mgc)
        {
            if (Vertices == null ||
                Indices == null ||
                Vertices.Length == 0 ||
                Indices.Length == 0)
            {
                // This can occur if a graph is configured without any stats, samples, or space on screen
                // in which case the buffers are not allocated
                return;
            }
            MeshWriteData mwd = mgc.Allocate(Vertices.Length, Indices.Length);
            mwd.SetAllVertices(Vertices);
            mwd.SetAllIndices(Indices);
        }
    }
}
#endif
