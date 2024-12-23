using System;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class CounterTests
    {
        [Test]
        public void Increment_Always_IncrementsUnderlyingValue()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.RpcSent);
            var counter = new Counter(id, 10);

            // Act
            counter.Increment();

            // Assert
            Assert.AreEqual(id, counter.Id);
            Assert.AreEqual(11, counter.Value);
        }

        [TestCase(10, 1, 11)]
        [TestCase(10, -1, 9)]
        public void Increment_Always_IncrementsUnderlyingValueBySpecifiedAmount(long initial, long increment, long expected)
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.UnnamedMessageSent);
            var counter = new Counter(id, initial);

            // Act
            counter.Increment(increment);

            // Assert
            Assert.AreEqual(id, counter.Id);
            Assert.AreEqual(expected, counter.Value);
        }
    }
}