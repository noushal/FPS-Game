using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    interface IMetricFactory
    {
        bool TryConstruct(MetricHeader header, out IMetric metric);
    }
}
