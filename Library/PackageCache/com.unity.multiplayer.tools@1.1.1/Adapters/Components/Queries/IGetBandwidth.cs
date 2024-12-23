namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get object bandwidth in bytes this frame
    /// </summary>
    interface IGetBandwidth : IAdapterComponent
    {
        int GetBandwidthBytes(ObjectId objectId);
    }
}