using NUnit.Framework;
namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class LineGraphConfigurationTests
    {
        [TestCase(0.99f ,ConfigurationLimits.k_GraphLineThicknessMin)]
        [TestCase(10.01f ,ConfigurationLimits.k_GraphLineThicknessMax)]
        [TestCase(4.99f ,4.99f)]
        [TestCase(11 ,ConfigurationLimits.k_GraphLineThicknessMax)]
        [TestCase(float.MinValue, ConfigurationLimits.k_GraphLineThicknessMin)]
        [TestCase(float.MaxValue, ConfigurationLimits.k_GraphLineThicknessMax)]
        public void SetGraphLineThickness_WithInBoundsOrOutBoundsValues_ReturnInBoundValueOrClampedValue(float lineThicknessInput, float lineThicknessExpected)
        {
            var displayElementConfiguration = new DisplayElementConfiguration();
            var graphConfig = displayElementConfiguration.GraphConfiguration;
            graphConfig.LineGraphConfiguration.LineThickness = lineThicknessInput;

            Assert.IsNotNull(graphConfig.LineGraphConfiguration);
            Assert.AreEqual(lineThicknessExpected, graphConfig.LineGraphConfiguration.LineThickness);
            Assert.That(graphConfig.LineGraphConfiguration.LineThickness, Is.InRange(ConfigurationLimits.k_GraphLineThicknessMin, ConfigurationLimits.k_GraphLineThicknessMax));
        }
    }
}
