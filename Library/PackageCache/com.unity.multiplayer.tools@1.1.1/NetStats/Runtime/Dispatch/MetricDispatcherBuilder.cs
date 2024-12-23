using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
    sealed class MetricDispatcherBuilder
    {
        readonly IDictionary<MetricId, IMetric<long>> m_Counters
            = new Dictionary<MetricId, IMetric<long>>();
        readonly IDictionary<MetricId, IMetric<double>> m_Gauges
            = new Dictionary<MetricId, IMetric<double>>();
        readonly IDictionary<MetricId, IMetric<TimeSpan>> m_Timers
            = new Dictionary<MetricId, IMetric<TimeSpan>>();
        readonly IDictionary<MetricId, IEventMetric> m_PayloadEvents
            = new Dictionary<MetricId, IEventMetric>();

        readonly List<IResettable> m_Resettables = new List<IResettable>();

        public MetricDispatcherBuilder WithCounters(params Counter[] counters)
        {
            foreach (var counter in counters)
            {
                m_Counters[counter.Id] = counter;
                m_Resettables.Add(counter);
            }

            return this;
        }

        public MetricDispatcherBuilder WithGauges(params Gauge[] gauges)
        {
            foreach (var gauge in gauges)
            {
                m_Gauges[gauge.Id] = gauge;
                m_Resettables.Add(gauge);
            }

            return this;
        }

        public MetricDispatcherBuilder WithTimers(params Timer[] timers)
        {
            foreach (var timer in timers)
            {
                m_Timers[timer.Id] = timer;
                m_Resettables.Add(timer);
            }

            return this;
        }

        public MetricDispatcherBuilder WithMetricEvents<TEvent>(params EventMetric<TEvent>[] metricEvents)
            where TEvent : unmanaged
        {
            foreach (var metricEvent in metricEvents)
            {
                m_PayloadEvents[metricEvent.Id] = metricEvent;
                m_Resettables.Add(metricEvent);
            }

            return this;
        }

        public IMetricDispatcher Build()
        {
            return new MetricDispatcher(
                new MetricCollection(
                    new ReadOnlyDictionary<MetricId, IMetric<long>>(m_Counters),
                    new ReadOnlyDictionary<MetricId, IMetric<double>>(m_Gauges),
                    new ReadOnlyDictionary<MetricId, IMetric<TimeSpan>>(m_Timers),
                    new ReadOnlyDictionary<MetricId, IEventMetric>(m_PayloadEvents)),
                m_Resettables,
                m_PayloadEvents.Values.ToList());
        }
    }
}