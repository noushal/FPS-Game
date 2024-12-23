using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    internal static class DirectedMetricTypeExtensions
    {
        static readonly Dictionary<DirectedMetricType, string> s_Identifiers =
            new Dictionary<DirectedMetricType, string>();
        static readonly Dictionary<DirectedMetricType, string> s_DisplayNames =
            new Dictionary<DirectedMetricType, string>();

        static DirectedMetricTypeExtensions()
        {
            var metricTypes = EnumUtil.GetValues<MetricType>();
            var networkDirections = EnumUtil.GetValues<NetworkDirection>();
            foreach (var metricType in metricTypes)
            {
                foreach (var networkDirection in networkDirections)
                {
                    var directedMetricType = metricType.GetDirectedMetric(networkDirection);
                    var combinedName = metricType.ToString() + networkDirection.ToString();
                    s_Identifiers[directedMetricType] = combinedName;
                    s_DisplayNames[directedMetricType] = StringUtil.AddSpacesToCamelCase(combinedName);
                }
            }
        }

        /// Create a DirectedMetricType from a MetricType and a Direction
        internal static DirectedMetricType GetDirectedMetric(
            this MetricType metricType,
            NetworkDirection direction)
        {
            return (DirectedMetricType)(
                ((int)metricType << NetworkDirectionConstants.k_BitWidth) |
                ((int)direction  &  NetworkDirectionConstants.k_Mask));
        }

        /// Get the MetricType from a DirectedMetricType
        internal static MetricType GetMetric(this DirectedMetricType directedMetric)
        {
            return (MetricType)((int)directedMetric >> NetworkDirectionConstants.k_BitWidth);
        }

        /// Get the Direction from a DirectedMetricType
        internal static NetworkDirection GetDirection(this DirectedMetricType directedMetric)
        {
            return (NetworkDirection)((int)directedMetric & NetworkDirectionConstants.k_Mask);
        }

        internal static MetricId GetId(this DirectedMetricType directedMetric)
        {
            return MetricId.Create(directedMetric);
        }

        internal static string GetDisplayName(this DirectedMetricType directedMetric)
        {
            if (s_DisplayNames.TryGetValue(directedMetric, out var id))
            {
                return id;
            }
            return directedMetric.ToString();
        }
    }
}