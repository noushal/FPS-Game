using System;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricEventTests
    {
        const int k_MetricValue1 = 1234;
        const int k_MetricValue2 = 5678;

        [TestCase(4596, 9)]
        [TestCase(-1, 7)]
        [TestCase(347, 11)]
        public void Constructor_WhenTypeIndexIsOutOfRange_ThrowsException(int typeIndex, int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new EventMetric<int>(
                new MetricId(typeIndex, value)));
        }

        [Test]
        public void ResetStringMetricEvent_Always_ClearsUnderlyingCollection()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.RpcReceived);
            var metric = new EventMetric<int>(id);
            metric.Mark(k_MetricValue1);

            // Act
            metric.Reset();

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.IsEmpty(metric.Values);
        }

        [Test]
        public void MarkStringMetricEvent_Always_AddsValueToUnderlyingCollection()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.RpcReceived);
            var metric = new EventMetric<int>(id);

            // Act
            metric.Mark(k_MetricValue1);

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.AreEqual(1, metric.Values.Count);
            Assert.AreEqual(k_MetricValue1, metric.Values.FirstOrDefault());
        }

        [Test]
        public void MarkStringMetricEvent_WithExistingItemsInCollection_KeepsUnderlyingItemsInOrder()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.SceneEventSent);
            var metric = new EventMetric<int>(id);
            var values = new[] { 1, 2, 3, 4, 5 };

            // Act
            foreach (var value in values)
            {
                metric.Mark(value);
            }

            // Assert
            Assert.AreEqual(id, metric.Id);
            CollectionAssert.AreEquivalent(values, metric.Values);
        }

        [Test]
        public void ResetCustomStructMetricEvent_Always_ClearsUnderlyingCollection()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.ObjectSpawnedReceived);
            var metric = new EventMetric<CustomEvent>(id);
            metric.Mark(new CustomEvent());

            // Act
            metric.Reset();

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.IsEmpty(metric.Values);
        }

        [Test]
        public void MarkCustomStructMetricEvent_Always_AddsValueToUnderlyingCollection()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.TotalBytesSent);
            var metric = new EventMetric<CustomEvent>(id);
            var value = new CustomEvent(k_MetricValue1);

            // Act
            metric.Mark(value);

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.AreEqual(1, metric.Values.Count);
            Assert.AreEqual(value, metric.Values.FirstOrDefault());
        }

        [Test]
        public void MarkCustomStructMetricEvent_WithExistingItemsInCollection_KeepsUnderlyingItemsInOrder()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.ObjectDestroyedReceived);
            var metric = new EventMetric<CustomEvent>(id);
            var values = new[] { new CustomEvent(k_MetricValue1), new CustomEvent(k_MetricValue2) };

            // Act
            foreach (var value in values)
            {
                metric.Mark(value);
            }

            // Assert
            Assert.AreEqual(id, metric.Id);
            CollectionAssert.AreEquivalent(values, metric.Values);
        }

        [Test]
        public void MarkMetricEvent_WhenNumberOfValuesDoesNotGoOverLimit_DoesNotSetWentOverLimitFlag()
        {
            // Arrange
            var metricLimit = 10;
            var id = MetricId.Create(DirectedMetricType.NetworkVariableDeltaReceived);
            var metric = new EventMetric<int>(id)
            {
                MaxNumberOfValues = metricLimit,
            };

            // Act
            foreach (var _ in Enumerable.Range(0, (int)metricLimit))
            {
                metric.Mark(k_MetricValue1);
            }

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.False(metric.WentOverLimit());
        }

        [Test]
        public void MarkMetricEvent_WhenNumberOfValuesGoesOverLimit_SetsWentOverLimitFlag()
        {
            // Arrange
            var metricLimit = 10;
            var id = MetricId.Create(DirectedMetricType.RpcSent);
            var metric = new EventMetric<int>(id)
            {
                MaxNumberOfValues = metricLimit,
            };

            // Act
            foreach (var _ in Enumerable.Range(0, (int)metricLimit + 1))
            {
                metric.Mark(k_MetricValue1);
            }

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.True(metric.WentOverLimit());
        }

        [Test]
        public void Reset_Always_ResetsWentOverLimitFlag()
        {
            // Arrange
            var metricLimit = 10;
            var id = MetricId.Create(DirectedMetricType.TotalBytesReceived);
            var metric = new EventMetric<int>(id)
            {
                MaxNumberOfValues = metricLimit,
            };

            // Act
            foreach (var _ in Enumerable.Range(0, (int)metricLimit + 1))
            {
                metric.Mark(k_MetricValue1);
            }

            Assert.True(metric.WentOverLimit());

            // Act
            metric.Reset();

            // Assert
            Assert.AreEqual(id, metric.Id);
            Assert.False(metric.WentOverLimit());
        }

        [Serializable]
        public struct CustomEvent
        {
            public CustomEvent(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }
    }
}