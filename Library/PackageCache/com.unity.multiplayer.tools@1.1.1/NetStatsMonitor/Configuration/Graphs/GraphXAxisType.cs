namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Enum used to select the units used to display the graph x-axis labels.
    /// </summary>
    public enum GraphXAxisType
    {
        /// <summary>
        /// Graph x-axis labels will display the duration of the graph in samples.
        /// </summary>
        Samples,
        /// <summary>
        /// Graph x-axis labels will display the duration of the graph in seconds.
        /// </summary>
        Time,
    }
}