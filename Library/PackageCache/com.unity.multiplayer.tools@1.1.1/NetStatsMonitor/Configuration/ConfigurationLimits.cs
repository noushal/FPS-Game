namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    static class ConfigurationLimits
    {
        internal const int k_GraphSampleMin = 8;
        internal const int k_GraphSampleMax = 4096;

        internal const int k_CounterSampleMin = 8;
        internal const int k_CounterSampleMax = 4096;

        // Limited to 7 for now
        // Informed by the approximate base 10 precision of 32-bit floating point
        // https://en.wikipedia.org/wiki/Single-precision_floating-point_format
        internal const int k_CounterSignificantDigitsMin = 1;
        internal const int k_CounterSignificantDigitsMax = 7;

        internal const double k_ExponentialMovingAverageHalfLifeMin = 0;

        internal const double k_RefreshRateMin = 1;

        internal const float k_PositionMin = 0;
        internal const float k_PositionMax = 1;

        internal const float k_GraphLineThicknessMin = 1;
        internal const float k_GraphLineThicknessMax = 5;
    }
}
