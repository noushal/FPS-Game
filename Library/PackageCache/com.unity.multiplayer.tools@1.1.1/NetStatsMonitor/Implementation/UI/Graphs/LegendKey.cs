// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    internal class LegendKey : VisualElement
    {
        Label m_KeyLabel;
        VisualElement m_Swatch;

        internal LegendKey()
        {
            AddToClassList(UssClassNames.k_GraphLegendKey);

            AddToClassList(UssClassNames.k_GraphLegendKey);
            m_Swatch = new VisualElement();
            m_Swatch.AddToClassList(UssClassNames.k_GraphLegendKeySwatch);
            Add(m_Swatch);

            m_KeyLabel = new Label();
            m_KeyLabel.AddToClassList(UssClassNames.k_GraphLegendKeyLabel);
            Add(m_KeyLabel);
        }

        internal LegendKey(string name, Color32 color)
            : this()
        {
            UpdateName(name);
            UpdateColor(color);
        }

        internal void Update(string name, Color32 color)
        {
            UpdateName(name);
            UpdateColor(color);
        }

        internal void UpdateName(string name)
        {
            this.name = name;
            m_KeyLabel.text = name;
        }

        internal void UpdateColor(Color32 color)
        {
            m_Swatch.style.backgroundColor = (Color)color;
        }
    }
}
#endif
