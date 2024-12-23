namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Aggregates configuration values used as presets for the network simulator.
    /// </summary>
    public interface INetworkSimulatorPreset
    {
        /// <summary>
        /// The name of the preset.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A description for the preset, usually explaining the real-world situation that the preset is attempting
        /// to re-create.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Fixed delay to apply to all packets which pass through.
        /// </summary>
        int PacketDelayMs { get; set; }

        /// <summary>
        /// Variable delay to apply to all packets which pass through, adds or subtracts amount from fixed delay.
        /// </summary>
        int PacketJitterMs { get; set; }

        /// <summary>
        /// Fixed interval to drop packets on. This is most suitable for tests where predictable
        /// behaviour is desired, every Xth packet will be dropped.
        /// E.g. If PacketLossInterval is 5 every 5th packet is dropped.
        /// </summary>
        int PacketLossInterval { get; set; }

        /// <summary>
        /// 0 - 100, denotes the percentage of packets that will be dropped.
        /// E.g. "5" means approximately every 20th packet will be dropped.
        /// </summary>
        int PacketLossPercent { get; set; }
    }
}
