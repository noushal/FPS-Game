using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Counter configuration used by <see cref="DisplayElementConfiguration"/>.
    /// This configuration contain all information about a counter.
    /// </summary>
    [Serializable]
    public sealed class CounterConfiguration
    {
        /// <summary>
        /// The desired smoothing method over time for the counter.
        /// </summary>
        [field: SerializeField]
        public SmoothingMethod SmoothingMethod { get; set; } = SmoothingMethod.ExponentialMovingAverage;

        /// <summary>
        /// The desired aggregation method for the stats used in the counter.
        /// </summary>
        [field: SerializeField]
        public AggregationMethod AggregationMethod { get; set; } = AggregationMethod.Sum;

        /// <summary>
        /// The number of significant digits to display for this counter.
        /// </summary>
        [field: SerializeField]
        [field: Range(ConfigurationLimits.k_CounterSignificantDigitsMin, ConfigurationLimits.k_CounterSignificantDigitsMax)]
        [field: Tooltip("The number of significant digits to display for this counter.")]
        int m_SignificantDigits = 3;

        /// <summary>
        /// The number of significant digits to display for this counter.
        /// </summary>
        public int SignificantDigits
        {
            get => m_SignificantDigits;
            set => m_SignificantDigits = Mathf.Clamp(
                value,
                ConfigurationLimits.k_CounterSignificantDigitsMin,
                ConfigurationLimits.k_CounterSignificantDigitsMax);
        }

        /// <summary>
        /// Values below this threshold will be highlighted by the default styling,
        /// and can be highlighted by custom styling using the following USS classes:
        /// - rnsm-counter-out-of-bounds
        /// - rnsm-counter-below-threshold
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Values below this threshold will be highlighted by the default styling, " +
                        "and can be highlighted by custom styling using the following USS classes: " +
                        "\"" + UssClassNames.k_CounterOutOfBounds + "\", or " +
                        "\"" + UssClassNames.k_CounterBelowThreshold + "\"")]
        public float HighlightLowerBound { get; set; } = float.NegativeInfinity;

        /// <summary>
        /// Values above this threshold will be highlighted by the default styling,
        /// and can be highlighted by custom styling using the following USS classes:
        /// - rnsm-counter-out-of-bounds
        /// - rnsm-counter-above-threshold
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Values above this threshold will be highlighted by the default styling, " +
                        "and can be highlighted by custom styling using the following USS classes: " +
                        "\"" + UssClassNames.k_CounterOutOfBounds + "\", or " +
                        "\"" + UssClassNames.k_CounterAboveThreshold + "\"")]
        public float HighlightUpperBound { get; set; } = float.PositiveInfinity;

        /// <summary>
        /// Parameters used if <see cref="SmoothingMethod"/> is set to ExponentialMovingAverage.
        /// </summary>
        [field: SerializeField]
        public ExponentialMovingAverageParams ExponentialMovingAverageParams { get; set; } = new();

        /// <summary>
        /// Parameters used if <see cref="SmoothingMethod"/> is set to SimpleMovingAverage.
        /// </summary>
        [field: SerializeField]
        public SimpleMovingAverageParams SimpleMovingAverageParams { get; set; } = new();

        /// <summary>
        /// The current configured sample count.
        /// Note that if the <see cref="SmoothingMethod"/> is set to ExponentialMovingAverage,
        /// the sample count will be zero.
        /// </summary>
        public int SampleCount => SmoothingMethod == SmoothingMethod.SimpleMovingAverage
            ? SimpleMovingAverageParams.SampleCount
            : 0;

        /// <summary>
        /// The current configured sample rate.
        /// </summary>
        /// <remarks>
        /// Note that if the <see cref="SmoothingMethod"/> is set to ExponentialMovingAverage
        /// the <see cref="SampleRate"/> will be PerFrame.
        /// </remarks>
        internal SampleRate SampleRate => SmoothingMethod == SmoothingMethod.SimpleMovingAverage
            ? SimpleMovingAverageParams.SampleRate
            : SampleRate.PerFrame;

        internal int ComputeHashCode()
        {
            return HashCode.Combine(
                (int)SmoothingMethod,
                (int)AggregationMethod,
                SignificantDigits,
                HighlightLowerBound,
                HighlightUpperBound,
                ExponentialMovingAverageParams.ComputeHashCode(),
                SimpleMovingAverageParams.ComputeHashCode());
        }
    }
}