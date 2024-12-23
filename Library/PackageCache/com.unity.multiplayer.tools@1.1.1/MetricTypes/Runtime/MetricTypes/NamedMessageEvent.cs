using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct NamedMessageEvent : INetworkMetricEvent
    {
        /// String overload maintained for backwards compatibility
        public NamedMessageEvent(ConnectionInfo connection, string name, long bytesCount)
            : this(connection, StringConversionUtility.ConvertToFixedString(name), bytesCount)
        {
        }

        public NamedMessageEvent(ConnectionInfo connection, FixedString64Bytes name, long bytesCount)
        {
            Connection = connection;
            Name = name;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public FixedString64Bytes Name { get; }

        public long BytesCount { get; }
    }
}