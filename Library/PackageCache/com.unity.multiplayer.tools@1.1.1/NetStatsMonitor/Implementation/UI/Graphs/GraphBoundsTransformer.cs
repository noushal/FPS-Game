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
    /// Computes the transforms that must be applied to existing graph geometry
    /// in response to changes in graph bounds, scale, and the scrolling movement of
    /// the graph as new points are added
    class GraphBoundsTransformer
    {
        /// The minimum x-axis render bound of the graph in pixels
        float m_BoundsXMin;
        /// The maximum x-axis render bound of the graph in pixels
        float m_BoundsXMax;
        /// The minimum y-axis render bound of the graph in pixels
        float m_BoundsYMin;
        /// The maximum y-axis render bound of the graph in pixels
        float m_BoundsYMax;

        /// The minimum y-value of the graph in reported units such as ms or B/s
        float m_YValueMin;
        /// The maximum y-value of the graph in reported units such as ms or B/s
        float m_YValueMax;

        // We may need to know the graph width in points

        public GraphBoundsTransformer(
            float boundsXMin,
            float boundsXMax,
            float boundsYMin,
            float boundsYMax,
            float yValueMin,
            float yValueMax)
        {
            m_BoundsXMin = boundsXMin;
            m_BoundsXMax = boundsXMax;
            m_BoundsYMin = boundsYMin;
            m_BoundsYMax = boundsYMax;
            m_YValueMin = yValueMin;
            m_YValueMax = yValueMax;
        }

        LinearTransform ComputeXAxisTransform(
            float newRenderBoundsXMin,
            float newRenderBoundsXMax,
            int pointsToAdvance)
        {
            return LinearTransform.Identity;
        }

        LinearTransform ComputeYAxisTransform(
            float newBoundsYMin,
            float newBoundsYMax,
            float newYValueMin,
            float newYValueMax)
        {
            if (newBoundsYMin == m_BoundsYMin &&
                newBoundsYMax == m_BoundsYMax &&
                newYValueMin == m_YValueMin &&
                newYValueMax == m_YValueMax)
            {
                // Return identity exactly in this case, as otherwise the
                // floating point error from the full computation will produce
                // results that are not quite Identity
                return LinearTransform.Identity;
            }

            var newRenderHeight = newBoundsYMax - newBoundsYMin;
            var oldRenderHeight = m_BoundsYMax - m_BoundsYMin;
            var newGraphRange = newYValueMax - newYValueMin;
            var oldGraphRange = m_YValueMax - m_YValueMin;

            // The explanatory equations below use the following notation:
            // Px is a value in pixels, such as the new or old render bounds
            // y  is the y value of the variable (with units like ms or MB), and there is no new y value
            // N  is a normalized position within the graph from [0, 1]
            // '  denotes a new value, so Px is the old position in pixels and Px' is the new position
            // ₋  (subscript minus) denotes a minimum value, for example Px₋' is the new minimum graph bound
            // ₊  (subscript plus)  denotes a maximum value, for example Px₊' is the new maximum graph bound
            // Δ  is a range. For example, ΔPx is the height of the graph in pixels

            // The process of mapping Px to Px' would look like:
            // 1. N   = (Px - Px₋) / ΔPx         Equation for previous normalized value
            // 2. Y   =  N  * ΔY   + Y₋          Equation for y-value
            // 3. N'  = (Y  - Y₋') / ΔY'         Equation for new normalized value
            // 3. Px' =  N' * ΔPx' + Px₋'        Equation for new y-position in pixels

            // These can be combined into a single equation mapping Px to Px':
            // 4. Px' = ((((Px - Px₋) / ΔPx) * ΔY + Y₋ - Y₋') / ΔY') * ΔPx' + Px₋'

            // Which can then be simplified into a linear transformation in the form y = x * a + b
            // 5. Px' = Px * (ΔY / ΔPx) * (ΔPx' / ΔY') + ((-Px₋ * (ΔY / ΔPx) + Y₋ - Y₋') * (ΔPx' /  ΔY') + Px₋'
            // 6.   a = (ΔY / ΔPx) * (ΔPx' / ΔY'),                             where a is the multiplier
            // 7.   b = ((-Px₋ * (ΔY / ΔPx) + Y₋ - Y₋') * (ΔPx' /  ΔY') + Px₋', where b is the additive constant

            var oldYValuesPerPixel = oldGraphRange   / oldRenderHeight;
            var newPixelsPerYValue = newRenderHeight / newGraphRange;

            var multiplier = oldYValuesPerPixel * newPixelsPerYValue;
            var offset     = -m_BoundsYMin * multiplier + (m_YValueMin - newYValueMin) * newPixelsPerYValue + newBoundsYMin;

            m_BoundsYMin = newBoundsYMin;
            m_BoundsYMax = newBoundsYMax;
            m_YValueMin = newYValueMin;
            m_YValueMax = newYValueMax;

            return new LinearTransform { A = multiplier, B = offset };
        }

        /// Computes the transforms that must be applied to existing graph geometry
        /// in response to changes in graph bounds, scale, and the scrolling movement
        /// of the graph as new points are added
        public (LinearTransform transformX, LinearTransform transformY)
            ComputeTransformsForNewBounds(
                float newBoundsXMin,
                float newBoundsXMax,
                float newBoundsYMin,
                float newBoundsYMax,
                float newYAxisMin,
                float newYAxisMax,
                int pointsToAdvance)
        {
            return (
                ComputeXAxisTransform(newBoundsXMin, newBoundsXMax, pointsToAdvance),
                ComputeYAxisTransform(newBoundsYMin, newBoundsYMax, newYAxisMin, newYAxisMax)
            );
        }
    }
}
#endif