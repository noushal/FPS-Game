namespace Unity.Multiplayer.Tools.Adapters
{
    interface INetworkAvailability : IAdapterComponent
    {
        bool IsConnected { get; }
    }
}