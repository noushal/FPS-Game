using Unity.Multiplayer.Tools.MetricEvents;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
    class MetricObserver : IMetricObserver
    {
        public void Observe(MetricCollection collection)
        {
            MetricEventPublisher.RaiseOnMetricsReceived(collection);
        }
    }
}
