using System;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class NamedMessageEventViewModel : ViewModelBase
    {
        public NamedMessageEventViewModel(string messageName, IRowData parent, Action onSelectedCallback = null)
            : base(
                parent,
                messageName,
                MetricType.NamedMessage,
                onSelectedCallback)
        {
        }
    }
}