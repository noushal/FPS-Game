using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Graph configuration used by <see cref="DisplayElementConfiguration"/>.
    /// This configuration contain all information about a Graph.
    /// </summary>
    [Serializable]
    public sealed class GraphConfiguration
    {
        /// <summary>
        /// The number of samples that are maintained for the purpose of graphing.
        /// </summary>
        /// <remarks>
        /// The value is clamped to the range [8, 4096].
        /// </remarks>
        [field: SerializeField]
        [field: Tooltip("The number of samples that are maintained for the purpose of graphing. " +
                        "The value is clamped to the range [8, 4096].")]
        [field: Range(ConfigurationLimits.k_GraphSampleMin, ConfigurationLimits.k_GraphSampleMax)]
        int m_SampleCount = 256;

        /// <summary>
        /// The number of samples that are maintained for the purpose of graphing.
        /// </summary>
        /// <remarks>
        /// The value is clamped to the range [8, 4096].
        /// </remarks>
        public int SampleCount
        {
            get => m_SampleCount;
            set => m_SampleCount = Mathf.Clamp(
                value,
                ConfigurationLimits.k_GraphSampleMin,
                ConfigurationLimits.k_GraphSampleMax);
        }

        /// <summary>
        /// The sample rate of the graph.
        /// </summary>
        /// <remarks>
        /// If the sample rate is <see cref="F:Unity.Multiplayer.Tools.NetStatsMonitor.SampleRate.PerSecond"/>
        /// then each point in the graph corresponds to data collected over a full second, whereas
        /// if the sample rate is <see cref="F:Unity.Multiplayer.Tools.NetStatsMonitor.SampleRate.PerFrame"/>
        /// then each point in the graph corresponds to data collected within a single frame.
        /// </remarks>
        [field: SerializeField]
        [Tooltip(
            "The sample rate of the graph. " +
            "If the sample rate is Per Second " +
            "then each point in the graph corresponds to data collected over a full second, whereas " +
            "if the sample rate is Per Frame " +
            "then each point in the graph corresponds to data collected within a single frame.")]
        public SampleRate SampleRate { get; set; } = SampleRate.PerSecond;

        /// <summary>
        /// List of colors to override the default colors of the graph.
        /// </summary>
        [field: SerializeField]
        public List<Color> VariableColors { get; set; } = new();

        /// <summary>
        /// The units used for displaying the bounds of the graph's x-axis.
        /// By default the graph bounds are displayed in units of sample count.
        /// If set to time, the the x-axis graph bounds will display
        /// the time over which these samples were collected.
        /// </summary>
        [field: SerializeField]
        public GraphXAxisType XAxisType { get; set; } = GraphXAxisType.Samples;

        /// <summary>
        /// Line-graph specific options.
        /// </summary>
        [field: SerializeField]
        public LineGraphConfiguration LineGraphConfiguration { get; set; } = new();

        internal int ComputeHashCode()
        {
            var hash = HashCode.Combine(SampleCount, (int)SampleRate, (int)XAxisType, LineGraphConfiguration.ComputeHashCode());
            if (VariableColors != null)
            {
                foreach (var color in VariableColors)
                {
                    hash = HashCode.Combine(hash, color);
                }
            }
            return hash;
        }
    }
}