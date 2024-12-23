#if UNITY_EDITOR

using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    class TestDataTrends
    {
        const float k_LargeMax = 20f;
        const float k_MediumMax = 10f;
        const float k_SmallMax = 5f;
        const float k_MinRtt =  30e-3f;
        const float k_MaxRtt = 200e-3f;

        const float k_Percent = 1e-2f;
        const float k_PacketLossMin = 00.1f * k_Percent;
        const float k_PacketLossMax = 10.0f * k_Percent;

        public LogNormalRandomWalk NamedMessagesSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk NamedMessagesReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk UnnamedMessagesSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk UnnamedMessagesReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk NetworkVariableDeltasSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk NetworkVariableDeltasReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk OwnershipChangeEventsReceived { get; } = new LogNormalRandomWalk { Max = k_MediumMax };
        public LogNormalRandomWalk OwnershipChangeEventsSent { get; } = new LogNormalRandomWalk { Max = k_MediumMax };

        public LogNormalRandomWalk ObjectSpawnEventsSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk ObjectSpawnEventsReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk ObjectDestroyedEventsSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk ObjectDestroyedEventsReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk RpcEventsSent { get; } = new LogNormalRandomWalk { Max = k_LargeMax };
        public LogNormalRandomWalk RpcEventsReceived { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk ServerLogEventsSent { get; } = new LogNormalRandomWalk { Max = k_SmallMax };
        public LogNormalRandomWalk ServerLogEventsReceived { get; } = new LogNormalRandomWalk { Max = k_SmallMax };

        public LogNormalRandomWalk SceneEventsSent { get; } = new LogNormalRandomWalk { Max = k_SmallMax };
        public LogNormalRandomWalk SceneEventsReceived { get; } = new LogNormalRandomWalk { Max = k_SmallMax };

        public LogNormalRandomWalk PacketSentCount { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk PacketReceivedCount { get; } = new LogNormalRandomWalk { Max = k_LargeMax };

        public LogNormalRandomWalk RttToServer { get; } = new LogNormalRandomWalk() { Min = k_MinRtt, Max = k_MaxRtt };

        public LogNormalRandomWalk NetworkObjectsCount { get; } = new LogNormalRandomWalk() { Min = k_SmallMax, Max = k_LargeMax };

        public LogNormalRandomWalk ConnectionsCount { get; } = new LogNormalRandomWalk() { Max = k_MediumMax };

        public LogNormalRandomWalk PacketLoss { get; } = new LogNormalRandomWalk() { Min = k_PacketLossMin, Max = k_PacketLossMax };
    }
}

#endif