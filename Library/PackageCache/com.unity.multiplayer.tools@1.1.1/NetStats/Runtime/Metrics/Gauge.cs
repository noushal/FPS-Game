using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class Gauge : Metric<double>
    {
        public Gauge(MetricId metricId, double defaultValue = default)
            : base(metricId, defaultValue)
        {
        }

        public void Set(double value)
        {
            Value = value;
        }

        public override MetricContainerType MetricContainerType => MetricContainerType.Gauge;
    }
}
