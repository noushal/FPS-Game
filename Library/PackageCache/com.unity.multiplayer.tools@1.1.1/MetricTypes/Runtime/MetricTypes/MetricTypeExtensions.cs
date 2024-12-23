using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetStats
{
    static class MetricTypeExtensions
    {
        internal static string GetDisplayNameString(string metricType)
        {
            return StringUtil.AddSpacesToCamelCase(metricType);
        }

        internal static string GetDisplayNameString(this MetricType metricType)
        {
            return GetDisplayNameString(metricType.ToString());
        }

        internal static string GetTypeNameString(string metricType)
        {
            return metricType.ToLowerInvariant();
        }

        internal static string GetTypeNameString(this MetricType metricType)
        {
            return GetTypeNameString(metricType.ToString());
        }
    }
}