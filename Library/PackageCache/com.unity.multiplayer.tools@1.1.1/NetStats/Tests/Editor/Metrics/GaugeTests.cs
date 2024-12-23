using System;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class GaugeTests
    {
        [Test]
        public void Set_Always_SetsUnderlyingValueToSpecifiedAmount()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.NetworkVariableDeltaReceived);
            var gauge = new Gauge(id);
            var value = new Random().NextDouble();

            // Act
            gauge.Set(value);

            // Assert
            Assert.AreEqual(id, gauge.Id);
            Assert.AreEqual(value, gauge.Value);
        }
    }
}