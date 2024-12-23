using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class RuntimeNetStatsMonitorTest
    {
        GameObject m_RnsmGameObject;
        RuntimeNetStatsMonitor m_NetStatsMonitor;
        StyleSheet m_StyleSheet;
        string m_StyleSheetPath = "Assets/styleSheet.asset";
        PanelSettings m_PanelSettings;
        string m_PanelSettingsPath = "Assets/panelSettings.asset";
        NetStatsMonitorConfiguration m_NetStatsMonitorConfiguration;
        string m_NetStatsMonitorConfigPath = "Assets/NetStatsMonitorConfiguration.asset";

        [OneTimeSetUp]
        public void Setup()
        {
            m_RnsmGameObject = new GameObject();
            m_NetStatsMonitor = m_RnsmGameObject.AddComponent<RuntimeNetStatsMonitor>();
            m_PanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(m_RnsmGameObject);
            Object.DestroyImmediate(m_PanelSettings, true);
            Object.DestroyImmediate(m_StyleSheet, true);
            Object.DestroyImmediate(m_NetStatsMonitorConfiguration, true);
            AssetDatabase.DeleteAsset(m_PanelSettingsPath);
            AssetDatabase.DeleteAsset(m_StyleSheetPath);
            AssetDatabase.DeleteAsset(m_NetStatsMonitorConfigPath);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetRnsmVisibility_WhenGivenBoolean_SetCorrectly(bool input)
        {
            m_NetStatsMonitor.Visible = input;
            Assert.That(m_NetStatsMonitor.Visible, Is.EqualTo(input));
        }

        [TestCase(1,1)]
        [TestCase(0, ConfigurationLimits.k_RefreshRateMin)]
        [TestCase(-1, ConfigurationLimits.k_RefreshRateMin)]
        [TestCase(double.MinValue, ConfigurationLimits.k_RefreshRateMin)]
        [TestCase(double.MaxValue, double.MaxValue)]
        public void SetRnsmMaxRefreshRate_WithInBoundsOrOutBoundsValues_ReturnInBoundValueOrClampedValue(double refreshRateInput, double refreshRateExpected)
        {
            m_NetStatsMonitor.MaxRefreshRate = refreshRateInput;
            Assert.AreEqual(refreshRateExpected, m_NetStatsMonitor.MaxRefreshRate);
        }

        [Test]
        public void SetRnsmCustomStyleSheet_WithStyleSheet_ReturnCorrectNameAndContentHash()
        {
            m_StyleSheet = ScriptableObject.CreateInstance<StyleSheet>();
            AssetDatabase.CreateAsset(m_StyleSheet, m_StyleSheetPath);
            m_NetStatsMonitor.CustomStyleSheet = m_StyleSheet;
            Assert.IsNotNull(m_NetStatsMonitor.CustomStyleSheet);
            Assert.AreEqual(m_NetStatsMonitor.CustomStyleSheet.name, "styleSheet");
            Assert.AreEqual(m_NetStatsMonitor.CustomStyleSheet.contentHash, m_StyleSheet.contentHash);
        }

        [Test]
        public void SetRnsmPanelSettingsOverride_WithPanelSettings_ReturnCorrectPanelSettingNameAndStyleSheet()
        {
            m_PanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            AssetDatabase.CreateAsset(m_PanelSettings, m_PanelSettingsPath);
            m_NetStatsMonitor.PanelSettingsOverride = m_PanelSettings;
            Assert.IsNotNull(m_NetStatsMonitor.PanelSettingsOverride);
            Assert.AreEqual(m_NetStatsMonitor.PanelSettingsOverride.name, "panelSettings");
            Assert.AreEqual(m_NetStatsMonitor.PanelSettingsOverride.themeStyleSheet, m_PanelSettings.themeStyleSheet);
        }

        [Test]
        public void SetRnsmNetStatsMonitorConfiguration_WithNetStatConfig_ReturnCorrectNetStatConfigNameAndConfigurationHash()
        {
            m_NetStatsMonitorConfiguration = ScriptableObject.CreateInstance<NetStatsMonitorConfiguration>();
            AssetDatabase.CreateAsset(m_NetStatsMonitorConfiguration, m_NetStatsMonitorConfigPath);
            m_NetStatsMonitor.Configuration = m_NetStatsMonitorConfiguration;
            Assert.IsNotNull(m_NetStatsMonitor.Configuration);
            Assert.AreEqual(m_NetStatsMonitor.Configuration.name, "NetStatsMonitorConfiguration");
            Assert.AreEqual(m_NetStatsMonitor.Configuration.ConfigurationHash, m_NetStatsMonitorConfiguration.ConfigurationHash);
        }
    }
}