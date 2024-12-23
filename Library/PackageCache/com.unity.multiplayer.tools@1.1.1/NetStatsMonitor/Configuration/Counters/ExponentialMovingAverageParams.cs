using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Parameters for the exponential moving average smoothing method in <see cref="CounterConfiguration"/>.
    /// </summary>
    [Serializable]
    public sealed class ExponentialMovingAverageParams
    {
        /// <summary>
        /// The half-life (in seconds) by which samples should decay.
        /// By default, this is set to one second.
        /// </summary>
        [field: SerializeField]
        [field: Min((float)ConfigurationLimits.k_ExponentialMovingAverageHalfLifeMin)]
        double m_HalfLife = 1;

        /// <summary>
        /// The half-life (in seconds) by which samples should decay.
        /// By default, this is set to one second.
        /// </summary>
        public double HalfLife
        {
            get => m_HalfLife;
            set => m_HalfLife = Math.Max(value, ConfigurationLimits.k_ExponentialMovingAverageHalfLifeMin);
        }

        internal int ComputeHashCode()
        {
            return HalfLife.GetHashCode();
        }
    }
}