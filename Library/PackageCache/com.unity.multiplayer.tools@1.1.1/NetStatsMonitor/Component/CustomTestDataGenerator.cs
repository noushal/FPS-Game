using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.Common;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    [Serializable]
    internal class MetricTrend
    {
        [field: SerializeField]
        public MetricId Metric { get; set; }

        [field: SerializeField]
        public LogNormalRandomWalk Trend { get; set; }
    }

    /// This component can be used to generate custom data for local manual testing.
    /// It is analogous to TestDataGenerator except that it generates data using the
    /// API for custom, user-defined stats, and it is configurable in the inspector.
#if UNITY_MP_TOOLS_DEV
    [AddComponentMenu("Multiplayer Tools/" + "Custom Test Data Generator", 1000)]
#else
    [AddComponentMenu("")] // Prevent the component from being instantiated in editor
#endif
    internal class CustomTestDataGenerator : MonoBehaviour
    {
        [field: Tooltip("Pairs of metrics and trends to generate test data for")]
        [field: SerializeField]
        internal List<MetricTrend> MetricTrends { get; set; } = new List<MetricTrend>
        {
            new MetricTrend
            {
                // This bit of boilerplate ensures that the trend receives its default
                // fields in the inspector, rather than being all zeroes
                Trend = new LogNormalRandomWalk { }
            }
        };

        RuntimeNetStatsMonitor m_Rnsm;

        System.Random m_Random = new();

        void Start()
        {
            m_Rnsm = FindObjectOfType<RuntimeNetStatsMonitor>();
        }

        void Update()
        {
            if (!m_Rnsm)
            {
                return;
            }
            foreach (var metricTrend in MetricTrends)
            {
                var sample = metricTrend.Trend.NextFloat(m_Random);
                m_Rnsm.AddCustomValue(metricTrend.Metric, sample);
            }
        }
    }
}
