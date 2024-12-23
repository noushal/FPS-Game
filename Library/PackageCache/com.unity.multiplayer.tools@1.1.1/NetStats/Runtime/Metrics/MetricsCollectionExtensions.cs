using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    static class MetricsCollectionExtensions
    {
        public static IReadOnlyList<TMetric> GetEventValues<TMetric>(
            this MetricCollection collection,
            MetricId metricId)
        {
            return collection.TryGetEvent<TMetric>(metricId, out var metric)
                ? metric.Values
                : Array.Empty<TMetric>();
        }
    }
}
