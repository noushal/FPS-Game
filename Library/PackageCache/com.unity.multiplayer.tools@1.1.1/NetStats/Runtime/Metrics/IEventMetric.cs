using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetStats
{
    interface IEventMetric : IMetric
    {
        int Count { get; }

        int MaxNumberOfValues { get; }

        int NumberOfValuesReceived { get; }
    }

    /// Default implementations on IEventMetric actually can't be invoked
    /// on classes which implement the interface (such as EventMetric).
    /// This is (IMO) a very poor design decision of C#, and one that can
    /// be overcome with an extension method on the interface instead of a
    /// default implementation on the interface.
    static class IEventMetricExtensions
    {
        public static bool WentOverLimit(this IEventMetric metric)
            => metric.NumberOfValuesReceived > metric.MaxNumberOfValues;

        public static int NumberOfValuesIgnored(this IEventMetric metric)
            => metric.NumberOfValuesReceived - metric.Count;

        public static string WentOverLimitMessage(this IEventMetric metric) =>
            $"Multiplayer Tools: Metric {metric.Name} received {metric.NumberOfValuesReceived} values, " +
            $"which exceeds the limit of {metric.MaxNumberOfValues}. " +
            $"{metric.NumberOfValuesIgnored()} values were ignored.";
    }

    interface IEventMetric<TValue> : IEventMetric
    {
        IReadOnlyList<TValue> Values { get; }
    }
}