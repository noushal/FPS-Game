namespace Unity.Multiplayer.Tools.NetStats.Tests
{
#if !UNITY_MP_TOOLS_DEV
    [MetricTypeEnumHideInInspector]
#endif
    [MetricTypeEnum]
    [MetricTypeSortPriority(SortPriority = SortPriority.VeryLow)]
    internal enum TestMetric
    {
        [MetricMetadata(Units = Units.Bytes, MetricKind = MetricKind.Counter)]
        BytesCounter,

        [MetricMetadata(Units = Units.Bytes, MetricKind = MetricKind.Counter)]
        BytesCounter2,

        [MetricMetadata(Units = Units.Bytes, MetricKind = MetricKind.Counter)]
        BytesCounter3,

        [MetricMetadata(Units = Units.Bytes, MetricKind = MetricKind.Counter)]
        BytesCounter4,

        [MetricMetadata(Units = Units.Bytes, MetricKind = MetricKind.Gauge)]
        BytesGauge,

        [MetricMetadata(Units = Units.Seconds, MetricKind = MetricKind.Counter)]
        SecondsCounter,
        [MetricMetadata(Units = Units.Seconds, MetricKind = MetricKind.Gauge)]
        SecondsGauge,

        [MetricMetadata(Units = Units.None, MetricKind = MetricKind.Counter)]
        UnitlessCounter,
        [MetricMetadata(Units = Units.None, MetricKind = MetricKind.Gauge)]
        UnitlessGauge,
    }
}