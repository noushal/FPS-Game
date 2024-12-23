#if UNITY_EDITOR

using System;
using System.Linq;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    class TestDataDispatcher
    {
        const int DefaultServerId = 0;
        const int DefaultClientId = 1;

        readonly Random m_Random;
        readonly TestDataGenerator m_DataGenerator;
        readonly ITestDataTracker m_Tracker;
        readonly TestDataTrends m_Trends;

        public TestDataDispatcher(int seed = 0)
        {
            m_Random = seed != 0 ? new Random(seed) : new Random();
            m_DataGenerator = new TestDataGenerator(
                new TestDataDefinition(seed != 0 ? seed : m_Random.Next()), m_Random, m_Random.Next(5, 10));
            m_Tracker = new TestDataTracker();
            m_Tracker.Dispatcher.RegisterObserver(new MetricObserver());
            m_Trends = new TestDataTrends();
        }

        public void DispatchClientFrame()
        {
            var connection = new ConnectionInfo(DefaultServerId);
            m_Tracker.SetConnectionId(DefaultClientId);

            m_Trends.NamedMessagesSent.Repeat(m_Random, () =>
            {
                var namedMessageEvent = m_DataGenerator.GenerateNamedMessageEvent(connection).First();
                m_Tracker.TrackNamedMessageSent(namedMessageEvent);
                m_Tracker.TrackTransportBytesSent(namedMessageEvent.BytesCount);
            });

            m_Trends.NamedMessagesReceived.Repeat(m_Random, () =>
            {
                var namedMessageEvent = m_DataGenerator.GenerateNamedMessageEvent(connection).First();
                m_Tracker.TrackNamedMessageReceived(namedMessageEvent);
                m_Tracker.TrackTransportBytesReceived(namedMessageEvent.BytesCount);
            });

            m_Trends.UnnamedMessagesSent.Repeat(m_Random, () =>
            {
                var unnamedMessageEvent = m_DataGenerator.GenerateUnnamedMessageEvent(connection).First();
                m_Tracker.TrackUnnamedMessageSent(unnamedMessageEvent);
                m_Tracker.TrackTransportBytesSent(unnamedMessageEvent.BytesCount);
            });

            m_Trends.UnnamedMessagesReceived.Repeat(m_Random, () =>
            {
                var unnamedMessageEvent = m_DataGenerator.GenerateUnnamedMessageEvent(connection).First();
                m_Tracker.TrackUnnamedMessageReceived(unnamedMessageEvent);
                m_Tracker.TrackTransportBytesReceived(unnamedMessageEvent.BytesCount);
            });

            m_Trends.NetworkVariableDeltasReceived.Repeat(m_Random, () =>
            {
                var networkVariableEvent = m_DataGenerator.GenerateNetworkVariableEvent(connection).First();
                m_Tracker.TrackNetworkVariableDeltaReceived(networkVariableEvent);
                m_Tracker.TrackTransportBytesReceived(networkVariableEvent.BytesCount);
            });

            m_Trends.OwnershipChangeEventsReceived.Repeat(m_Random, () =>
            {
                var ownershipChangeEvent = m_DataGenerator.GenerateOwnershipChangeEvent(connection).First();
                m_Tracker.TrackOwnershipChangeReceived(ownershipChangeEvent);
                m_Tracker.TrackTransportBytesReceived(ownershipChangeEvent.BytesCount);
            });

            m_Trends.ObjectSpawnEventsReceived.Repeat(m_Random, () =>
            {
                var objectSpawnedEvent = m_DataGenerator.GenerateObjectSpawnedEvent(connection).First();
                m_Tracker.TrackObjectSpawnReceived(objectSpawnedEvent);
                m_Tracker.TrackTransportBytesReceived(objectSpawnedEvent.BytesCount);
            });

            m_Trends.ObjectDestroyedEventsReceived.Repeat(m_Random, () =>
            {
                var objectDestroyedEvent = m_DataGenerator.GenerateObjectDestroyedEvent(connection).First();
                m_Tracker.TrackObjectDestroyReceived(objectDestroyedEvent);
                m_Tracker.TrackTransportBytesReceived(objectDestroyedEvent.BytesCount);
            });

            m_Trends.RpcEventsSent.Repeat(m_Random, () =>
            {
                var rpcEvent = m_DataGenerator.GenerateRpcEvent(connection).First();
                m_Tracker.TrackRpcSent(rpcEvent);
                m_Tracker.TrackTransportBytesSent(rpcEvent.BytesCount);
            });

            m_Trends.RpcEventsReceived.Repeat(m_Random, () =>
            {
                var rpcEvent = m_DataGenerator.GenerateRpcEvent(connection).First();
                m_Tracker.TrackRpcReceived(rpcEvent);
                m_Tracker.TrackTransportBytesReceived(rpcEvent.BytesCount);
            });

            m_Trends.ServerLogEventsSent.Repeat(m_Random, () =>
            {
                var serverLogEvent = m_DataGenerator.GenerateServerLogEvent(connection).First();
                m_Tracker.TrackServerLogSent(serverLogEvent);
                m_Tracker.TrackTransportBytesSent(serverLogEvent.BytesCount);
            });

            m_Trends.SceneEventsSent.Repeat(m_Random, () =>
            {
                var sceneEvent = m_DataGenerator.GenerateSceneEvent("C2S", connection).First();
                m_Tracker.TrackSceneEventSent(sceneEvent);
                m_Tracker.TrackTransportBytesSent(sceneEvent.BytesCount);
            });

            m_Trends.SceneEventsReceived.Repeat(m_Random, () =>
            {
                var sceneEvent = m_DataGenerator.GenerateSceneEvent("S2C", connection).First();
                m_Tracker.TrackSceneEventReceived(sceneEvent);
                m_Tracker.TrackTransportBytesReceived(sceneEvent.BytesCount);
            });

            m_Tracker.TrackPacketSent(m_Trends.PacketSentCount.NextInt(m_Random));
            m_Tracker.TrackPacketReceived(m_Trends.PacketReceivedCount.NextInt(m_Random));
            m_Tracker.TrackRttToServer(m_Trends.RttToServer.NextInt(m_Random));
            m_Tracker.UpdateNetworkObjectsCount(m_Trends.NetworkObjectsCount.NextInt(m_Random));
            m_Tracker.UpdateConnectionsCount(m_Trends.ConnectionsCount.NextInt(m_Random));
            m_Tracker.UpdatePacketLoss(m_Trends.PacketLoss.NextFloat(m_Random));

            m_Tracker.Dispatcher.Dispatch();
        }

        public void DispatchServerFrame(uint nbClients = 2)
        {
            m_Tracker.SetConnectionId(DefaultServerId);
            var clientAndServerConnections = Enumerable.Range(0, (int)nbClients + 1)
                .Select(x => new ConnectionInfo((ulong)x))
                .ToArray();

            m_Trends.NamedMessagesSent.Repeat(m_Random, () =>
            {
                var namedMessageEvents = m_DataGenerator.GenerateNamedMessageEvent(clientAndServerConnections);
                foreach (var namedMessageEvent in namedMessageEvents)
                {
                    m_Tracker.TrackNamedMessageSent(namedMessageEvent);
                    m_Tracker.TrackTransportBytesSent(namedMessageEvent.BytesCount);
                }
            });

            m_Trends.NamedMessagesReceived.Repeat(m_Random, () =>
            {
                var namedMessageEvents = m_DataGenerator.GenerateNamedMessageEvent(clientAndServerConnections);
                foreach (var namedMessageEvent in namedMessageEvents)
                {
                    m_Tracker.TrackNamedMessageReceived(namedMessageEvent);
                    m_Tracker.TrackTransportBytesReceived(namedMessageEvent.BytesCount);
                }
            });

            m_Trends.UnnamedMessagesSent.Repeat(m_Random, () =>
            {
                var unnamedMessageEvents = m_DataGenerator.GenerateUnnamedMessageEvent(clientAndServerConnections);
                foreach (var unnamedMessageEvent in unnamedMessageEvents)
                {
                    m_Tracker.TrackUnnamedMessageSent(unnamedMessageEvent);
                    m_Tracker.TrackTransportBytesSent(unnamedMessageEvent.BytesCount);
                }
            });

            m_Trends.UnnamedMessagesReceived.Repeat(m_Random, () =>
            {
                var unnamedMessageEvents = m_DataGenerator.GenerateUnnamedMessageEvent(clientAndServerConnections);
                foreach (var unnamedMessageEvent in unnamedMessageEvents)
                {
                    m_Tracker.TrackUnnamedMessageReceived(unnamedMessageEvent);
                    m_Tracker.TrackTransportBytesReceived(unnamedMessageEvent.BytesCount);
                }
            });

            m_Trends.NetworkVariableDeltasSent.Repeat(m_Random, () =>
            {
                var networkVariableEvents = m_DataGenerator.GenerateNetworkVariableEvent(clientAndServerConnections);
                foreach (var networkVariableEvent in networkVariableEvents)
                {
                    m_Tracker.TrackNetworkVariableDeltaSent(networkVariableEvent);
                    m_Tracker.TrackTransportBytesSent(networkVariableEvent.BytesCount);
                }
            });

            m_Trends.OwnershipChangeEventsSent.Repeat(m_Random, () =>
            {
                var ownershipChangeEvents = m_DataGenerator.GenerateOwnershipChangeEvent(clientAndServerConnections);
                foreach (var ownershipChangeEvent in ownershipChangeEvents)
                {
                    m_Tracker.TrackOwnershipChangeSent(ownershipChangeEvent);
                    m_Tracker.TrackTransportBytesSent(ownershipChangeEvent.BytesCount);
                }
            });

            m_Trends.ObjectSpawnEventsSent.Repeat(m_Random, () =>
            {
                var objectSpawnEvents = m_DataGenerator.GenerateObjectSpawnedEvent(clientAndServerConnections);
                foreach (var objectSpawnEvent in objectSpawnEvents)
                {
                    m_Tracker.TrackObjectSpawnSent(objectSpawnEvent);
                    m_Tracker.TrackTransportBytesSent(objectSpawnEvent.BytesCount);
                }
            });

            m_Trends.ObjectDestroyedEventsSent.Repeat(m_Random, () =>
            {
                var objectDestroyEvents = m_DataGenerator.GenerateObjectDestroyedEvent(clientAndServerConnections);
                foreach (var objectDestroyEvent in objectDestroyEvents)
                {
                    m_Tracker.TrackObjectDestroySent(objectDestroyEvent);
                    m_Tracker.TrackTransportBytesSent(objectDestroyEvent.BytesCount);
                }
            });

            m_Trends.RpcEventsSent.Repeat(m_Random, () =>
            {
                var rpcEvents = m_DataGenerator.GenerateRpcEvent(clientAndServerConnections);
                foreach (var rpcEvent in rpcEvents)
                {
                    m_Tracker.TrackRpcSent(rpcEvent);
                    m_Tracker.TrackTransportBytesSent(rpcEvent.BytesCount);
                }
            });

            m_Trends.RpcEventsReceived.Repeat(m_Random, () =>
            {
                var rpcEvents = m_DataGenerator.GenerateRpcEvent(clientAndServerConnections);
                foreach (var rpcEvent in rpcEvents)
                {
                    m_Tracker.TrackRpcReceived(rpcEvent);
                    m_Tracker.TrackTransportBytesReceived(rpcEvent.BytesCount);
                }
            });

            m_Trends.ServerLogEventsReceived.Repeat(m_Random, () =>
            {
                var serverLogEvents = m_DataGenerator.GenerateServerLogEvent(clientAndServerConnections);
                foreach (var serverLogEvent in serverLogEvents)
                {
                    m_Tracker.TrackServerLogReceived(serverLogEvent);
                    m_Tracker.TrackTransportBytesReceived(serverLogEvent.BytesCount);
                }
            });

            m_Trends.SceneEventsSent.Repeat(m_Random, () =>
            {
                var sceneEvents = m_DataGenerator.GenerateSceneEvent("S2C", clientAndServerConnections);
                foreach (var sceneEvent in sceneEvents)
                {
                    m_Tracker.TrackSceneEventSent(sceneEvent);
                    m_Tracker.TrackTransportBytesSent(sceneEvent.BytesCount);
                }
            });

            m_Trends.SceneEventsReceived.Repeat(m_Random, () =>
            {
                var sceneEvents = m_DataGenerator.GenerateSceneEvent("C2S", clientAndServerConnections);
                foreach (var sceneEvent in sceneEvents)
                {
                    m_Tracker.TrackSceneEventReceived(sceneEvent);
                    m_Tracker.TrackTransportBytesReceived(sceneEvent.BytesCount);
                }
            });

            m_Tracker.TrackPacketSent(m_Trends.PacketSentCount.NextInt(m_Random));
            m_Tracker.TrackPacketReceived(m_Trends.PacketReceivedCount.NextInt(m_Random));
            m_Tracker.TrackRttToServer(m_Trends.RttToServer.NextInt(m_Random));
            m_Tracker.UpdateNetworkObjectsCount(m_Trends.NetworkObjectsCount.NextInt(m_Random));
            m_Tracker.UpdateConnectionsCount(m_Trends.ConnectionsCount.NextInt(m_Random));
            m_Tracker.UpdatePacketLoss(m_Trends.PacketLoss.NextFloat(m_Random));

            m_Tracker.Dispatcher.Dispatch();
        }
    }
}

#endif