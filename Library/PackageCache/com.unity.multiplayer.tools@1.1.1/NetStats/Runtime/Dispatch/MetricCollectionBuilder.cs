using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
    class MetricCollectionBuilder
    {
        readonly List<IMetric<long>> m_Counters = new List<IMetric<long>>();
        readonly List<IMetric<double>> m_Gauges = new List<IMetric<double>>();
        readonly List<IMetric<TimeSpan>> m_Timers = new List<IMetric<TimeSpan>>();
        readonly List<IEventMetric> m_PayloadEvents = new List<IEventMetric>();

        public MetricCollectionBuilder WithCounters(params Counter[] counters)
        {
            m_Counters.AddRange(counters);

            return this;
        }

        public MetricCollectionBuilder WithGauges(params Gauge[] gauges)
        {
            m_Gauges.AddRange(gauges);

            return this;
        }

        public MetricCollectionBuilder WithTimers(params Timer[] timers)
        {
            m_Timers.AddRange(timers);

            return this;
        }

        public MetricCollectionBuilder WithMetricEvents<TEvent>(params IEventMetric<TEvent>[] metricEvents)
            where TEvent : struct
        {
            m_PayloadEvents.AddRange(metricEvents);

            return this;
        }

        public MetricCollection Build()
        {
            return new MetricCollection(
                new ReadOnlyDictionary<MetricId, IMetric<long>>(m_Counters.ToDictionary(x => x.Id, x => x)),
                new ReadOnlyDictionary<MetricId, IMetric<double>>(m_Gauges.ToDictionary(x => x.Id, x => x)),
                new ReadOnlyDictionary<MetricId, IMetric<TimeSpan>>(m_Timers.ToDictionary(x => x.Id, x => x)),
                new ReadOnlyDictionary<MetricId, IEventMetric>(m_PayloadEvents.ToDictionary(x => x.Id, x => x)));
        }
    }
}