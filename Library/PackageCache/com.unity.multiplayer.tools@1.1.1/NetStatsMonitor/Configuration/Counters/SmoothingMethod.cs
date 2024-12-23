using System;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Enum to select the different smoothing method offered by <see cref="CounterConfiguration"/>.
    /// </summary>
    public enum SmoothingMethod
    {
        /// <summary>
        /// Smoothing method using an exponential moving average.
        /// </summary>
        /// <remarks>
        /// This method takes less memory than <see cref="SimpleMovingAverage"/>
        /// since it doesn't require to keep samples to obtain a value.
        /// </remarks>
        ExponentialMovingAverage,
        /// <summary>
        /// Smoothing method using a simple moving average.
        /// </summary>
        SimpleMovingAverage,
    }
}