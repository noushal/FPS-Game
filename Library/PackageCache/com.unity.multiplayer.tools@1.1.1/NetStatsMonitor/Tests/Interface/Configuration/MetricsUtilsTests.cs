
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStatsMonitor.Configuration;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Configuration.Util
{
    internal class MetricsUtilsTests
    {
        [TestCase(new DirectedMetricType[] {}, "")]

        // Single stats
        // --------------------------------------------------------------------
        [TestCase(new[] { DirectedMetricType.Connections }, "Connections")]
        [TestCase(new[] { DirectedMetricType.TotalBytesSent }, "Total Bytes Sent")]
        [TestCase(new[] { DirectedMetricType.TotalBytesReceived }, "Total Bytes Received")]
        [TestCase(new[] { DirectedMetricType.RttToServer }, "RTT To Server")]

        // Special case, duplicated stat
        // --------------------------------------------------------------------
        [TestCase(new[]
        {
            DirectedMetricType.NamedMessageSent,
            DirectedMetricType.NamedMessageSent
        }, "2 × Named Messages Sent")]
        [TestCase(new[]
        {
            DirectedMetricType.RttToServer,
            DirectedMetricType.RttToServer
        }, "2 × RTT To Server")]

        // Same underlying stats, different direction
        // --------------------------------------------------------------------
        [TestCase(new[]
        {
            DirectedMetricType.TotalBytesSent,
            DirectedMetricType.TotalBytesReceived
        }, "Total Bytes Sent and Received")]
        [TestCase(new[]
        {
            DirectedMetricType.RpcSent,
            DirectedMetricType.RpcReceived
        }, "RPCs Sent and Received")]
        [TestCase(new[]
        {
            DirectedMetricType.NetworkVariableDeltaReceived,
            DirectedMetricType.NetworkVariableDeltaSent,
        }, "Network Variable Deltas Sent and Received")]

        // Different underlying stats, same direction
        // --------------------------------------------------------------------
        [TestCase(new[]
        {
            DirectedMetricType.NamedMessageSent,
            DirectedMetricType.UnnamedMessageSent
            // I would like this to be "Named and Unnamed Messages Sent",
            // but in the general case that's starting to get a bit more involved
            // by finding shared nouns between both display names and combining their
            // adjectives as well as their directions
        }, "Named Messages and Unnamed Messages Sent")]
        [TestCase(new[]
        {
            DirectedMetricType.UnnamedMessageReceived,
            DirectedMetricType.NamedMessageReceived
        }, "Unnamed Messages and Named Messages Received")]
        [TestCase(new[]
        {
            DirectedMetricType.TotalBytesSent,
            DirectedMetricType.ObjectDestroyedSent,
        }, "Total Bytes and Objects Destroyed Sent")]

        // Unrelated stats
        // --------------------------------------------------------------------
        // Hopefully the user doesn't combine some of these stats with
        // differing units, but the generated label should still work
        [TestCase(new[]
        {
            DirectedMetricType.NamedMessageSent,
            DirectedMetricType.UnnamedMessageReceived
        }, "Named Messages Sent and Unnamed Messages Received")]
        [TestCase(new[]
        {
            DirectedMetricType.RpcSent,
            DirectedMetricType.RttToServer,
            // Hopefully the user doesn't combine these two of differing units,
            // but the generated label should still work
        }, "RPCs Sent and RTT To Server")]
        [TestCase(new[]
        {
            DirectedMetricType.PacketLoss,
            DirectedMetricType.NetworkObjects,
        }, "Packet Loss and Network Objects")]

        // More than two stats, we give up on generating a label at this point
        // --------------------------------------------------------------------
        [TestCase(new[]
        {
            DirectedMetricType.NamedMessageSent,
            DirectedMetricType.NamedMessageReceived,
            DirectedMetricType.UnnamedMessageReceived,
        }, "")]
        [TestCase(new[]
        {
            DirectedMetricType.NetworkMessageReceived,
            DirectedMetricType.PacketLoss,
            DirectedMetricType.TotalBytesSent,
        }, "")]
        [TestCase(new[]
        {
            DirectedMetricType.NamedMessageSent,
            DirectedMetricType.NamedMessageReceived,
            DirectedMetricType.UnnamedMessageSent,
            DirectedMetricType.UnnamedMessageReceived,
        }, "")]
        public void GenerateLabelTest(DirectedMetricType[] metrics, string expectedLabel)
        {
            // Can't use metricIds directly in the attribute, as they are not constants
            var metricIds = metrics.Select(MetricId.Create).ToList();
            var label = LabelGeneration.GenerateLabel(metricIds);
            Assert.AreEqual(expectedLabel, label);
        }
    }
}