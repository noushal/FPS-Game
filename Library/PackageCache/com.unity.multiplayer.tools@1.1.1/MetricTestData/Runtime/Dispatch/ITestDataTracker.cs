using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    interface ITestDataTracker
    {
        IMetricDispatcher Dispatcher { get; }

        void SetConnectionId(ulong connectionId);

        void TrackTransportBytesSent(long bytesCount);

        void TrackTransportBytesReceived(long bytesCount);

        void TrackNetworkMessageSent(NetworkMessageEvent networkMessageEvent);

        void TrackNetworkMessageReceived(NetworkMessageEvent networkMessageEvent);

        void TrackNamedMessageSent(NamedMessageEvent namedMessageEvent);

        void TrackNamedMessageReceived(NamedMessageEvent namedMessageEvent);

        void TrackUnnamedMessageSent(UnnamedMessageEvent unnamedMessageEvent);

        void TrackUnnamedMessageReceived(UnnamedMessageEvent unnamedMessageEvent);

        void TrackNetworkVariableDeltaSent(NetworkVariableEvent networkVariableEvent);

        void TrackNetworkVariableDeltaReceived(NetworkVariableEvent networkVariableEvent);

        void TrackOwnershipChangeSent(OwnershipChangeEvent ownershipChangeEvent);

        void TrackOwnershipChangeReceived(OwnershipChangeEvent ownershipChangeEvent);

        void TrackObjectSpawnSent(ObjectSpawnedEvent objectSpawnedEvent);

        void TrackObjectSpawnReceived(ObjectSpawnedEvent objectSpawnedEvent);

        void TrackObjectDestroySent(ObjectDestroyedEvent objectDestroyedEvent);

        void TrackObjectDestroyReceived(ObjectDestroyedEvent objectDestroyedEvent);

        void TrackRpcSent(RpcEvent rpcEvent);

        void TrackRpcReceived(RpcEvent rpcEvent);

        void TrackServerLogSent(ServerLogEvent serverLogEvent);

        void TrackServerLogReceived(ServerLogEvent serverLogEvent);

        void TrackSceneEventSent(SceneEventMetric sceneEvent);

        void TrackSceneEventReceived(SceneEventMetric sceneEvent);

        void TrackPacketSent(int packetCount);

        void TrackPacketReceived(int packetCount);

        void TrackRttToServer(int rtt);

        void UpdateNetworkObjectsCount(int count);

        void UpdateConnectionsCount(int count);

        void UpdatePacketLoss(float count);
    }
}