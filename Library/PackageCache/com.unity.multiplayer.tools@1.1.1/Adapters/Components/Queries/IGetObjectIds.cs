using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Interface that provides Ids for all networked objects this frame
    /// </summary>
    interface IGetObjectIds : IAdapterComponent
    {
        IEnumerable<ObjectId> ObjectIds { get; }
    }
}