namespace Unity.Multiplayer.Tools.Adapters
{
    interface IHandleNetworkParameters : IAdapterComponent
    {
        NetworkParameters NetworkParameters { get; set; }
    }

    class NetworkParameters
    {
        public int PacketDelayMilliseconds { get; set; }

        public int PacketDelayRangeMilliseconds { get; set; }

        public int PacketLossIntervalMilliseconds { get; set; }

        public int PacketLossPercent { get; set; }
    }
}