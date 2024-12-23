using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Parameters for the simple moving average smoothing method in <see cref="CounterConfiguration"/>.
    /// </summary>
    [Serializable]
    public sealed class SimpleMovingAverageParams
    {
        /// <summary>
        /// The number of samples that are maintained for the purpose of smoothing.
        /// </summary>
        /// <remarks>
        /// The value is clamped to the range [8, 4096].
        /// </remarks>
        [field: SerializeField]
        [field: Min(1)]
        [field: Tooltip("The number of samples that are maintained for the purpose of smoothing." +
                        "The value is clamped to the range [8, 4096].")]
        [field: Range(ConfigurationLimits.k_CounterSampleMin, ConfigurationLimits.k_CounterSampleMax)]
        int m_SampleCount = 64;

        /// <summary>
        /// The number of samples that are maintained for the purpose of smoothing.
        /// </summary>
        /// <remarks>
        /// The value is clamped to the range [8, 4096].
        /// </remarks>
        public int SampleCount
        {
            get => m_SampleCount;
            set => m_SampleCount = Mathf.Clamp(
                value,
                ConfigurationLimits.k_CounterSampleMin,
                ConfigurationLimits.k_CounterSampleMax);
        }

        /// <summary>
        /// The sample rate of the Simple Moving Average counter.
        /// </summary>
        /// <remarks>
        /// If the sample rate is <see cref="F:Unity.Multiplayer.Tools.NetStatsMonitor.SampleRate.PerSecond"/>
        /// then each sample in the counter is collected over a full second, whereas
        /// if the sample rate is <see cref="F:Unity.Multiplayer.Tools.NetStatsMonitor.SampleRate.PerFrame"/>
        /// then each sample in the counter is collected during a single frame.
        /// </remarks>
        [field: SerializeField]
        [Tooltip(
            "The sample rate of the counter. " +
            "If the sample rate is Per Second " +
            "then each sample in the counter is collected over a full second, whereas " +
            "if the sample rate is Per Frame " +
            "then each sample in the counter is collected during a single frame.")]
        public SampleRate SampleRate { get; set; } = SampleRate.PerFrame;

        internal int ComputeHashCode()
        {
            return HashCode.Combine(SampleCount, SampleRate);
        }
    }
}