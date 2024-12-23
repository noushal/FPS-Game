using Unity.Multiplayer.Tools.Adapters;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    interface INetworkTransportApi
    {
        bool IsAvailable { get; }

        bool IsConnected { get; }

        void SimulateDisconnect();

        void SimulateReconnect();

        void UpdateNetworkParameters(NetworkParameters networkParameters);
    }
}