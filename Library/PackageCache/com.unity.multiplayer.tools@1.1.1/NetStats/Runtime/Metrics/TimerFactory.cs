namespace Unity.Multiplayer.Tools.NetStats
{
    class TimerFactory : IMetricFactory
    {
        public bool TryConstruct(MetricHeader header, out IMetric metric)
        {
            metric = new Timer(header.MetricId);
            return true;
        }
    }
}
