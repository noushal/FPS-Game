using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Serializable]
    struct SceneEventMetric : INetworkMetricEvent
    {
        /// String overload maintained for backwards compatibility
        public SceneEventMetric(ConnectionInfo connection, string sceneEventType, string sceneName, long bytesCount)
            : this(connection, StringConversionUtility.ConvertToFixedString(sceneEventType), StringConversionUtility.ConvertToFixedString(sceneName), bytesCount)
        {
        }

        public SceneEventMetric(ConnectionInfo connection, FixedString64Bytes sceneEventType, FixedString64Bytes sceneName, long bytesCount)
        {
            Connection = connection;
            SceneEventType = sceneEventType;
            SceneName = sceneName;
            BytesCount = bytesCount;
        }

        public ConnectionInfo Connection { get; }

        public FixedString64Bytes SceneEventType{ get; }

        public FixedString64Bytes SceneName { get; }

        public long BytesCount { get; }
    }
}