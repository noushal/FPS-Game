using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class ProfilerCountersTestData
    {
        const long k_DefaultExpectedCounterValue = 100;

        public static List<ProfilerCountersTests.CounterValidationParameters> ValidateCounterTestCases => new List<ProfilerCountersTests.CounterValidationParameters>()
        {
            new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = DirectedMetricType.TotalBytesSent.GetDisplayName(),
                Metrics = () => SingleCounterValue(DirectedMetricType.TotalBytesSent.GetId(), 100),
                CounterName = counters => counters.totalBytes.Sent,
                CounterValue = 100
            },
            new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = DirectedMetricType.TotalBytesReceived.GetDisplayName(),
                Metrics = () => SingleCounterValue(DirectedMetricType.TotalBytesReceived.GetId(), 100),
                CounterName = counters => counters.totalBytes.Received,
                CounterValue = 100
            },

            // RPC
            SingleEventTestCase(
                DirectedMetricType.RpcSent.GetId(),
                new RpcEvent(default, default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.rpc.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.RpcReceived.GetId(),
                new RpcEvent(default, default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.rpc.Bytes.Received),

            // Named Message
            SingleEventTestCase(
                DirectedMetricType.NamedMessageSent.GetId(),
                new NamedMessageEvent(default, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.namedMessage.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.NamedMessageReceived.GetId(),
                new NamedMessageEvent(default, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.namedMessage.Bytes.Received),

            // Unnamed Message
            SingleEventTestCase(
                DirectedMetricType.UnnamedMessageSent.GetId(),
                new UnnamedMessageEvent(default, k_DefaultExpectedCounterValue),
                counters => counters.unnamedMessage.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.UnnamedMessageReceived.GetId(),
                new UnnamedMessageEvent(default, k_DefaultExpectedCounterValue),
                counters => counters.unnamedMessage.Bytes.Received),

            // Network Variable Delta
            SingleEventTestCase(
                DirectedMetricType.NetworkVariableDeltaSent.GetId(),
                new NetworkVariableEvent(default, default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.networkVariableDelta.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.NetworkVariableDeltaReceived.GetId(),
                new NetworkVariableEvent(default, default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.networkVariableDelta.Bytes.Received),

            // Object Spawned
            SingleEventTestCase(
                DirectedMetricType.ObjectSpawnedSent.GetId(),
                new ObjectSpawnedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectSpawned.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.ObjectSpawnedReceived.GetId(),
                new ObjectSpawnedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectSpawned.Bytes.Received),

            // Object Destroyed
            SingleEventTestCase(
                DirectedMetricType.ObjectDestroyedSent.GetId(),
                new ObjectDestroyedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectDestroyed.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.ObjectDestroyedReceived.GetId(),
                new ObjectDestroyedEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.objectDestroyed.Bytes.Received),

            // ServerLog
            SingleEventTestCase(
                DirectedMetricType.ServerLogSent.GetId(),
                new ServerLogEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.serverLog.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.ServerLogReceived.GetId(),
                new ServerLogEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.serverLog.Bytes.Received),

            // SceneEvent
            SingleEventTestCase(
                DirectedMetricType.SceneEventSent.GetId(),
                new SceneEventMetric(default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.sceneEvent.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.SceneEventReceived.GetId(),
                new SceneEventMetric(default, string.Empty, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.sceneEvent.Bytes.Received),

            // Ownership Change
            SingleEventTestCase(
                DirectedMetricType.OwnershipChangeSent.GetId(),
                new OwnershipChangeEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.ownershipChange.Bytes.Sent),
            SingleEventTestCase(
                DirectedMetricType.OwnershipChangeReceived.GetId(),
                new OwnershipChangeEvent(default, default, k_DefaultExpectedCounterValue),
                counters => counters.ownershipChange.Bytes.Received),

            // Network Message
            SingleEventTestCase(
                DirectedMetricType.NetworkMessageSent.GetId(),
                new NetworkMessageEvent(default, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.networkMessage.Events.Sent,
                1),
            SingleEventTestCase(
                DirectedMetricType.NetworkMessageReceived.GetId(),
                new NetworkMessageEvent(default, string.Empty, k_DefaultExpectedCounterValue),
                counters => counters.networkMessage.Events.Received,
                1),

            // Custom Message
            new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = "Custom Message Sent",
                Metrics = () => new IMetric[]
                {
                    Events(DirectedMetricType.NamedMessageSent.GetId(), new[]
                    {
                        new NamedMessageEvent(default, "Test1", 100)
                    }),
                    Events(DirectedMetricType.UnnamedMessageSent.GetId(), new[]
                    {
                        new UnnamedMessageEvent(default, 100)
                    })
                },
                CounterName = counters => counters.customMessage.Bytes.Sent,
                CounterValue = 200
            },
            new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = "Custom Message Received",
                Metrics = () => new IMetric[]
                {
                    Events(DirectedMetricType.NamedMessageReceived.GetId(), new[]
                    {
                        new NamedMessageEvent(default, "Test1", 100)
                    }),
                    Events(DirectedMetricType.UnnamedMessageReceived.GetId(), new[]
                    {
                        new UnnamedMessageEvent(default, 100)
                    })
                },
                CounterName = counters => counters.customMessage.Bytes.Received,
                CounterValue = 200
            },
        };

        static ProfilerCountersTests.CounterValidationParameters SingleEventTestCase<T>(
            MetricId metricId,
            T metricEvent,
            Func<ProfilerCounters, string> counterName,
            long expectedCounterValue = k_DefaultExpectedCounterValue) where T : unmanaged
            => new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = metricId.ToString(),
                Metrics = () => SingleEvent(
                    metricId,
                    metricEvent),
                CounterName = counterName,
                CounterValue = expectedCounterValue
            };

        static ProfilerCountersTests.CounterValidationParameters SingleEventTestCase<T>(
            string testCaseName,
            MetricId metricId,
            T metricEvent,
            Func<ProfilerCounters, string> counterName,
            long expectedCounterValue = k_DefaultExpectedCounterValue) where T : unmanaged
            => new ProfilerCountersTests.CounterValidationParameters()
            {
                TestCaseName = testCaseName,
                Metrics = () => SingleEvent(
                    metricId,
                    metricEvent),
                CounterName = counterName,
                CounterValue = expectedCounterValue
            };

        static IMetric[] SingleCounterValue(MetricId id, long value)
            => new IMetric[]
            {
                Counter(id, value)
            };

        static IMetric[] SingleEvent<T>(MetricId id, T evt) where T : unmanaged
            => new IMetric[]
            {
                Events(id, new[]
                {
                    evt
                })
            };

        static Counter Counter(MetricId id, long value)
        {
            var metric = new Counter(id);
            metric.Increment(value);
            return metric;
        }

        static EventMetric<T> Events<T>(MetricId id, T[] events) where T : unmanaged
        {
            var metric = new EventMetric<T>(id);
            foreach (var evt in events)
            {
                metric.Mark(evt);
            }

            return metric;
        }
    }
}
