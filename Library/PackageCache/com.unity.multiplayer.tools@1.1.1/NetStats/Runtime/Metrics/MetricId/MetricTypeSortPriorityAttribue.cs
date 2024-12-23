using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    internal enum SortPriority : int
    {
        VeryHigh = -2,
        High = -1,
        Neutral = 0,
        Low = 1,
        VeryLow = 2,
    }

    /// Attribute internal fields cannot be referenced when applying attributes.
    /// As an alternative, this second, internal attribute can be used to hide metrics
    /// from the inspector that are only intended for internal MP Tools testing.
    [AttributeUsage(AttributeTargets.Enum)]
    internal class MetricTypeSortPriorityAttribute : Attribute
    {
        public SortPriority SortPriority { get; set; }
    }
}