using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    [Flags]
    enum NetworkDirection
    {
        None            = 0,
        Received        = 1 << 0,
        Sent            = 1 << 1,
        SentAndReceived = Received | Sent,
    }

    static class NetworkDirectionConstants
    {
        // Mask and BitCount are used in the definition of DirectedMetricType.
        // Please update these values when making changes to NetworkDirection
        // that will effect the bit width required to store its largest value.
        internal const int k_Mask = (int)NetworkDirection.SentAndReceived;
        internal const int k_BitWidth = 2;
    }

    static class NetworkDirectionExtensions
    {
        public static string DisplayString(this NetworkDirection direction)
        {
            return direction == NetworkDirection.None
                ? ""
                : direction.ToString();
        }
    }
}