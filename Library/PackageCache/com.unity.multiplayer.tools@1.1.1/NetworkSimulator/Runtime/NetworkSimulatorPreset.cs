using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Preset containing the parameters to configure and simulate network conditions.
    /// </summary>
    [Serializable]
    public class NetworkSimulatorPreset : INetworkSimulatorPreset, IEquatable<NetworkSimulatorPreset>
    {
        /// <summary>
        /// Network simulation configuration name.
        /// </summary>
        [field: SerializeField]
        public string Name { get; set; }

        /// <summary>
        /// Optional description of the configuration.
        /// </summary>
        [field: SerializeField]
        public string Description { get; set; }

        /// <summary>
        /// Value for the delay between packet in milliseconds.
        /// </summary>
        [field: SerializeField]
        public int PacketDelayMs { get; set; }

        /// <summary>
        /// Value for the network jitter (variance) in milliseconds.
        /// </summary>
        [field: SerializeField]
        public int PacketJitterMs { get; set; }

        /// <summary>
        /// Value for at which interval packet are dropped
        /// This value is a drop every X packet, not in time.
        /// </summary>
        [field: SerializeField]
        public int PacketLossInterval { get; set; }

        /// <summary>
        /// Value for the average percentage of packet are dropped.
        /// </summary>
        [field: SerializeField]
        public int PacketLossPercent { get; set; }

        /// <summary>
        /// Utility function to create a configuration at runtime.
        /// </summary>
        /// <param name="name">Name of the configuration.</param>
        /// <param name="description">Description of the configuration.</param>
        /// <param name="packetDelayMs">Value for the packet delay in milliseconds.</param>
        /// <param name="packetJitterMs">Value for the network jitter in milliseconds.</param>
        /// <param name="packetLossInterval">Value for the packet loss interval.</param>
        /// <param name="packetLossPercent">Value for the packet loss percentage.</param>
        /// <returns>A valid simulation configuration.</returns>
        public static NetworkSimulatorPreset Create(
            string name,
            string description = "",
            int packetDelayMs = 0,
            int packetJitterMs = 0,
            int packetLossInterval = 0,
            int packetLossPercent = 0)
        {
            var configuration = new NetworkSimulatorPreset
            {
                Name = name,
                Description = description,
                PacketDelayMs = packetDelayMs,
                PacketJitterMs = packetJitterMs,
                PacketLossInterval = packetLossInterval,
                PacketLossPercent = packetLossPercent,
            };

            return configuration;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
        public bool Equals(NetworkSimulatorPreset other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name
                && Description == other.Description
                && PacketDelayMs == other.PacketDelayMs
                && PacketJitterMs == other.PacketJitterMs
                && PacketLossInterval == other.PacketLossInterval
                && PacketLossPercent == other.PacketLossPercent;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NetworkSimulatorPreset)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, PacketDelayMs, PacketJitterMs, PacketLossInterval, PacketLossPercent);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(NetworkSimulatorPreset)} {Name}";
        }
    }
}
