// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System.Collections.Generic;

using UnityEngine.UIElements;

using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// A graph renderer is responsible for writing to the vertex buffer to render the correct kind of graph
    interface IGraphRenderer
    {
        /// Updates the vertices with the geometry needed to render the graph.
        /// Returns the true min and max value displayed on the graph, for use
        /// in dynamic scaling of the graph.
        /// This returning of the true min and max from UpdateVertices is for
        /// the purpose of efficiency, as it avoids traversing the data twice:
        /// once to determine the min and max and a second time to render.
        /// It does however mean that updates of the graph bounds in response
        /// to a new min or max are delayed by one frame.
        /// <param name="stats"> The stats to be plotted.</param>
        /// <param name="dataSampler"> Sample data for each point in the graph.</param>
        /// <param name="pointsToAdvance">
        /// The number of points to advance this frame (also the number of new samples received).
        /// </param>
        /// <param name="graphParams"> Parameters of the graph.</param>
        /// <param name="bufferParams"> Parameters of the graph buffers.</param>
        /// <param name="yAxisMin"> The minimum y-value within the graph bounds.</param>
        /// <param name="yAxisMax"> The maximum y-value within the graph bounds.</param>
        /// <param name="renderBoundsXMin"> The render bound corresponding to the minimum x value</param>
        /// <param name="renderBoundsXMax"> The render bound corresponding to the maximum x value</param>
        /// <param name="renderBoundsYMin">
        /// The render bound corresponding to the minimum y value.
        /// Note that this is not necessarily the minimum value of the two render bounds.
        /// </param>
        /// <param name="renderBoundsYMax">
        /// The render bound corresponding to the maximum y value.
        /// Note that this is not necessarily the maximum value of the two render bounds.
        /// </param>
        /// <param name="vertices"> The vertices to be written to. </param>
        MinAndMax UpdateVertices(
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
            Vertex[] vertices);

        float MaxPointsPerPixel { get; }

        void UpdateConfiguration(DisplayElementConfiguration config){}
    }
}
#endif
