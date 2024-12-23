namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get object ownership this frame
    /// </summary>
    interface IGetOwnership : IAdapterComponent
    {
        ClientId GetOwner(ObjectId objectId);
    }
}