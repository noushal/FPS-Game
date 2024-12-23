// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// Utils for lists of MetricIds
    internal static class MetricsUtils
    {
        public static BaseUnits GetUnits(List<MetricId> metrics, string displayElementLabel)
        {
            if (metrics.Count <= 0)
            {
                return new BaseUnits();
            }

            var firstMetricUnits = metrics[0].Units;
            for (var i = 1; i < metrics.Count; ++i)
            {
                if (!metrics[i].Units.Equals(firstMetricUnits))
                {
                    // Only do this slow diagnostic step when we've found a mismatch
                    // and know there's a problem
                    return HandleInconvertibleUnits(metrics, displayElementLabel);
                }
            }
            return firstMetricUnits;
        }

        /// Handle unequal units by choosing the most popular one
        static BaseUnits ChooseMostPopularUnits(List<MetricId> metrics)
        {
            Dictionary<BaseUnits, int> unitCounts = new();
            foreach (var metric in metrics)
            {
                var unit = metric.Units;
                if (unitCounts.ContainsKey(unit))
                {
                    unitCounts[unit]++;
                }
                else
                {
                    unitCounts.Add(unit, 1);
                }
            }
            var maxCount = 0;
            var maxCountUnit = new BaseUnits();
            foreach (var (unit, count) in unitCounts)
            {
                if (count > maxCount)
                {
                    maxCount = count;
                    maxCountUnit = unit;
                }
            }
            return maxCountUnit;
        }

        /// Handle inconvertible units by raising an error with information about the
        /// inconvertible units and then picking the most popular one
        static BaseUnits HandleInconvertibleUnits(
            List<MetricId> metrics,
            string displayElementLabel)
        {
            HashSet<BaseUnits> inconvertibleUnits = new();
            foreach (var metric in metrics)
            {
                inconvertibleUnits.Add(metric.Units);
            }
            var inconvertibleUnitsString = String.Join(", ", inconvertibleUnits);
            Debug.LogWarning($"Display Element {displayElementLabel} is configured with " +
                             $"inconvertible units:\n {{{inconvertibleUnitsString}}}");
            return ChooseMostPopularUnits(metrics);
        }

        public static bool ShouldDisplayAsPercentage(List<MetricId> metrics, string displayElementLabel)
        {
            if (metrics.Count <= 0)
            {
                return false;
            }

            var firstMetricShouldBeDisplayedAsPercentage = metrics[0].DisplayAsPercentage;

            // var allDisplayAsPercentage = true;
            for (var i = 1; i < metrics.Count; ++i)
            {
                if (!metrics[i].DisplayAsPercentage.Equals(firstMetricShouldBeDisplayedAsPercentage))
                {
                    // Only do this slow diagnostic step when we've found a mismatch
                    // and know there's a problem
                    return HandleMismatchedDisplayAsPercentages(metrics, displayElementLabel);
                }
            }
            return firstMetricShouldBeDisplayedAsPercentage;
        }

        static bool HandleMismatchedDisplayAsPercentages(
            List<MetricId> metrics,
            string displayElementLabel)
        {
            Debug.LogWarning($"Display Element {displayElementLabel} is configured with " +
                             "some stats that should be displayed as percentages and others not. " +
                             "This display element will display its values without percentages.");
            return false;
        }
    }
}
#endif
