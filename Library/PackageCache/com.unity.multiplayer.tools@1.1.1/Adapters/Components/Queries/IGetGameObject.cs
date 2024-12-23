using UnityEngine;

namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface to get a GameObject from its object ID
    /// </summary>
    interface IGetGameObject : IAdapterComponent
    {
        GameObject GetGameObject(ObjectId objectId);
    }
}