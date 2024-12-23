using JetBrains.Annotations;

namespace Unity.Multiplayer.Tools.Adapters
{
    interface INetworkAdapter
    {
        AdapterMetadata Metadata { get; }

        [CanBeNull]
        T GetComponent<T>() where T : class, IAdapterComponent;
    }
}