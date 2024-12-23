using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct NetworkObjectIdentifier
    {
        /// String overload maintained for backwards compatibility
        public NetworkObjectIdentifier(string name, ulong networkId)
            : this(StringConversionUtility.ConvertToFixedString(name), networkId)
        {
        }

        public NetworkObjectIdentifier(FixedString64Bytes name, ulong networkId)
        {
            Name = name;
            NetworkId = networkId;
        }

        public FixedString64Bytes Name { get; }

        public ulong NetworkId { get; }
    }
}