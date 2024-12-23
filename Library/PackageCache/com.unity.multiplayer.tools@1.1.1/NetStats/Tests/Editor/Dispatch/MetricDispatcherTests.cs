using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class MetricDispatcherTests
    {
        readonly MetricId MetricId = MetricId.Create(DirectedMetricType.RpcSent);

        [Test]
        public void Dispatch_WhenCounterIsRegistered_DispatchesCounterValueToObservers()
        {
            // Arrange
            var counter = new Counter(MetricId);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetCounter(MetricId, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(10, metric.Value);
            }));

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenGaugeIsRegistered_DispatchesGaugeValueToObservers()
        {
            // Arrange
            var gauge = new Gauge(MetricId);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithGauges(gauge)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetGauge(MetricId, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(10, metric.Value);
            }));

            gauge.Set(10);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenTimerIsRegistered_DispatchesTimerValueToObservers()
        {
            // Arrange
            var timer = new Timer(MetricId);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithTimers(timer)
                .Build();

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetTimer(MetricId, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.AreEqual(TimeSpan.FromHours(1), metric.Value);
            }));

            timer.Set(TimeSpan.FromHours(1));

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenEventIsRegistered_DispatchesEventValueToObservers()
        {
            // Arrange
            var metricEvent = new EventMetric<int>(MetricId);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithMetricEvents(metricEvent)
                .Build();

            var value = 1234;

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetEvent<int>(MetricId, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.NotNull(metric.Values);
                Assert.AreEqual(1, metric.Values.Count);
                Assert.AreEqual(value, metric.Values.First());
            }));

            metricEvent.Mark(value);

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenTypedEventIsRegistered_DispatchesEventValueToObservers()
        {
            // Arrange
            var metricEvent = new EventMetric<TestMetricEvent>(MetricId);
            var metricDispatcher = new MetricDispatcherBuilder()
                .WithMetricEvents(metricEvent)
                .Build();

            var value = 1234;

            metricDispatcher.RegisterObserver(new TestObserver(snapshot =>
            {
                // Assert
                var found = snapshot.TryGetEvent<TestMetricEvent>(MetricId, out var metric);
                Assert.IsTrue(found);
                Assert.NotNull(metric);
                Assert.NotNull(metric.Values);
                Assert.AreEqual(1, metric.Values.Count);
                Assert.NotNull(metric.Values.First());
                Assert.AreEqual(value, metric.Values.First().Value);
            }));

            metricEvent.Mark(new TestMetricEvent(value));

            // Act
            metricDispatcher.Dispatch();
        }

        [Test]
        public void Dispatch_WhenMetricIsResetOnDispatch_ResetsMetric()
        {
            // Arrange
            var counter = new Counter(MetricId)
            {
                ShouldResetOnDispatch = true,
            };

            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();

            // Assert
            Assert.AreEqual(0, counter.Value);
        }

        [Test]
        public void Dispatch_WhenMetricIsNotResetOnDispatch_DoesNotResetMetric()
        {
            // Arrange
            var counter = new Counter(MetricId)
            {
                ShouldResetOnDispatch = false,
            };

            var metricDispatcher = new MetricDispatcherBuilder()
                .WithCounters(counter)
                .Build();

            counter.Increment(10);

            // Act
            metricDispatcher.Dispatch();

            // Assert
            Assert.AreEqual(10, counter.Value);
        }

        [Test]
        public void Dispatch_WhenMetricWentOverLimit_LogsWarning()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.RpcReceived);
            var metric = new EventMetric<int>(id)
            {
                MaxNumberOfValues = 0,
            };

            var metricDispatcher = new MetricDispatcherBuilder()
                .WithMetricEvents(metric)
                .Build();

            metric.Mark(1234);

            // Act & Assert
            Assert.True(metric.WentOverLimit());
            const string k_RegexWhitespaceZeroOrMore = "\\s*";
            LogAssert.Expect(
                LogType.Warning,
                new Regex(Regex.Escape(metric.WentOverLimitMessage()) + k_RegexWhitespaceZeroOrMore));
            metricDispatcher.Dispatch();
        }

        [Serializable]
        public struct TestMetricEvent
        {
            public TestMetricEvent(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        class TestObserver : IMetricObserver
        {
            private readonly Action<MetricCollection> m_Assertion;

            public TestObserver(Action<MetricCollection> assertion)
            {
                m_Assertion = assertion;
            }

            public void Observe(MetricCollection collection)
            {
                m_Assertion.Invoke(collection);
            }
        }
    }
}
