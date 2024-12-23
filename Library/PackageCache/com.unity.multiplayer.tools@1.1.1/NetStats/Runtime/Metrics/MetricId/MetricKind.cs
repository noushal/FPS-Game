namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// Represent the kind a metric can be.
    /// </summary>
    public enum MetricKind
    {
        /// <summary>
        /// Represent a counter metric.
        /// A counter is a cumulative metric whose value can only be increased or reset to zero.
        /// For a metric that could be decreased, a <see cref="Gauge"/> should be used.
        /// </summary>
        Counter,
        /// <summary>
        /// Represent a gauge metric.
        /// A gauge is a metric that represents a single numerical value that can go up or down.
        /// For a metric that can only go up, a <see cref="Counter"/> should be used.
        /// </summary>
        Gauge,
    }
}