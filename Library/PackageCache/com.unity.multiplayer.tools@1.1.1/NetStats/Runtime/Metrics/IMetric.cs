using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    interface IMetric
    {
        string Name { get; }
        MetricId Id { get; }
        int GetWriteSize();
        void Write(FastBufferWriter writer);
        void Read(FastBufferReader reader);
        MetricContainerType MetricContainerType { get; }
        FixedString128Bytes FactoryTypeName { get; }
    }

    interface IMetric<TValue> : IMetric
    {
        TValue Value { get; }
    }
}
