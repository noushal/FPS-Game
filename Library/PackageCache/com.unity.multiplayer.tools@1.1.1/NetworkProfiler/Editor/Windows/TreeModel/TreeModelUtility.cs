using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class TreeModelUtility
    {
        public static List<IRowData> FlattenTree(TreeModel tree)
        {
            var rowData = new List<IRowData>();

            foreach (var child in tree.Children)
            {
                FlattenTreeRecursive(child, rowData);
            }

            return rowData;
        }

        static void FlattenTreeRecursive(TreeModelNode node, List<IRowData> outList)
        {
            outList.Add(node.RowData);

            foreach (var child in node.Children)
            {
                FlattenTreeRecursive(child, outList);
            }
        }

        // These matches how NGO reports those message.
        // They may match the MetricTypes enum but that is coincidental. 
        static readonly IReadOnlyCollection<string> k_ExcludedNetworkMessageTypeNames = new[]
        {
            "NamedMessage",
            "UnnamedMessage",
            "SceneEventMessage",
            "ServerLogMessage"
        };

        public static TreeModel CreateMessagesTreeStructure(MetricCollection metrics)
        {
            if (metrics == null)
            {
                return new TreeModel();
            }

            return new TreeModelBuilder(metrics)
                .AddUnderConnection(
                    MetricType.NamedMessage,
                    (NamedMessageEvent metric, TreeModelNode node)
                        => new NamedMessageEventViewModel(metric.Name.ToString(), node.RowData))
                .AddUnderConnection(
                    MetricType.UnnamedMessage,
                    (UnnamedMessageEvent metric, TreeModelNode node)
                        => new UnnamedMessageEventViewModel(node.RowData))
                .AddUnderConnection(
                    MetricType.SceneEvent,
                    (SceneEventMetric metric, TreeModelNode node)
                        => new SceneEventViewModel(metric.SceneName.ToString(), metric.SceneEventType.ToString(), node.RowData))
                .AddUnderConnection(
                    MetricType.ServerLog,
                    (ServerLogEvent metric, TreeModelNode node)
                        => new ServerLogEventViewModel(metric.LogLevel, node.RowData))
                .AddUnderConnection(
                    MetricType.NetworkMessage,
                    (NetworkMessageEvent metric, TreeModelNode node)
                        => new NetworkMessageEventViewModel(metric.Name.ToString(), node.RowData),
                    metric => !k_ExcludedNetworkMessageTypeNames.Contains(metric.Name.ToString()))
                .Build();
        }

        public static TreeModel CreateActivityTreeStructure(MetricCollection metrics)
        {
            if (metrics == null)
            {
                return new TreeModel();
            }

            return new TreeModelBuilder(metrics)

                .AddUnderNetworkObject(
                    MetricType.ObjectSpawned,
                    (ObjectSpawnedEvent metric, TreeModelNode node)
                        => new SpawnEventViewModel(metric.NetworkId, node.RowData))

                .AddUnderNetworkObject(
                    MetricType.ObjectDestroyed,
                    (ObjectDestroyedEvent metric, TreeModelNode node)
                        => new DestroyEventViewModel(node.RowData))

                .AddUnderNetworkObject(
                    MetricType.OwnershipChange,
                    (OwnershipChangeEvent metric, TreeModelNode node)
                        => new OwnershipChangeEventViewModel(node.RowData))

                .AddUnderNetworkObject(
                    MetricType.Rpc,
                    (RpcEvent metric, TreeModelNode node)
                        => new RpcEventViewModel(metric.NetworkBehaviourName.ToString(), metric.Name.ToString(), node.RowData))

                .AddUnderNetworkObject(
                    MetricType.NetworkVariableDelta,
                    (NetworkVariableEvent metric, TreeModelNode node)
                        => new NetworkVariableEventViewModel(metric.NetworkBehaviourName.ToString(), metric.Name.ToString(), node.RowData))

                .Build();
        }
    }
}
