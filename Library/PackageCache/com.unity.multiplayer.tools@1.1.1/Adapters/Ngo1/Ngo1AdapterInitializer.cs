using UnityEngine;

namespace Unity.Multiplayer.Tools.Adapters.Ngo1
{
    static class Ngo1AdapterInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void InitializeAdapter()
        {
            var ngo1Adapter = new Ngo1Adapter();
            MetricEvents.MetricEventPublisher.OnMetricsReceived += ngo1Adapter.OnMetricsReceived;
            NetworkAdapters.AddAdapter(ngo1Adapter);
        }
    }
}