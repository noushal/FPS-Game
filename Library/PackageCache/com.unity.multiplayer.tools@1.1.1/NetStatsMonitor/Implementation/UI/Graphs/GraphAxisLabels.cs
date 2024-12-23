// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    internal class GraphAxisLabels : VisualElement
    {
        readonly Label m_MinLabel = new();
        readonly Label m_MaxLabel = new();

        public string MinLabel
        {
            get => m_MinLabel.text;
            set => m_MinLabel.text = value;
        }
        public string MaxLabel
        {
            get => m_MaxLabel.text;
            set => m_MaxLabel.text = value;
        }

        internal GraphAxisLabels()
        {
            AddToClassList(UssClassNames.k_GraphAxis);
            m_MinLabel.AddToClassList(UssClassNames.k_GraphAxisMinValueLabel);
            m_MaxLabel.AddToClassList(UssClassNames.k_GraphAxisMaxValueLabel);

            Add(m_MinLabel);
            Add(m_MaxLabel);
        }

        public void SetLabels(string minLabel, string maxLabel)
        {
            MinLabel = minLabel;
            MaxLabel = maxLabel;
        }

        public StyleLength MaxLabelMarginRight
        {
            get => m_MaxLabel.style.marginRight;
            set => m_MaxLabel.style.marginRight = value;
        }
    }
}
#endif
