namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get priority this frame
    /// </summary>
    interface IGetPriority : IAdapterComponent
    {
        int GetPriority(ObjectId objectId);
    }
}