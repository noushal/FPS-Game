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
    internal class NoDataReceivedVisualElement : VisualElement
    {
        Label m_Label = new();
        internal NoDataReceivedVisualElement()
        {
            AddToClassList(UssClassNames.k_DisplayElement);
            AddToClassList(UssClassNames.k_NoDataReceived);
            Add(m_Label);
            m_Label.AddToClassList(UssClassNames.k_NoDataReceivedLabel);
        }

        internal void Update(double secondsSinceDataReceived)
        {
            var wholeSecondsSinceLastUpdate = secondsSinceDataReceived.ToString("N0");
            m_Label.text = $"No data received for {wholeSecondsSinceLastUpdate} seconds";
        }
    }
}
#endif
