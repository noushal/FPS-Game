namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get object interest this frame
    /// </summary>
    interface IGetInterest : IAdapterComponent
    {
        bool GetInterest(ObjectId objectId);
    }
}