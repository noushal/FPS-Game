using Unity.Multiplayer.Tools.NetStats;
using MT = Unity.Multiplayer.Tools.MetricTypes.MetricType;
using ND = Unity.Multiplayer.Tools.MetricTypes.NetworkDirection;
using NDC = Unity.Multiplayer.Tools.MetricTypes.NetworkDirectionConstants;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    enum MetricType
    {
        None,
        TotalBytes,
        Rpc,
        NamedMessage,
        UnnamedMessage,
        NetworkVariableDelta,
        ObjectSpawned,
        ObjectDestroyed,
        OwnershipChange,
        ServerLog,
        SceneEvent,
        NetworkMessage,
        Packets,
        RttToServer,
        NetworkObjects,
        Connections,
        PacketLoss
    }

    // NOTE: Public because it needs to be exposed for RNSM configuration

    /// <summary>
    /// The built in set of metrics that can be displayed in the multiplayer tools,
    /// such as the Network Profiler and the Runtime Net Stats Monitor.
    /// </summary>
    [MetricTypeEnum(DisplayName = "Built-In Metrics")]
    [MetricTypeSortPriority(SortPriority = SortPriority.VeryHigh)]
    public enum DirectedMetricType
    {
        /// <summary>
        /// Number of total bytes sent.
        /// </summary>
        [MetricMetadata(Units        = Units.Bytes)]
        TotalBytesSent               = (MT.TotalBytes           << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of total bytes received.
        /// </summary>
        [MetricMetadata(Units        = Units.Bytes)]
        TotalBytesReceived           = (MT.TotalBytes           << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of RPC sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "RPCs Sent")]
        RpcSent                      = (MT.Rpc                  << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of RPC received.
        /// </summary>
        [MetricMetadata(DisplayName  = "RPCs Received")]
        RpcReceived                  = (MT.Rpc                  << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of custom named message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Named Messages Sent")]
        NamedMessageSent             = (MT.NamedMessage         << NDC.k_BitWidth) | ND.Sent,

        /// <summary>
        /// Number of custom named message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Named Messages Received")]
        NamedMessageReceived         = (MT.NamedMessage         << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of custom unnamed message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Unnamed Messages Sent")]
        UnnamedMessageSent           = (MT.UnnamedMessage       << NDC.k_BitWidth) | ND.Sent,

        /// <summary>
        /// Number of custom unnamed message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Unnamed Messages Received")]
        UnnamedMessageReceived       = (MT.UnnamedMessage       << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of network variable delta message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Network Variable Deltas Sent")]
        NetworkVariableDeltaSent     = (MT.NetworkVariableDelta << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of network variable delta message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Network Variable Deltas Received")]
        NetworkVariableDeltaReceived = (MT.NetworkVariableDelta << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of network object spawned message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Objects Spawned Sent")]
        ObjectSpawnedSent            = (MT.ObjectSpawned        << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of network object spawned message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Objects Spawned Received")]
        ObjectSpawnedReceived        = (MT.ObjectSpawned        << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of network object destroyed message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Objects Destroyed Sent")]
        ObjectDestroyedSent          = (MT.ObjectDestroyed      << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of network object destroyed message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Objects Destroyed Received")]
        ObjectDestroyedReceived      = (MT.ObjectDestroyed      << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of ownership change message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Ownership Changes Sent")]
        OwnershipChangeSent          = (MT.OwnershipChange      << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of ownership change message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Ownership Changes Received")]
        OwnershipChangeReceived      = (MT.OwnershipChange      << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of server log message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Server Logs Sent")]
        ServerLogSent                = (MT.ServerLog            << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of server log message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Server Logs Received")]
        ServerLogReceived            = (MT.ServerLog            << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of scene event message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Scene Events Sent")]
        SceneEventSent               = (MT.SceneEvent           << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of scene event message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Scene Events Received")]
        SceneEventReceived           = (MT.SceneEvent           << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of network message sent.
        /// </summary>
        [MetricMetadata(DisplayName  = "Network Messages Sent")]
        NetworkMessageSent           = (MT.NetworkMessage       << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of network message received.
        /// </summary>
        [MetricMetadata(DisplayName  = "Network Messages Received")]
        NetworkMessageReceived       = (MT.NetworkMessage       << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// Number of packets sent.
        /// </summary>
        PacketsSent                  = (MT.Packets              << NDC.k_BitWidth) | ND.Sent,
        /// <summary>
        /// Number of packets received.
        /// </summary>
        PacketsReceived              = (MT.Packets              << NDC.k_BitWidth) | ND.Received,

        /// <summary>
        /// The RTT from a client to a server.
        /// This include the processing time at the transport level
        /// but does not contain the processing time spent inside the Netcode for GameObjects.
        /// </summary>
        [MetricMetadata(
            DisplayName = "RTT To Server",
            MetricKind  = MetricKind.Gauge,
            Units       = Units.Seconds)]
        RttToServer                  = (MT.RttToServer          << NDC.k_BitWidth) | ND.SentAndReceived,

        /// <summary>
        /// Number of active Network Objects.
        /// </summary>
        [MetricMetadata(MetricKind   = MetricKind.Gauge)]
        NetworkObjects               = (MT.NetworkObjects       << NDC.k_BitWidth) | ND.SentAndReceived,

        /// <summary>
        /// Number of active network connections.
        /// A client will always show one active connection (client to server).
        /// </summary>
        [MetricMetadata(MetricKind   = MetricKind.Gauge)]
        Connections                  = (MT.Connections          << NDC.k_BitWidth) | ND.SentAndReceived,

        /// <summary>
        /// Percentage of packet loss over the lifetime of the connection.
        /// This is only valid for clients.
        /// This value will always be zero on a server.
        /// </summary>
        [MetricMetadata(
            MetricKind = MetricKind.Gauge,
            DisplayAsPercentage = true)]
        PacketLoss                   = (MT.PacketLoss           << NDC.k_BitWidth) | ND.Received,
    }
}