namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// The sample rate of a graph or simple-moving-average counter
    /// </summary>
    /// <remarks>
    /// In display elements with a <see cref="PerSecond"/> sample rate, each point or sample
    /// corresponds to data collected over a full second, whereas
    /// in display elements with a <see cref="PerFrame"/> sample rate, each point or sample
    /// corresponds to data collected in a single frame.
    /// </remarks>
    public enum SampleRate
    {
        /// <summary>
        /// Graph points and Simple Moving Average counter samples correspond to data collected within a single frame.
        /// </summary>
        PerFrame,
        /// <summary>
        /// Graph points and Simple Moving Average counter samples correspond to data collected over a full second.
        /// </summary>
        PerSecond,
    }

    /// <summary>
    /// I'm including these in a separate class rather than as members of the enum,
    /// so they don't need to be supported indefinitely as part of our public API,
    /// or show up as options in the inspector UI or in enum dropdowns
    /// </summary>
    static class SampleRates
    {
        public const SampleRate k_First = SampleRate.PerFrame;
        public const SampleRate k_Last = SampleRate.PerSecond;
    }

    static class SampleRateExtensions
    {
        public static SampleRate Next(this SampleRate rate)
        {
            return (SampleRate)((int)rate + 1);
        }
    }
}
