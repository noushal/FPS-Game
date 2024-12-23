using static Unity.Multiplayer.Tools.NetStats.Tests.TestMetricIdConstants;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    static class TestMetricIdConstants
    {
        public const int Test1Value = 1;
        public const string Test1Name = nameof(TestMetricId.Test1);
        public const string Test1DisplayName = "3E6C45D1-3C1D-4AEE-A32A-1C80F23FA6E9";
        public const Units Test1Units = Units.Hertz;
        public const MetricKind Test1MetricKind = MetricKind.Gauge;
    }

    [MetricTypeEnum]
    [MetricTypeEnumHideInInspector]
    [MetricTypeSortPriority(SortPriority = SortPriority.VeryLow)]
    enum TestMetricId
    {
        [MetricMetadata(
            DisplayName = Test1DisplayName,
            Units = Test1Units,
            MetricKind = Test1MetricKind)]
        Test1 = Test1Value,
        Test2
    }
}