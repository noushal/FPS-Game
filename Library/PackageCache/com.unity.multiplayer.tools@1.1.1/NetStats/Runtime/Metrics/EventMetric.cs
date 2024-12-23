using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class EventMetric<TValue> : IEventMetric<TValue>, IResettable
        where TValue : unmanaged
    {
        const int k_DefaultMaxNumberOfValues = 1000;

        readonly List<TValue> m_Values = new List<TValue>();

        public int Count => m_Values.Count;

        public EventMetric(MetricId id)
        {
            Id = id;

            if(EventMetricFactory.TryGetFactoryTypeName(typeof(TValue), out var genericTypeName))
            {
                FactoryTypeName = genericTypeName;
            }
        }

        public string Name => Id.ToString();
        public MetricId Id { get; }

        public MetricContainerType MetricContainerType => MetricContainerType.Event;
        public FixedString128Bytes FactoryTypeName { get; }

        public int GetWriteSize()
        {
            int size = 0;
            size += FastBufferWriter.GetWriteSize<int>(); // count;
            size += FastBufferWriter.GetWriteSize<TValue>() * m_Values.Count;
            return size;
        }

        public void Write(FastBufferWriter writer)
        {
            writer.WriteValue(m_Values.Count);
            for (int i = 0; i < m_Values.Count; ++i)
            {
                writer.WriteValue(m_Values[i]);
            }
        }

        public void Read(FastBufferReader reader)
        {
            m_Values.Clear();
            reader.ReadValueSafe(out int count);
            for (int i = 0; i < count; ++i)
            {
                reader.ReadValueSafe(out TValue value);
                m_Values.Add(value);
            }
        }

        public IReadOnlyList<TValue> Values => m_Values;

        public bool ShouldResetOnDispatch { get; set; } = true;

        public int MaxNumberOfValues { get; set; } = k_DefaultMaxNumberOfValues;

        public int NumberOfValuesReceived { get; private set; } = 0;

        public void Mark(TValue value)
        {
            ++NumberOfValuesReceived;
            if (m_Values.Count >= MaxNumberOfValues)
            {
                return;
            }
            m_Values.Add(value);
        }

        public void Reset()
        {
            m_Values.Clear();
            NumberOfValuesReceived = 0;
        }
    }
}
