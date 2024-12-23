using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class Counter : Metric<long>
    {
        public Counter(MetricId metricId, long defaultValue = default)
            : base(metricId, defaultValue)
        {
        }

        public void Increment(long increment = 1)
        {
            Value += increment;
        }
        
        public override MetricContainerType MetricContainerType => MetricContainerType.Counter;
    }
}
