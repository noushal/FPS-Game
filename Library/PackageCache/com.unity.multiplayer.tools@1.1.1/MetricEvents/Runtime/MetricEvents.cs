using System;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricEvents
{
    static class MetricEventPublisher
    {
        public static event Action<MetricCollection> OnMetricsReceived;

        /// This function is needed for producers of this information to raise this event
        /// because events can only be invoked within the class they are declared
        public static void RaiseOnMetricsReceived(MetricCollection metricCollection)
        {
            OnMetricsReceived?.Invoke(metricCollection);
        }
    }
}