using System;
using System.Linq;

using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    /// <summary>
    /// DEPRECATED
    /// This type is now just a wrapper around <see cref="DirectedMetricType"/>.
    /// Please use <see cref="DirectedMetricType"/> instead.
    /// This type can't be deleted yet as it's still referenced by the NGO library (as of 2021-12-07).
    /// </summary>
    struct DirectionalMetricInfo
    {
        public DirectionalMetricInfo(DirectedMetricType directedMetricType)
        {
            DirectedMetricType = directedMetricType;
        }

        public DirectionalMetricInfo(MetricType metricType, NetworkDirection networkDirection)
        {
            DirectedMetricType = metricType.GetDirectedMetric(networkDirection);
        }

        internal DirectedMetricType DirectedMetricType { get; }

        internal MetricType Type => DirectedMetricType.GetMetric();

        internal NetworkDirection Direction => DirectedMetricType.GetDirection();

        internal MetricId Id => DirectedMetricType.GetId();

        internal string DisplayName => DirectedMetricType.GetDisplayName();
    }
}