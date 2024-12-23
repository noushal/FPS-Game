using System;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class RpcEventViewModel : ViewModelBase
    {
        public RpcEventViewModel(string componentName, string rpcName, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                $"{componentName}.{rpcName}",
                MetricType.Rpc,
                onSelectedCallback)
        {
        }
    }
}