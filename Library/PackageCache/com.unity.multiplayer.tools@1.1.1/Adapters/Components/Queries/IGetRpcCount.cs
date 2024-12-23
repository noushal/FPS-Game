namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get the number of RPCs called on an object this frame
    /// </summary>
    interface IGetRpcCount : IAdapterComponent
    {
        int GetRpcCount(ObjectId objectId);
    }
}