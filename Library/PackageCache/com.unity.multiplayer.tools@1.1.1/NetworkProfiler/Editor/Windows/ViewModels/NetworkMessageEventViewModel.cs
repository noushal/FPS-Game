using System;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class NetworkMessageEventViewModel : ViewModelBase
    {
        public NetworkMessageEventViewModel(string messageName, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                messageName,
                MetricTypeExtensions.GetDisplayNameString(messageName), //Using messageName to clarify the type instead of just NetworkMessage
                MetricTypeExtensions.GetTypeNameString(messageName),
                onSelectedCallback)
        {
        }
    }
}