using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    class NetStatSerializer : INetStatSerializer
    {
        MetricFactory m_MetricFactory = new MetricFactory();

        public NativeArray<byte> Serialize(MetricCollection metricCollection)
        {
            int size = 0;
            for (int i = 0; i < metricCollection.Metrics.Count; ++i)
            {
                var metric = metricCollection.Metrics[i];
                size += FastBufferWriter.GetWriteSize<MetricHeader>();
                size += metric.GetWriteSize();
            }

            size += FastBufferWriter.GetWriteSize<ulong>();

            using var writer = new FastBufferWriter(size, Allocator.Temp, int.MaxValue);

            writer.WriteValueSafe(metricCollection.ConnectionId);

            writer.WriteValueSafe(metricCollection.Metrics.Count);
            for(int i = 0; i < metricCollection.Metrics.Count; ++i)
            {
                var metric = metricCollection.Metrics[i];

                var header = new MetricHeader(
                    metric.FactoryTypeName,
                    metric.MetricContainerType,
                    metric.Id
                );

                writer.WriteValueSafe(header);

                writer.TryBeginWrite(metric.GetWriteSize());
                metric.Write(writer);
            }

            return writer.ToNativeArray(Allocator.Temp);
        }

        public MetricCollection Deserialize(NativeArray<byte> bytes)
        {
            var metrics = new List<IMetric>();
            ulong connectionId;

            using (var reader = new FastBufferReader(bytes, Allocator.Temp))
            {
                reader.ReadValueSafe(out connectionId);

                reader.ReadValueSafe(out int metricsCount);
                for (int i = 0; i < metricsCount; ++i)
                {
                    reader.ReadValueSafe(out MetricHeader header);

                    if (m_MetricFactory.TryConstruct(header, out var metric))
                    {
                        metric.Read(reader);
                        metrics.Add(metric);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Failed to construct metric from serialized data. Metric Header: {header}");
                    }
                }
            }

            return new MetricCollection(
                metrics,
                connectionId);
        }
    }
}
