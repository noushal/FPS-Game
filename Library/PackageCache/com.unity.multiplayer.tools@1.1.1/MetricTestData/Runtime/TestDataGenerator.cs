#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.MetricTestData
{
    class TestDataGenerator
    {
        readonly TestDataDefinition m_DataDefinition;
        readonly Random m_Random;
        readonly IReadOnlyList<NetworkObjectIdentifier> m_GameObjects;

        public TestDataGenerator(TestDataDefinition dataDefinition, Random random, int nbGameObjects)
        {
            m_DataDefinition = dataDefinition;
            m_Random = random;

            m_GameObjects = Enumerable.Range(0, nbGameObjects)
                .Select(x => new NetworkObjectIdentifier(m_DataDefinition.GenerateGameObjectName(), (ulong) x))
                .ToList();
        }

        public IEnumerable<NamedMessageEvent> GenerateNamedMessageEvent(params ConnectionInfo[] connections)
        {
            var name = m_DataDefinition.GenerateNamedMessageName();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new NamedMessageEvent(connection, name, byteCount);
            }
        }

        public IEnumerable<UnnamedMessageEvent> GenerateUnnamedMessageEvent(params ConnectionInfo[] connections)
        {
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new UnnamedMessageEvent(connection, byteCount);
            }
        }

        public IEnumerable<NetworkVariableEvent> GenerateNetworkVariableEvent(params ConnectionInfo[] connections)
        {
            var gameObject = GetRandomGameObject();
            var componentName = m_DataDefinition.GenerateComponentName();
            var variableName = m_DataDefinition.GenerateVariableName();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new NetworkVariableEvent(connection, gameObject, variableName, componentName, byteCount);
            }
        }

        public IEnumerable<OwnershipChangeEvent> GenerateOwnershipChangeEvent(params ConnectionInfo[] connections)
        {
            var gameObject = GetRandomGameObject();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new OwnershipChangeEvent(connection, gameObject, byteCount);
            }
        }

        public IEnumerable<ObjectSpawnedEvent> GenerateObjectSpawnedEvent(params ConnectionInfo[] connections)
        {
            var gameObject = GetRandomGameObject();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new ObjectSpawnedEvent(connection, gameObject, byteCount);
            }
        }

        public IEnumerable<ObjectDestroyedEvent> GenerateObjectDestroyedEvent(params ConnectionInfo[] connections)
        {
            var gameObject = GetRandomGameObject();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new ObjectDestroyedEvent(connection, gameObject, byteCount);
            }
        }

        public IEnumerable<RpcEvent> GenerateRpcEvent(params ConnectionInfo[] connections)
        {
            var gameObject = GetRandomGameObject();
            var rpcName = m_DataDefinition.GenerateRpcName();
            var componentName = m_DataDefinition.GenerateComponentName();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new RpcEvent(connection, gameObject, rpcName, componentName, byteCount);
            }
        }

        public IEnumerable<ServerLogEvent> GenerateServerLogEvent(params ConnectionInfo[] connections)
        {
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new ServerLogEvent(connection, LogLevel.Info, byteCount);
            }
        }

        public IEnumerable<SceneEventMetric> GenerateSceneEvent(string prefix, params ConnectionInfo[] connections)
        {
            var sceneName = m_DataDefinition.GenerateSceneName();
            var byteCount = m_DataDefinition.GenerateByteCount();

            foreach (var connection in connections)
            {
                yield return new SceneEventMetric(connection, $"{prefix}_SceneEventType", sceneName, byteCount);
            }
        }


        NetworkObjectIdentifier GetRandomGameObject() => m_GameObjects[m_Random.Next(0, m_GameObjects.Count)];
    }
}

#endif