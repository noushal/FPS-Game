using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Unity.Multiplayer.Tools.NetStats;
using Unity.Netcode;

namespace Unity.Multiplayer.Tools.Adapters.Ngo1
{
    class Ngo1Adapter

        : INetworkAdapter

        // Events
        // --------------------------------------------------------------------
        , IMetricCollectionEvent

        // Queries
        // --------------------------------------------------------------------
        , IGetBandwidth
        , IGetClientId
        , IGetGameObject
        , IGetObjectIds
        , IGetOwnership
        , IGetRpcCount
    {
        // TODO: Get reference via OnSingletonReady when we have internal access to NGO
        NetworkManager NetworkManager => NetworkManager.Singleton;
        NetworkSpawnManager SpawnManager => NetworkManager.SpawnManager;

        public AdapterMetadata Metadata { get; } = new AdapterMetadata{
            PackageInfo = new PackageInfo
            {
                PackageName = "com.unity.netcode.gameobjects",
                Version = new PackageVersion
                {
                    Major = 1,
                    Minor = 0,
                    Patch = 0,
                    PreRelease = ""
                }
            }
        };

        public T GetComponent<T>() where T : class, IAdapterComponent
        {
            return this as T;
        }

        // Events
        // --------------------------------------------------------------------
        public event Action<MetricCollection> MetricCollectionEvent;
        internal void OnMetricsReceived(MetricCollection metricCollection)
        {
            MetricCollectionEvent?.Invoke(metricCollection);
        }

        // Queries
        // --------------------------------------------------------------------
        public int GetBandwidthBytes(ObjectId objectId)
        {
            throw new System.NotImplementedException();
        }

        public ClientId LocalClientId => (ClientId)NetworkManager.LocalClientId;
        public ClientId ServerClientId => (ClientId)NetworkManager.ServerClientId;

        public GameObject GetGameObject(ObjectId objectId) =>
            SpawnManager.SpawnedObjects[(ulong)objectId].gameObject;

        public IEnumerable<ObjectId> ObjectIds =>
            SpawnManager.SpawnedObjects.Keys.Select(ulongId => (ObjectId)ulongId);

        public ClientId GetOwner(ObjectId objectId) =>
            (ClientId)SpawnManager.SpawnedObjects[(ulong)objectId].OwnerClientId;

        public int GetRpcCount(ObjectId objectId)
        {
            throw new System.NotImplementedException();
        }
    }
}