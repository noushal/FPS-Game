using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
    static class MetricObserverFactory
    {
        internal static IMetricObserver Construct() => new MetricObserver();
    }
}
