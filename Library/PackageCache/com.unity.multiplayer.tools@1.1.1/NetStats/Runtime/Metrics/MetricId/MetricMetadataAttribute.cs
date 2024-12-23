using System;
using JetBrains.Annotations;

namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// Attribute to provide more information about a metric,
    /// such as a custom name and units.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MetricMetadataAttribute : Attribute
    {
        /// <summary>
        /// The custom display name to show for a metric.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The kind of metric.
        /// By default, the metric is a counter.
        /// </summary>
        public MetricKind MetricKind { get; set; } = MetricKind.Counter;

        /// <summary>
        /// The units for the metric.
        /// By default, there are no units.
        /// </summary>
        public Units Units { get; set; } = Units.None;

        /// <summary>
        /// Toggle for the metric to be shown as a percentage.
        /// This should only be used for unitless metrics.
        /// </summary>
        public bool DisplayAsPercentage { get; set; }
    }
}