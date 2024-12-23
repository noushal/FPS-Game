using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    class MetricFactoryTestsTmp
    {
        MetricFactory m_TestObj;

        static MetricHeader GetCounterMetricHeader() => new MetricHeader
        {
            MetricContainerType = MetricContainerType.Counter,

            MetricId = MetricId.Create(DirectedMetricType.NamedMessageReceived)
        };

        static MetricHeader GetGaugeMetricHeader() => new MetricHeader
        {
            MetricContainerType = MetricContainerType.Gauge,
            MetricId = MetricId.Create(DirectedMetricType.TotalBytesSent)
        };

        static MetricHeader GetTimerMetricHeader() => new MetricHeader
        {
            MetricContainerType = MetricContainerType.Timer,
            MetricId = MetricId.Create(DirectedMetricType.ObjectDestroyedReceived)
        };

        static MetricHeader GetEventMetricHeader(FixedString128Bytes eventFactoryTypeName) => new MetricHeader
        {
            MetricContainerType = MetricContainerType.Event,
            MetricId = MetricId.Create(DirectedMetricType.ObjectDestroyedReceived),
            EventFactoryTypeName = eventFactoryTypeName
        };

        [SetUp]
        public void SetUp()
        {
            m_TestObj = new MetricFactory();
        }

        [Test]
        public void GivenCounterHeader_WhenTryConstructCalled_CounterMetricReturned()
        {
            var header = GetCounterMetricHeader();

            var result = m_TestObj.TryConstruct(header, out var metric);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<Counter>(metric);
        }

        [Test]
        public void WhenTryConstructCalled_MetricHasCorrectName()
        {
            var metricId = MetricId.Create(DirectedMetricType.ObjectDestroyedReceived);

            var header = GetCounterMetricHeader();
            header.MetricId = metricId;

            var result = m_TestObj.TryConstruct(header, out var metric);

            Assert.AreEqual(metric.Id, metricId);
        }

        [Test]
        public void GivenGaugeHeader_WhenTryConstructCalled_GaugeMetricReturned()
        {
            var header = GetGaugeMetricHeader();

            var result = m_TestObj.TryConstruct(header, out var metric);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<Gauge>(metric);
        }

        [Test]
        public void GivenTimerHeader_WhenTryConstructCalled_TimerMetricReturned()
        {
            var header = GetTimerMetricHeader();

            var result = m_TestObj.TryConstruct(header, out var metric);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<Timer>(metric);
        }

        [Test]
        public void GivenEventHeader_WhenTryConstructCalled_EventMetricReturned()
        {
            Assert.IsTrue(EventMetricFactory.TryGetFactoryTypeName(typeof(TestEventData), out var eventFactoryTypeName));

            var header = GetEventMetricHeader(eventFactoryTypeName);

            var result = m_TestObj.TryConstruct(header, out var metric);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<EventMetric<TestEventData>>(metric);
        }
    }
}
