using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface
{
    static class RnsmHelper
    {
        public static RuntimeNetStatsMonitor CreateRnsm(
            NetStatsMonitorConfiguration netStatsMonitorConfiguration = null,
            PositionConfiguration positionConfiguration = null,
            PanelSettings panelSettings = null,
            StyleSheet styleSheet = null)
        {
            var go = new GameObject();
            var rnsm = go.AddComponent<RuntimeNetStatsMonitor>();

            if (netStatsMonitorConfiguration != null)
            {
                rnsm.Configuration = netStatsMonitorConfiguration;
            }

            if (positionConfiguration != null)
            {
                rnsm.Position = positionConfiguration;
            }

            if (panelSettings != null)
            {
                rnsm.PanelSettingsOverride = panelSettings;
            }

            if (styleSheet != null)
            {
                rnsm.CustomStyleSheet = styleSheet;
            }

            return rnsm;
        }
    }
}
