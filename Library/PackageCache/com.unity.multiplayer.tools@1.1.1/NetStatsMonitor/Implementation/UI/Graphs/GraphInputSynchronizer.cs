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
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    /// The GraphInputSynchronize helps to compute how many new points the graph
    /// should draw each frame, based on the number of samples received since the
    /// last visual update, accounting for fractional use of samples
    /// (as the number of samples included in each point may be fractional).
    class GraphInputSynchronizer
    {
        double m_LastReadSampleTimeStamp = Double.NegativeInfinity;
        float m_FractionRemainingOfLastReadSample = 0f;

        /// <summary>
        /// Computes the lastReadSampleIndex and the unreadTimeStampCount, based on a RingBuffer of
        /// timestamps and the last read timestamp.
        /// </summary>
        /// <param name="timeStamps"></param>
        /// <returns> Returns the lastReadSampleIndex and the unreadTimeStampCount </returns>
        (int lastReadSampleIndex, int unreadTimeStampCount)
            ComputeLastReadSampleIndexAndUnreadSampleCount(RingBuffer<double> timeStamps)
        {
            var timeStampCount = timeStamps.Length;
            var lastTimeStampIndex = timeStampCount - 1;
            var lastReadTimeStampIndex = lastTimeStampIndex;
            while (lastReadTimeStampIndex >= 0 && timeStamps[lastReadTimeStampIndex] > m_LastReadSampleTimeStamp)
            {
                --lastReadTimeStampIndex;
            }
            var unreadSampleCount = lastTimeStampIndex - lastReadTimeStampIndex;
            return (lastReadTimeStampIndex, unreadSampleCount);
        }

        /// <summary>
        /// Computes the number of points to advance based on a RingBuffer of timestamps
        /// and the number of samples included in each point. Information about the last
        /// read timestamp is maintained internally.
        /// </summary>
        /// <param name="timeStamps"></param>
        /// <param name="samplesPerPoint"> The number of samples used to compute each point. </param>
        /// <returns> The number of points to advance. </returns>
        public int ComputeNumberOfPointsToAdvance(
            RingBuffer<double> timeStamps,
            float samplesPerPoint)
        {
            var (lastReadSampleIndex, unreadSampleCount) = ComputeLastReadSampleIndexAndUnreadSampleCount(timeStamps);

            // Total unread samples, including the fraction remaining of the last read sample
            var unreadSamplesFrac = unreadSampleCount + m_FractionRemainingOfLastReadSample;

            var pointsToAdvance = (int)(unreadSamplesFrac / samplesPerPoint);
            if (pointsToAdvance <= 0)
            {
                // No other update required
                return 0;
            }

            var samplesRead = pointsToAdvance * samplesPerPoint;

            var nextReadSampleIndex = lastReadSampleIndex + samplesRead - m_FractionRemainingOfLastReadSample;

            m_FractionRemainingOfLastReadSample = (unreadSamplesFrac - samplesRead) % 1f;
            m_LastReadSampleTimeStamp = timeStamps[(int)MathF.Ceiling(nextReadSampleIndex)];

            return pointsToAdvance;
        }
    }
}
#endif