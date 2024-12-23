using NUnit.Framework;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class PositionConfigurationTests
    {
        GameObject m_RnsmGameObject;
        RuntimeNetStatsMonitor m_NetStatsMonitor;

        [OneTimeSetUp]
        public void Setup()
        {
            m_RnsmGameObject = new GameObject();
            m_NetStatsMonitor = m_RnsmGameObject.AddComponent<RuntimeNetStatsMonitor>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(m_RnsmGameObject);
        }

        [TestCase(0,0)]
        [TestCase(-0.01f,ConfigurationLimits.k_PositionMin)]
        [TestCase(1.01f,ConfigurationLimits.k_PositionMax)]
        [TestCase(0.7f,0.7f)]
        [TestCase(1,1)]
        [TestCase(float.MinValue, ConfigurationLimits.k_PositionMin)]
        [TestCase(float.MaxValue, ConfigurationLimits.k_PositionMax)]
        public void SetPositionConfiguration_WithInBoundsOrOutBoundsValues_ReturnInBoundValueOrClampedValue(float positionConfigurationInput, float positionConfigurationExpected)
        {
            m_NetStatsMonitor.Position = new PositionConfiguration
            {
                PositionLeftToRight = positionConfigurationInput,
                PositionTopToBottom = positionConfigurationInput
            };
            Assert.AreEqual(positionConfigurationExpected, m_NetStatsMonitor.Position.PositionLeftToRight);
            Assert.AreEqual(positionConfigurationExpected, m_NetStatsMonitor.Position.PositionTopToBottom);
            Assert.That(m_NetStatsMonitor.Position.PositionLeftToRight, Is.InRange(ConfigurationLimits.k_PositionMin, ConfigurationLimits.k_PositionMax));
            Assert.That(m_NetStatsMonitor.Position.PositionTopToBottom, Is.InRange(ConfigurationLimits.k_PositionMin, ConfigurationLimits.k_PositionMax));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetPositionConfiguration_WhenGivenBoolean_SetCorrectly(bool input)
        {
            m_NetStatsMonitor.Position = new PositionConfiguration{ OverridePosition = input};
            Assert.That(m_NetStatsMonitor.Position.OverridePosition, Is.EqualTo(input));
        }
    }
}
