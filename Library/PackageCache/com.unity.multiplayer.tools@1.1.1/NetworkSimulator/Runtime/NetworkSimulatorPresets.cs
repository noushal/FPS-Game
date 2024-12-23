namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Static class containing the built-in presets to configure and simulate network conditions.
    /// </summary>
    public static class NetworkSimulatorPresets
    {
        const string k_BroadbandDescription = "Typical of desktop and console platforms (and generally speaking most mobile players too).";
        const string k_PoorMobileDescription = "Extremely poor connection, completely unsuitable for synchronous multiplayer gaming due to exceptionally high ping. Turn based games may work.";
        const string k_MediumMobileDescription = "This is the minimum supported mobile connection for synchronous gameplay. Expect high pings, jitter, stuttering and packet loss.";
        const string k_DecentMobileDescription =
            "Suitable for synchronous multiplayer, except that ping (and overall connection quality and stability) may be quite poor.\n\nExpect to handle players dropping all packets in bursts of 1-60s. I.e. Ensure you handle reconnections.";
        const string k_GoodMobileDescription = "In many places, expect this to be 'as good as' or 'better than' home broadband.";

        /// <summary>
        /// Preset with no network simulation applied.
        /// </summary>
        public static readonly NetworkSimulatorPreset None;

        /// <summary>
        /// Preset that simulates a typical home broadband connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset HomeBroadband;

        /// <summary>
        /// Preset that simulates a typical 2G mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile2G;

        /// <summary>
        /// Preset that simulates a typical 2.5G (GPRS) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile2_5G;

        /// <summary>
        /// Preset that simulates a typical 2.75G (EDGE) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile2_75G;

        /// <summary>
        /// Preset that simulates a typical 3G mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile3G;

        /// <summary>
        /// Preset that simulates a typical 3.5G (HSDPA/H) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile3_5G;

        /// <summary>
        /// Preset that simulates a typical 3.75G (HSPA+/H+) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile3_75G;

        /// <summary>
        /// Preset that simulates a typical 4G (LTE) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile4G;

        /// <summary>
        /// Preset that simulates a typical 4.5G (LTE+) mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile4_5G;

        /// <summary>
        /// Preset that simulates a typical 5G mobile connection.
        /// </summary>
        public static readonly NetworkSimulatorPreset Mobile5G;

        /// <summary>
        /// Contains all available network preset values.
        /// </summary>
        public static readonly NetworkSimulatorPreset[] Values;

        static NetworkSimulatorPresets()
        {
            None = NetworkSimulatorPreset.Create("None", string.Empty);
            HomeBroadband = NetworkSimulatorPreset.Create(
                name: "Home Broadband [WIFI, Cable, Console, PC]",
                description: k_BroadbandDescription,
                packetDelayMs: 32,
                packetJitterMs: 12,
                packetLossInterval: 0,
                packetLossPercent: 2);
            Mobile2G = NetworkSimulatorPreset.Create(
                name: "Mobile 2G [CDMA & GSM, '00]",
                description: k_PoorMobileDescription,
                packetDelayMs: 400,
                packetJitterMs: 200,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile2_5G = NetworkSimulatorPreset.Create(
                name: "Mobile 2.5G [GPRS, G, '00]",
                description: k_PoorMobileDescription,
                packetDelayMs: 200,
                packetJitterMs: 100,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile2_75G = NetworkSimulatorPreset.Create(
                name: "Mobile 2.75G [Edge, E, '06]",
                description: k_PoorMobileDescription,
                packetDelayMs: 200,
                packetJitterMs: 100,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile3G = NetworkSimulatorPreset.Create(
                name: "Mobile 3G [WCDMA & UMTS, '03]",
                description: k_PoorMobileDescription,
                packetDelayMs: 200,
                packetJitterMs: 100,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile3_5G = NetworkSimulatorPreset.Create(
                name: "Mobile 3.5G [HSDPA, H, '06]",
                description: k_MediumMobileDescription,
                packetDelayMs: 75,
                packetJitterMs: 50,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile3_75G = NetworkSimulatorPreset.Create(
                name: "Mobile 3.75G [HDSDPA+, H+, '11]",
                description: k_DecentMobileDescription,
                packetDelayMs: 75,
                packetJitterMs: 50,
                packetLossInterval: 0,
                packetLossPercent: 3
            );
            Mobile4G = NetworkSimulatorPreset.Create(
                name: "Mobile 4G [4G, LTE, '13]",
                description: k_DecentMobileDescription,
                packetDelayMs: 50,
                packetJitterMs: 25,
                packetLossInterval: 0,
                packetLossPercent: 2
            );
            Mobile4_5G = NetworkSimulatorPreset.Create(
                name: "Mobile 4.5G [4G+, LTE-A, '16]",
                description: k_DecentMobileDescription,
                packetDelayMs: 50,
                packetJitterMs: 25,
                packetLossInterval: 0,
                packetLossPercent: 2
            );
            Mobile5G = NetworkSimulatorPreset.Create(
                name: "Mobile 5G ['20]",
                description: k_GoodMobileDescription,
                packetDelayMs: 1,
                packetJitterMs: 10,
                packetLossInterval: 0,
                packetLossPercent: 2
            );
            Values = new []{
                None,
                HomeBroadband,
                Mobile2G,
                Mobile2_5G,
                Mobile2_75G,
                Mobile3G,
                Mobile3_5G,
                Mobile3_75G,
                Mobile4G,
                Mobile4_5G,
                Mobile5G,
            };
        }
    }
}
