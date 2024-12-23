using System.Diagnostics;
using Unity.Multiplayer.Tools.Adapters;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    static class ProfilerAdapterEventListener
    {
        [RuntimeInitializeOnLoadMethod]
        static void SubscribeToAdapterAndMetricEvents()
        {
            _ = NetworkAdapters.SubscribeToAll(
                OnAdapterAdded,
                OnAdapterRemoved);
        }

        static void OnAdapterAdded(INetworkAdapter adapter)
        {
            var metricEventComponent = adapter.GetComponent<IMetricCollectionEvent>();
            if (metricEventComponent != null)
            {
                metricEventComponent.MetricCollectionEvent += OnMetricsReceived;
            }
        }

        static void OnAdapterRemoved(INetworkAdapter adapter)
        {
            var metricEventComponent = adapter.GetComponent<IMetricCollectionEvent>();
            if (metricEventComponent != null)
            {
                metricEventComponent.MetricCollectionEvent -= OnMetricsReceived;
            }
        }

        static void OnMetricsReceived(MetricCollection metricCollection)
        {
            PopulateProfilerIfEnabled(metricCollection);
        }

#if UNITY_2021_2_OR_NEWER
        static readonly NetStatSerializer s_NetStatSerializer = new();
#endif

        [Conditional("ENABLE_PROFILER")]
        static void PopulateProfilerIfEnabled(MetricCollection collection)
        {
#if UNITY_2021_2_OR_NEWER
            ProfilerCounters.Instance.UpdateFromMetrics(collection);

            using var result = s_NetStatSerializer.Serialize(collection);
            Profiler.EmitFrameMetaData(
                FrameInfo.NetworkProfilerGuid,
                FrameInfo.NetworkProfilerDataTag,
                result);
#else
            ProfilerCounters.Instance.UpdateFromMetrics(collection);
#endif
        }
    }
}