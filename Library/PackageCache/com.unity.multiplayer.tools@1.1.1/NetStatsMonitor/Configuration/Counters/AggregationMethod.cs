namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Enum to select the different aggregation method offered by <see cref="CounterConfiguration"/>.
    /// </summary>
    public enum AggregationMethod
    {
        /// <summary>
        /// Aggregation using the sum of multiple stats.
        /// </summary>
        Sum,
        /// <summary>
        /// Aggregation using the average of multiple stats.
        /// </summary>
        Average,

        // TODO: MTT-1722
        // Reintroduce these non-linear aggregation methods with some clever caching of
        // combined, non-linear, smoothed stats over time in the history
        // Min,
        // Max,
        // Median,
    }
}