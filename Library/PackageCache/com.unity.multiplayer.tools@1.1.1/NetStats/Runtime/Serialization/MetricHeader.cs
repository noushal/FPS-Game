using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    struct MetricHeader
    {
        public FixedString128Bytes EventFactoryTypeName { get; set; }

        public MetricContainerType MetricContainerType { get; set; }

        public MetricId MetricId { get; set; }

        public MetricHeader(
            FixedString128Bytes eventFactoryTypeName,
            MetricContainerType metricContainerType,
            MetricId metricId)
        {
            EventFactoryTypeName = eventFactoryTypeName;
            MetricContainerType = metricContainerType;
            MetricId = metricId;
        }
    }
}
