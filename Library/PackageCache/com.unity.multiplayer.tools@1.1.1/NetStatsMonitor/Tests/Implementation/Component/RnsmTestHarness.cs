using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTestData;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests
{
    internal class RnsmTestHarness
    {
        NetStatsMonitorConfiguration m_Configuration = ScriptableObject.CreateInstance<NetStatsMonitorConfiguration>();

        RnsmComponentImplementation m_Rnsm = new();
        float m_RnsmRefreshRate = 1f;

        GameObject m_TestDataGeneratorObject;
        TestDataGeneratorComponent m_TestDataGenerator;

        double m_CurrentTime = 0;
        int m_DisplayUpdateCount = 0;
        int m_ExpectedDisplayUpdateCount = 0;

        internal RnsmTestHarness(float rnsmRefreshRate, List<MetricId> statsDisplayed = null)
        {
            if (statsDisplayed != null)
            {
                m_Configuration.DisplayElements.Add(new DisplayElementConfiguration
                {
                    Label = "Stats",
                    Stats = statsDisplayed,
                });
            }
            var position = new PositionConfiguration();
            var styleSheet = Resources.Load<StyleSheet>("rnsmStyleSheet");

            // Mocking time for RNSM
            m_Rnsm.GetCurrentTime = () => m_CurrentTime;

            // Callback to record when the display is updated
            m_Rnsm.OnDisplayUpdate += () => { ++m_DisplayUpdateCount; };

            m_Rnsm.SetupAndConfigure(m_Configuration, position, styleSheet, null, m_RnsmRefreshRate);
        }

        internal void Teardown()
        {
            m_Rnsm.Teardown();
            m_Rnsm = null;
            Object.DestroyImmediate(m_TestDataGeneratorObject);
        }

        /// Simulates a frame and validates that display updates occur
        /// in response to new data or elapsed time
        internal void SimulateFrameAndVerifyDisplayUpdate(
            double timeElapsed = 0,
            bool generateNewData = false,
            Dictionary<MetricId, float> customData = null,
            bool updateRnsm = true,
            RnsmDisplayUpdateStatus expectedUpdateStatus = RnsmDisplayUpdateStatus.NoDataReceived)
        {
            m_CurrentTime += timeElapsed;
            if (customData != null)
            {
                foreach (var (metric, value) in customData)
                {
                    m_Rnsm.AddCustomValue(metric, value);
                }
            }
            if (generateNewData)
            {
                if (m_TestDataGeneratorObject == null)
                {
                    m_TestDataGeneratorObject = new GameObject();
                }
                if (m_TestDataGenerator == null)
                {
                    m_TestDataGenerator = m_TestDataGeneratorObject
                        .AddComponent<TestDataGeneratorComponent>();
                }
                m_TestDataGenerator.Update();
            }
            if (updateRnsm)
            {
                m_Rnsm.Update(m_Configuration, m_RnsmRefreshRate);
            }
            if (expectedUpdateStatus.UpdateExpected())
            {
                ++m_ExpectedDisplayUpdateCount;
            }
            Assert.AreEqual(m_ExpectedDisplayUpdateCount, m_DisplayUpdateCount, expectedUpdateStatus.AssertMessage());
        }
    }
}