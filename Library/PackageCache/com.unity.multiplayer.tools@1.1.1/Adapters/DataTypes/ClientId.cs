namespace Unity.Multiplayer.Tools.Adapters
{
    /// <summary>
    /// Strongly-typed enum wrapper for a numeric ClientId, used by the adapter interfaces
    /// </summary>
    /// <remarks>
    /// The use of a wrapper enum was chosen over a wrapper struct for performance reasons.
    /// The use of a wrapper struct can incur method calls for equality and hashing,
    /// which can make wrapper structs slower than an enum or unwrapped numeric type when
    /// used as dictionary keys.
    /// </remarks>
    enum ClientId : long
    {
        // No members on purpose, this is a wrapper struct for strong typing
    }
}
