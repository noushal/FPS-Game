using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class GraphConfigurationTests
    {
        GraphConfiguration m_GraphConfiguration = new();

        [TestCase(7, ConfigurationLimits.k_GraphSampleMin)]
        [TestCase(-1, ConfigurationLimits.k_GraphSampleMin)]
        [TestCase(8, 8)]
        [TestCase(4096, 4096)]
        [TestCase(4097, ConfigurationLimits.k_GraphSampleMax)]
        [TestCase(int.MinValue, ConfigurationLimits.k_GraphSampleMin)]
        [TestCase(int.MaxValue, ConfigurationLimits.k_GraphSampleMax)]
        public void SetGraphSampleCount_WithInBoundsOrOutOfBoundsValues_ReturnInBoundValueOrClampedValue(int sampleCountInput, int sampleCountExpected)
        {
            m_GraphConfiguration.SampleCount = sampleCountInput;
            Assert.AreEqual(sampleCountExpected, m_GraphConfiguration.SampleCount);
            Assert.That(m_GraphConfiguration.SampleCount, Is.InRange(ConfigurationLimits.k_GraphSampleMin, ConfigurationLimits.k_GraphSampleMax));
        }

        [TestCase(GraphXAxisType.Samples)]
        [TestCase(GraphXAxisType.Time)]
        public void SetGraphXAxisType_WithXAxisType_XAxisTypeSetCorrectly(GraphXAxisType input)
        {
            m_GraphConfiguration.XAxisType = input;
            Assert.AreEqual(input, m_GraphConfiguration.XAxisType);
        }

        [Test]
        [Description("Test the graph configuration color by adding them to a list")]
        public void AddGraphColor_GivenColorList_ShouldContainAddedColor()
        {
            IReadOnlyList<Color> colorsList = new List<Color>
            {
                Color.black,
                Color.blue,
                Color.red,
                Color.yellow,
                new(1f, 0.5f, 0f, 0.5f),
            };

            foreach (var color in colorsList)
            {
                var count = m_GraphConfiguration.VariableColors.Count;
                m_GraphConfiguration.VariableColors.Add(color);
                Assert.Contains(color, m_GraphConfiguration.VariableColors);
                Assert.AreEqual (count + 1, m_GraphConfiguration.VariableColors.Count);
            }
        }
    }
}