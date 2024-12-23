namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// This enum can be used to indicate the units that a metric is reported in,
    /// so that they can be displayed in the Runtime Network Stats Monitor with the appropriate units.
    /// </summary>
    public enum Units
    {
        /// <summary>
        /// A dimensionless metric without units (e.g. connection count)
        /// </summary>
        None,

        /// <summary>
        /// Represent bytes.
        /// </summary>
        Bytes,
        /// <summary>
        /// Represent bytes per second.
        /// </summary>
        BytesPerSecond,
        /// <summary>
        /// Represent a second.
        /// </summary>
        Seconds,
        /// <summary>
        /// Represent a hertz (1/second).
        /// </summary>
        Hertz,
    }
}