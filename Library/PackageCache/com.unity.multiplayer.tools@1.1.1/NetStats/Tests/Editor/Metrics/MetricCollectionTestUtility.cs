using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    static class MetricCollectionTestUtility
    {
        public static MetricCollection ConstructFromMetrics(
            IReadOnlyCollection<IMetric> metrics,
             ulong localConnectionId = ulong.MaxValue)
        {
            static MetricId ById(IMetric metric) => metric.Id;
            var metricCollection = new MetricCollection(
                metrics.OfType<IMetric<long>>().ToDictionary(ById),
                metrics.OfType<IMetric<double>>().ToDictionary(ById),
                metrics.OfType<IMetric<TimeSpan>>().ToDictionary(ById),
                metrics.OfType<IEventMetric>().ToDictionary(ById));
            metricCollection.ConnectionId = localConnectionId;

            return metricCollection;
        }
    }
}
