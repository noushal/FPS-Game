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
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation.Graphing;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    public class GraphLegend : VisualElement
    {
        List<LegendKey> m_LegendKeys = new List<LegendKey>();
        public GraphLegend()
        {
            AddToClassList(UssClassNames.k_GraphLegend);
        }

        public void UpdateConfiguration(DisplayElementConfiguration configuration)
        {
            var stats = configuration.Stats;

            if (stats.Count < m_LegendKeys.Count)
            {
                //Delete the extra legends
                var diff = m_LegendKeys.Count - stats.Count;
                for (var i = m_LegendKeys.Count-1; m_LegendKeys.Count != stats.Count; --i)
                {
                    RemoveAt(i);
                    m_LegendKeys.RemoveAt(i);
                }
            }

            m_LegendKeys.Resize(stats.Count, () => new LegendKey());
            var childrenCount = Children().Count();
            var variableColors = configuration.GraphConfiguration.VariableColors;
            for (var i = 0; i < stats.Count; ++i)
            {
                var legendKey = m_LegendKeys[i];
                var stat = stats[i];
                Color32 color = (variableColors != null && i < variableColors.Count)
                    ? variableColors[i]
                    : GraphColorUtils.GetColorForIndex(i, stats.Count);
                legendKey.Update(stat.ToString(), color);

                if (i >= childrenCount)
                {
                    Add(legendKey);
                }
            }
        }
    }
}
#endif
