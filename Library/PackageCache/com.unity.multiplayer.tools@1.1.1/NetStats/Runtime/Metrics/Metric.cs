using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    abstract class Metric<TValue> : IMetric<TValue>, IResettable
        where TValue : unmanaged
    {
        protected Metric(MetricId metricId, TValue defaultValue = default)
        {
            Id = metricId;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public string Name => Id.ToString();

        // public string Name => Id.get
        public MetricId Id { get; }
        public int GetWriteSize() => FastBufferWriter.GetWriteSize<TValue>();
        public void Write(FastBufferWriter writer)
        {
            writer.TryBeginWriteValue(Value);
            writer.WriteValue(Value);
        }

        public void Read(FastBufferReader reader)
        {
            reader.TryBeginReadValue(default(TValue));
            reader.ReadValue(out TValue value);
            Value = value;
        }

        public abstract MetricContainerType MetricContainerType { get; }
        public FixedString128Bytes FactoryTypeName => default;

        public TValue Value { get; protected set; }

        protected TValue DefaultValue { get; }

        public bool ShouldResetOnDispatch { get; set; } = true;

        public void Reset()
        {
            Value = DefaultValue;
        }
    }
}
