using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    class TestDataTracker : ITestDataTracker
    {
        readonly Counter m_TransportBytesSent = new Counter(DirectedMetricType.TotalBytesSent.GetId())
        {
            ShouldResetOnDispatch = true,
        };
        readonly Counter m_TransportBytesReceived = new Counter( DirectedMetricType.TotalBytesReceived.GetId())
        {
            ShouldResetOnDispatch = true,
        };

        readonly EventMetric<NetworkMessageEvent> m_NetworkMessageSentEvent = new EventMetric<NetworkMessageEvent>(DirectedMetricType.NetworkMessageSent.GetId());
        readonly EventMetric<NetworkMessageEvent> m_NetworkMessageReceivedEvent = new EventMetric<NetworkMessageEvent>(DirectedMetricType.NetworkMessageReceived.GetId());
        readonly EventMetric<NamedMessageEvent> m_NamedMessageSentEvent = new EventMetric<NamedMessageEvent>(DirectedMetricType.NamedMessageSent.GetId());
        readonly EventMetric<NamedMessageEvent> m_NamedMessageReceivedEvent = new EventMetric<NamedMessageEvent>(DirectedMetricType.NamedMessageReceived.GetId());
        readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageSentEvent = new EventMetric<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageSent.GetId());
        readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageReceivedEvent = new EventMetric<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageReceived.GetId());
        readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaSentEvent = new EventMetric<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaSent.GetId());
        readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaReceivedEvent = new EventMetric<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaReceived.GetId());
        readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeSentEvent = new EventMetric<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeSent.GetId());
        readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeReceivedEvent = new EventMetric<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeReceived.GetId());
        readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnSentEvent = new EventMetric<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedSent.GetId());
        readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnReceivedEvent = new EventMetric<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedReceived.GetId());
        readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroySentEvent = new EventMetric<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedSent.GetId());
        readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroyReceivedEvent = new EventMetric<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedReceived.GetId());
        readonly EventMetric<RpcEvent> m_RpcSentEvent = new EventMetric<RpcEvent>(DirectedMetricType.RpcSent.GetId());
        readonly EventMetric<RpcEvent> m_RpcReceivedEvent = new EventMetric<RpcEvent>(DirectedMetricType.RpcReceived.GetId());
        readonly EventMetric<ServerLogEvent> m_ServerLogSentEvent = new EventMetric<ServerLogEvent>(DirectedMetricType.ServerLogSent.GetId());
        readonly EventMetric<ServerLogEvent> m_ServerLogReceivedEvent = new EventMetric<ServerLogEvent>(DirectedMetricType.ServerLogReceived.GetId());
        readonly EventMetric<SceneEventMetric> m_SceneEventSentEvent = new EventMetric<SceneEventMetric>(DirectedMetricType.SceneEventSent.GetId());
        readonly EventMetric<SceneEventMetric> m_SceneEventReceivedEvent = new EventMetric<SceneEventMetric>(DirectedMetricType.SceneEventReceived.GetId());

        private readonly Counter m_PacketSentCounter = new Counter(NetworkMetricTypes.PacketsSent.Id)
        {
            ShouldResetOnDispatch = true,
        };
        private readonly Counter m_PacketReceivedCounter = new Counter(NetworkMetricTypes.PacketsReceived.Id)
        {
            ShouldResetOnDispatch = true,
        };
        private readonly Gauge m_RttToServerGauge = new Gauge(NetworkMetricTypes.RttToServer.Id)
        {
            ShouldResetOnDispatch = true,
        };
        readonly Gauge m_NetworkObjectsGauge = new Gauge(NetworkMetricTypes.NetworkObjects.Id)
        {
            ShouldResetOnDispatch = true,
        };
        readonly Gauge m_ConnectionsGauge = new Gauge(NetworkMetricTypes.ConnectedClients.Id)
        {
            ShouldResetOnDispatch = true,
        };
        readonly Gauge m_PacketLoss = new Gauge(NetworkMetricTypes.PacketLoss.Id)
        {
            ShouldResetOnDispatch = true,
        };

        public TestDataTracker()
        {
            Dispatcher = new MetricDispatcherBuilder()
                .WithCounters(m_TransportBytesSent, m_TransportBytesReceived)
                .WithMetricEvents(m_NetworkMessageSentEvent, m_NetworkMessageReceivedEvent)
                .WithMetricEvents(m_NamedMessageSentEvent, m_NamedMessageReceivedEvent)
                .WithMetricEvents(m_UnnamedMessageSentEvent, m_UnnamedMessageReceivedEvent)
                .WithMetricEvents(m_NetworkVariableDeltaSentEvent, m_NetworkVariableDeltaReceivedEvent)
                .WithMetricEvents(m_OwnershipChangeSentEvent, m_OwnershipChangeReceivedEvent)
                .WithMetricEvents(m_ObjectSpawnSentEvent, m_ObjectSpawnReceivedEvent)
                .WithMetricEvents(m_ObjectDestroySentEvent, m_ObjectDestroyReceivedEvent)
                .WithMetricEvents(m_RpcSentEvent, m_RpcReceivedEvent)
                .WithMetricEvents(m_ServerLogSentEvent, m_ServerLogReceivedEvent)
                .WithMetricEvents(m_SceneEventSentEvent, m_SceneEventReceivedEvent)
                .WithCounters(m_PacketSentCounter, m_PacketReceivedCounter)
                .WithGauges(m_RttToServerGauge)
                .WithGauges(m_NetworkObjectsGauge)
                .WithGauges(m_ConnectionsGauge)
                .WithGauges(m_PacketLoss)
                .Build();
        }

        public IMetricDispatcher Dispatcher { get; }

        public void SetConnectionId(ulong connectionId)
        {
            Dispatcher.SetConnectionId(connectionId);
        }

        public void TrackTransportBytesSent(long bytesCount)
        {
            m_TransportBytesSent.Increment(bytesCount);
        }

        public void TrackTransportBytesReceived(long bytesCount)
        {
            m_TransportBytesReceived.Increment(bytesCount);
        }

        public void TrackNetworkMessageSent(NetworkMessageEvent networkMessageEvent)
        {
            m_NetworkMessageSentEvent.Mark(networkMessageEvent);
        }

        public void TrackNetworkMessageReceived(NetworkMessageEvent networkMessageEvent)
        {
            m_NetworkMessageReceivedEvent.Mark(networkMessageEvent);
        }

        public void TrackNamedMessageSent(NamedMessageEvent namedMessageEvent)
        {
            m_NamedMessageSentEvent.Mark(namedMessageEvent);
        }

        public void TrackNamedMessageReceived(NamedMessageEvent namedMessageEvent)
        {
            m_NamedMessageReceivedEvent.Mark(namedMessageEvent);
        }

        public void TrackUnnamedMessageSent(UnnamedMessageEvent unnamedMessageEvent)
        {
            m_UnnamedMessageSentEvent.Mark(unnamedMessageEvent);
        }

        public void TrackUnnamedMessageReceived(UnnamedMessageEvent unnamedMessageEvent)
        {
            m_UnnamedMessageReceivedEvent.Mark(unnamedMessageEvent);
        }

        public void TrackNetworkVariableDeltaSent(NetworkVariableEvent networkVariableEvent)
        {
            m_NetworkVariableDeltaSentEvent.Mark(networkVariableEvent);
        }

        public void TrackNetworkVariableDeltaReceived(NetworkVariableEvent networkVariableEvent)
        {
            m_NetworkVariableDeltaReceivedEvent.Mark(networkVariableEvent);
        }

        public void TrackOwnershipChangeSent(OwnershipChangeEvent ownershipChangeEvent)
        {
            m_OwnershipChangeSentEvent.Mark(ownershipChangeEvent);
        }

        public void TrackOwnershipChangeReceived(OwnershipChangeEvent ownershipChangeEvent)
        {
            m_OwnershipChangeReceivedEvent.Mark(ownershipChangeEvent);
        }

        public void TrackObjectSpawnSent(ObjectSpawnedEvent objectSpawnedEvent)
        {
            m_ObjectSpawnSentEvent.Mark(objectSpawnedEvent);
        }

        public void TrackObjectSpawnReceived(ObjectSpawnedEvent objectSpawnedEvent)
        {
            m_ObjectSpawnReceivedEvent.Mark(objectSpawnedEvent);
        }

        public void TrackObjectDestroySent(ObjectDestroyedEvent objectDestroyedEvent)
        {
            m_ObjectDestroySentEvent.Mark(objectDestroyedEvent);
        }

        public void TrackObjectDestroyReceived(ObjectDestroyedEvent objectDestroyedEvent)
        {
            m_ObjectDestroyReceivedEvent.Mark(objectDestroyedEvent);
        }

        public void TrackRpcSent(RpcEvent rpcEvent)
        {
            m_RpcSentEvent.Mark(rpcEvent);
        }

        public void TrackRpcReceived(RpcEvent rpcEvent)
        {
            m_RpcReceivedEvent.Mark(rpcEvent);
        }

        public void TrackServerLogSent(ServerLogEvent serverLogEvent)
        {
            m_ServerLogSentEvent.Mark(serverLogEvent);
        }

        public void TrackServerLogReceived(ServerLogEvent serverLogEvent)
        {
            m_ServerLogReceivedEvent.Mark(serverLogEvent);
        }

        public void TrackSceneEventSent(SceneEventMetric sceneEvent)
        {
            m_SceneEventSentEvent.Mark(sceneEvent);
        }

        public void TrackSceneEventReceived(SceneEventMetric sceneEvent)
        {
            m_SceneEventReceivedEvent.Mark(sceneEvent);
        }

        public void TrackPacketSent(int packetCount)
        {
            m_PacketSentCounter.Increment(packetCount);
        }

        public void TrackPacketReceived(int packetCount)
        {
            m_PacketReceivedCounter.Increment(packetCount);
        }

        public void TrackRttToServer(int rtt)
        {
            m_RttToServerGauge.Set(rtt);
        }

        public void UpdateNetworkObjectsCount(int count)
        {
            m_NetworkObjectsGauge.Set(count);
        }

        public void UpdateConnectionsCount(int count)
        {
            m_ConnectionsGauge.Set(count);
        }

        public void UpdatePacketLoss(float count)
        {
            m_PacketLoss.Set(count);
        }
    }
}