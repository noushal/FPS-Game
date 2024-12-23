using System;

using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.Adapters
{
    interface IMetricCollectionEvent : IAdapterComponent
    {
        event Action<MetricCollection> MetricCollectionEvent;
    }
}
