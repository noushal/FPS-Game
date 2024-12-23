using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// ScriptableObject used to store the parameters to configure and simulate network conditions.
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(NetworkSimulatorPresetAsset),
        menuName = "Multiplayer/" + nameof(NetworkSimulatorPresetAsset))]
    public class NetworkSimulatorPresetAsset : ScriptableObject, INetworkSimulatorPreset
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
        public static NetworkSimulatorPresetAsset Create(
            string name,
            string description = "",
            int packetDelayMs = 0,
            int packetJitterMs = 0,
            int packetLossInterval = 0,
            int packetLossPercent = 0)
        {
            var configuration = CreateInstance<NetworkSimulatorPresetAsset>();

            configuration.Name = name;
            configuration.Description = description;
            configuration.PacketDelayMs = packetDelayMs;
            configuration.PacketJitterMs = packetJitterMs;
            configuration.PacketLossInterval = packetLossInterval;
            configuration.PacketLossPercent = packetLossPercent;

            return configuration;
        }
    }
}
