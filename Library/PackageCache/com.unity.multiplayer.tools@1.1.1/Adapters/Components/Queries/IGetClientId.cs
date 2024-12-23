namespace Unity.Multiplayer.Tools.Adapters
{
    interface IGetClientId : IAdapterComponent
    {
        ClientId LocalClientId { get; }
        ClientId ServerClientId { get; }
    }
}
