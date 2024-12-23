using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
    class ProfilerCounters
    {
        static ProfilerCounters s_Singleton;
        public static ProfilerCounters Instance => s_Singleton ??= new ProfilerCounters();
        
        public readonly MetricByteCounters totalBytes;
        public readonly MetricCounters rpc;
        public readonly MetricCounters namedMessage;
        public readonly MetricCounters unnamedMessage;
        public readonly MetricCounters networkVariableDelta;
        public readonly MetricCounters objectSpawned;
        public readonly MetricCounters objectDestroyed;
        public readonly MetricCounters serverLog;
        public readonly MetricCounters sceneEvent;
        public readonly MetricCounters ownershipChange;
        public readonly MetricCounters customMessage;
        public readonly MetricCounters networkMessage;

        ICounterFactory m_ByteCounterFactory;
        ICounterFactory m_EventCounterFactory;
        
        public ProfilerCounters(
            ICounterFactory byteCounterFactory = null, 
            ICounterFactory eventCounterFactory = null)
        {
            m_ByteCounterFactory = byteCounterFactory ?? new ByteCounterFactory();
            m_EventCounterFactory = eventCounterFactory ?? new EventCounterFactory();

            totalBytes = ConstructMetricByteCounters("Total");
            rpc = ConstructMetricCounters(MetricType.Rpc);
            namedMessage = ConstructMetricCounters(MetricType.NamedMessage);
            unnamedMessage = ConstructMetricCounters(MetricType.UnnamedMessage);
            networkVariableDelta = ConstructMetricCounters("Network Variable");
            objectSpawned = ConstructMetricCounters(MetricType.ObjectSpawned);
            objectDestroyed = ConstructMetricCounters(MetricType.ObjectDestroyed);
            serverLog = ConstructMetricCounters(MetricType.ServerLog);
            sceneEvent = ConstructMetricCounters(MetricType.SceneEvent);
            ownershipChange = ConstructMetricCounters(MetricType.OwnershipChange);
            customMessage = ConstructMetricCounters("Custom");
            networkMessage = ConstructMetricCounters("Network Messages");
        }

        MetricByteCounters ConstructMetricByteCounters(string name)
            => new MetricByteCounters(
                name,
                m_ByteCounterFactory);

        MetricCounters ConstructMetricCounters(MetricType metricType)
            => ConstructMetricCounters(metricType.GetDisplayNameString());
        MetricCounters ConstructMetricCounters(string name)
            => new MetricCounters(name, m_ByteCounterFactory, m_EventCounterFactory);

        public void UpdateFromMetrics(MetricCollection collection)
        {
            totalBytes.Sample(
                collection.TryGetCounter(DirectedMetricType.TotalBytesSent.GetId(), out var bytesSent)
                    ? bytesSent.Value
                    : 0L,
                collection.TryGetCounter(DirectedMetricType.TotalBytesReceived.GetId(), out var bytesReceived)
                    ? bytesReceived.Value
                    : 0L);

            rpc.Sample(
                collection.GetEventValues<RpcEvent>(DirectedMetricType.RpcSent.GetId()),
                collection.GetEventValues<RpcEvent>(DirectedMetricType.RpcReceived.GetId()));

            namedMessage.Sample(
                collection.GetEventValues<NamedMessageEvent>(DirectedMetricType.NamedMessageSent.GetId()),
                collection.GetEventValues<NamedMessageEvent>(DirectedMetricType.NamedMessageReceived.GetId()));

            unnamedMessage.Sample(
                collection.GetEventValues<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageSent.GetId()),
                collection.GetEventValues<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageReceived.GetId()));

            // Custom messages is a combination of named and unnamed messages
            customMessage.Sample(
                collection.GetEventValues<NamedMessageEvent>(DirectedMetricType.NamedMessageSent.GetId()),
                collection.GetEventValues<NamedMessageEvent>(DirectedMetricType.NamedMessageReceived.GetId()));
            customMessage.Sample(
                collection.GetEventValues<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageSent.GetId()),
                collection.GetEventValues<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageReceived.GetId()));

            networkVariableDelta.Sample(
                collection.GetEventValues<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaSent.GetId()),
                collection.GetEventValues<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaReceived.GetId()));

            objectSpawned.Sample(
                collection.GetEventValues<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedSent.GetId()),
                collection.GetEventValues<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedReceived.GetId()));

            objectDestroyed.Sample(
                collection.GetEventValues<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedSent.GetId()),
                collection.GetEventValues<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedReceived.GetId()));

            serverLog.Sample(
                collection.GetEventValues<ServerLogEvent>(DirectedMetricType.ServerLogSent.GetId()),
                collection.GetEventValues<ServerLogEvent>(DirectedMetricType.ServerLogReceived.GetId()));

            sceneEvent.Sample(
                collection.GetEventValues<SceneEventMetric>(DirectedMetricType.SceneEventSent.GetId()),
                collection.GetEventValues<SceneEventMetric>(DirectedMetricType.SceneEventReceived.GetId()));

            ownershipChange.Sample(
                collection.GetEventValues<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeSent.GetId()),
                collection.GetEventValues<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeReceived.GetId()));

            networkMessage.Sample(
                collection.GetEventValues<NetworkMessageEvent>(DirectedMetricType.NetworkMessageSent.GetId()),
                collection.GetEventValues<NetworkMessageEvent>(DirectedMetricType.NetworkMessageReceived.GetId()));
        }
    }
}