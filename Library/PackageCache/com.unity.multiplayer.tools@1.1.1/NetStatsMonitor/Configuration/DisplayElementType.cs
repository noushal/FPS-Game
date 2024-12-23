namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// Enum representing the different type of elements the <see cref="RuntimeNetStatsMonitor"/> can render.
    /// </summary>
    public enum DisplayElementType
    {
        /// <summary>
        /// Counter type for <see cref="DisplayElementConfiguration"/>
        /// </summary>
        Counter,
        /// <summary>
        /// Line graph type for <see cref="DisplayElementConfiguration"/>
        /// </summary>
        LineGraph,
        /// <summary>
        /// Stacked area graph type for <see cref="DisplayElementConfiguration"/>
        /// </summary>
        StackedAreaGraph,
    }
}