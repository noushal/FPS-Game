using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    internal class NetStatSerializerTests
    {
        static MetricCollection SerializeDeserializeMetrics(List<IMetric> metrics)
        {
            var collection = MetricCollectionTestUtility.ConstructFromMetrics(metrics);

            var serialized = new NetStatSerializer().Serialize(collection);
            var deserialized = new NetStatSerializer().Deserialize(serialized);
            return deserialized;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenCountersInCollection_WhenSerializedDeserialized_CounterValuesCorrect(int numCounters)
        {
            var counters = new List<Counter>();
            for (int i = 0; i < numCounters; ++i)
            {
                var id = MetricId.Create((DirectedMetricType)i);
                counters.Add(new Counter(id, i));
            }

            var result = SerializeDeserializeMetrics(counters.Cast<IMetric>().ToList());

            for (int i = 0; i < numCounters; ++i)
            {
                Assert.IsTrue(result.TryGetCounter(counters[i].Id, out var deserializedCounter));
                Assert.AreEqual(deserializedCounter.Value, counters[i].Value);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenGaugesInCollection_WhenSerializedDeserialized_GaugeValuesCorrect(int numGauges)
        {
            var gauges = new List<Gauge>();
            for (int i = 0; i < numGauges; ++i)
            {
                var id = MetricId.Create((DirectedMetricType)i);
                gauges.Add(new Gauge(id, i * 0.5f));
            }

            var result = SerializeDeserializeMetrics(gauges.Cast<IMetric>().ToList());

            for (int i = 0; i < numGauges; ++i)
            {
                Assert.IsTrue(result.TryGetGauge(gauges[i].Id, out var deserializedGauge));
                Assert.IsTrue(Math.Abs(deserializedGauge.Value - gauges[i].Value) < float.Epsilon);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenEventsInCollection_WhenSerializedDeserialized_EventValuesCorrect(int numEvents)
        {
            var events = new List<EventMetric<int>>();
            for (int i = 0; i < numEvents; ++i)
            {
                var id = MetricId.Create((DirectedMetricType)i);
                var evt = new EventMetric<int>(id);
                for (int j = 0; j < numEvents; ++j)
                {
                    evt.Mark(j);
                }

                events.Add(evt);
            }

            var result = SerializeDeserializeMetrics(events.Cast<IMetric>().ToList());

            for (int i = 0; i < numEvents; ++i)
            {
                Assert.IsTrue(result.TryGetEvent<int>(events[i].Id, out var deserializedEvent));
                Assert.AreEqual(events[i].Values.Count, deserializedEvent.Values.Count);
                Assert.IsTrue(events[i].Values.SequenceEqual(deserializedEvent.Values));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void GivenGenericEventsInCollection_WhenSerializedDeserialized_EventValuesCorrect(int numEvents)
        {
            var events = new List<EventMetric<TestEventData>>();
            for (int i = 0; i < numEvents; ++i)
            {
                var id = MetricId.Create((DirectedMetricType)i);
                var evt = new EventMetric<TestEventData>(id);
                for (int j = 0; j < numEvents; ++j)
                {
                    evt.Mark(new TestEventData()
                    {
                        Int1 = j * 10,
                        Bool1 = j % 2 == 0
                    });
                }

                events.Add(evt);
            }

            var result = SerializeDeserializeMetrics(events.Cast<IMetric>().ToList());

            for (int i = 0; i < numEvents; ++i)
            {
                Assert.IsTrue(result.TryGetEvent<TestEventData>(events[i].Id, out var deserializedEvent));
                Assert.AreEqual(events[i].Values.Count, deserializedEvent.Values.Count);

                var eventValues = events[i].Values.ToList();
                var deserializedEventValues = deserializedEvent.Values.ToList();
                for (int j = 0; j < eventValues.Count; ++j)
                {
                    deserializedEventValues[j].AssertEquals(eventValues[j]);
                }
            }
        }
    }
}
