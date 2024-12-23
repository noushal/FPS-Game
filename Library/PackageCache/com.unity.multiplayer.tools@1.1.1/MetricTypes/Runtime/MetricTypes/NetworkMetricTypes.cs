namespace Unity.Multiplayer.Tools.MetricTypes
{
    /// <summary>
    /// DEPRECATED
    /// <see cref="DirectionalMetricInfo"/> is now just a wrapper around <see cref="DirectedMetricType"/>.
    /// Please use <see cref="DirectedMetricType"/> instead.
    /// <see cref="DirectionalMetricInfo"/> can't be deleted yet as it's still referenced by the NGO library (as of 2021-12-07).
    /// </summary>
    static class NetworkMetricTypes
    {
        public static readonly DirectionalMetricInfo NetworkMessageSent =
            new DirectionalMetricInfo(DirectedMetricType.NetworkMessageSent);
        public static readonly DirectionalMetricInfo NetworkMessageReceived =
            new DirectionalMetricInfo(DirectedMetricType.NetworkMessageReceived);

        public static readonly DirectionalMetricInfo TotalBytesSent =
            new DirectionalMetricInfo(DirectedMetricType.TotalBytesSent);
        public static readonly DirectionalMetricInfo TotalBytesReceived =
            new DirectionalMetricInfo(DirectedMetricType.TotalBytesReceived);

        public static readonly DirectionalMetricInfo RpcSent =
            new DirectionalMetricInfo(DirectedMetricType.RpcSent);
        public static readonly DirectionalMetricInfo RpcReceived =
            new DirectionalMetricInfo(DirectedMetricType.RpcReceived);

        public static readonly DirectionalMetricInfo NamedMessageSent =
            new DirectionalMetricInfo(DirectedMetricType.NamedMessageSent);
        public static readonly DirectionalMetricInfo NamedMessageReceived =
            new DirectionalMetricInfo(DirectedMetricType.NamedMessageReceived);

        public static readonly DirectionalMetricInfo UnnamedMessageSent =
            new DirectionalMetricInfo(DirectedMetricType.UnnamedMessageSent);
        public static readonly DirectionalMetricInfo UnnamedMessageReceived =
            new DirectionalMetricInfo(DirectedMetricType.UnnamedMessageReceived);

        public static readonly DirectionalMetricInfo NetworkVariableDeltaSent =
            new DirectionalMetricInfo(DirectedMetricType.NetworkVariableDeltaSent);
        public static readonly DirectionalMetricInfo NetworkVariableDeltaReceived =
            new DirectionalMetricInfo(DirectedMetricType.NetworkVariableDeltaReceived);

        public static readonly DirectionalMetricInfo ObjectSpawnedSent =
            new DirectionalMetricInfo(DirectedMetricType.ObjectSpawnedSent);
        public static readonly DirectionalMetricInfo ObjectSpawnedReceived =
            new DirectionalMetricInfo(DirectedMetricType.ObjectSpawnedReceived);

        public static readonly DirectionalMetricInfo ObjectDestroyedSent =
            new DirectionalMetricInfo(DirectedMetricType.ObjectDestroyedSent);
        public static readonly DirectionalMetricInfo ObjectDestroyedReceived =
            new DirectionalMetricInfo(DirectedMetricType.ObjectDestroyedReceived);

        public static readonly DirectionalMetricInfo OwnershipChangeSent =
            new DirectionalMetricInfo(DirectedMetricType.OwnershipChangeSent);
        public static readonly DirectionalMetricInfo OwnershipChangeReceived =
            new DirectionalMetricInfo(DirectedMetricType.OwnershipChangeReceived);

        public static readonly DirectionalMetricInfo ServerLogSent =
            new DirectionalMetricInfo(DirectedMetricType.ServerLogSent);
        public static readonly DirectionalMetricInfo ServerLogReceived =
            new DirectionalMetricInfo(DirectedMetricType.ServerLogReceived);

        public static readonly DirectionalMetricInfo SceneEventSent =
            new DirectionalMetricInfo(DirectedMetricType.SceneEventSent);
        public static readonly DirectionalMetricInfo SceneEventReceived =
            new DirectionalMetricInfo(DirectedMetricType.SceneEventReceived);

        public static readonly DirectionalMetricInfo PacketsSent =
            new DirectionalMetricInfo(DirectedMetricType.PacketsSent);
        public static readonly DirectionalMetricInfo PacketsReceived =
            new DirectionalMetricInfo(DirectedMetricType.PacketsReceived);

        public static readonly DirectionalMetricInfo RttToServer =
            new DirectionalMetricInfo(DirectedMetricType.RttToServer);

        public static readonly DirectionalMetricInfo NetworkObjects =
            new DirectionalMetricInfo(DirectedMetricType.NetworkObjects);

        public static readonly DirectionalMetricInfo ConnectedClients =
            new DirectionalMetricInfo(DirectedMetricType.Connections);

        public static readonly DirectionalMetricInfo PacketLoss =
            new DirectionalMetricInfo(DirectedMetricType.PacketLoss);
    }
}