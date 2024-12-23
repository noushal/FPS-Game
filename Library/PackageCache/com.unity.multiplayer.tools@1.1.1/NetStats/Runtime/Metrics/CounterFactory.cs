namespace Unity.Multiplayer.Tools.NetStats
{
    class CounterFactory : IMetricFactory
    {
        public bool TryConstruct(MetricHeader header, out IMetric metric)
        {
            metric = new Counter(header.MetricId);
            return true;
        }
    }
}
