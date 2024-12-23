using System;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricTests
    {
        [TestCase(4957, 18)]
        [TestCase(-487, 22)]
        [TestCase(312, 3)]
        public void Constructor_WhenMetricIdIsOutOfRange_ThrowsException(int typeIndex, int enumValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Counter(
                new MetricId(typeIndex: typeIndex, enumValue: enumValue)));
        }

        [Test]
        public void Constructor_Always_SetsNameAndDefaultValue()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.OwnershipChangeReceived);
            var value = new Random().Next();

            // Act
            var metric = new TestMetric(id, value);

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.AreEqual(value, metric.Value);
        }

        [Test]
        public void Reset_Always_SetsUnderlyingValueToDefaultValue()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.TotalBytesSent);
            var value = new Random().Next();

            var metric = new TestMetric(id, value);
            metric.SetValue(100);

            // Act
            metric.Reset();

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.AreEqual(value, metric.Value);
        }

        [Serializable]
        internal class TestMetric : Metric<int>
        {
            public TestMetric(MetricId metricId, int defaultValue = default)
                : base(metricId, defaultValue)
            {
            }

            public void SetValue(int value)
            {
                Value = value;
            }

            public override MetricContainerType MetricContainerType => MetricContainerType.Counter;
        }
    }
}