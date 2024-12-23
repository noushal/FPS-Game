using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    sealed class MetricCollection
    {
        IReadOnlyDictionary<MetricId, IMetric<long>> m_Counters;
        IReadOnlyDictionary<MetricId, IMetric<double>> m_Gauges;
        IReadOnlyDictionary<MetricId, IMetric<TimeSpan>> m_Timers;
        IReadOnlyDictionary<MetricId, IEventMetric> m_PayloadEvents;


        internal MetricCollection(
            IReadOnlyDictionary<MetricId, IMetric<long>> counters,
            IReadOnlyDictionary<MetricId, IMetric<double>> gauges,
            IReadOnlyDictionary<MetricId, IMetric<TimeSpan>> timers,
            IReadOnlyDictionary<MetricId, IEventMetric> payloadEvents)
        {
            m_Counters = counters;
            m_Gauges = gauges;
            m_Timers = timers;
            m_PayloadEvents = payloadEvents;

            Metrics = counters.Values
                .Concat<IMetric>(gauges.Values)
                .Concat(timers.Values)
                .Concat(m_PayloadEvents.Values)
                .ToList();
        }

        internal MetricCollection(
            IReadOnlyCollection<IMetric> metrics,
            ulong localConnectionId)
        {
            static MetricId ByMetricId(IMetric metric) => metric.Id;
            m_Counters = metrics.OfType<IMetric<long>>().ToDictionary(ByMetricId);
            m_Gauges = metrics.OfType<IMetric<double>>().ToDictionary(ByMetricId);
            m_Timers = metrics.OfType<IMetric<TimeSpan>>().ToDictionary(ByMetricId);
            m_PayloadEvents = metrics.OfType<IEventMetric>().ToDictionary(ByMetricId);
            ConnectionId = localConnectionId;
        }

        public IReadOnlyList<IMetric> Metrics { get; }

        public ulong ConnectionId { get; set; } = ulong.MaxValue;

        public bool TryGetCounter(MetricId metricId, out IMetric<long> counter)
        {
            return m_Counters.TryGetValue(metricId, out counter);
        }

        public IMetric<long> GetCounterOrDefault(MetricId metricId)
        {
            if (TryGetCounter(metricId, out IMetric<long> counter))
            {
                return counter;
            }
            else
            {
                return null;
            }
        }

        public bool TryGetGauge(MetricId metricId, out IMetric<double> gauge)
        {
            return m_Gauges.TryGetValue(metricId, out gauge);
        }

        public bool TryGetTimer(MetricId metricId, out IMetric<TimeSpan> timer)
        {
            return m_Timers.TryGetValue(metricId, out timer);
        }

        public bool TryGetEvent<TEvent>(MetricId metricId, out IEventMetric<TEvent> metricEvent)
        {
            var found = m_PayloadEvents.TryGetValue(metricId, out var value);
            if (found && value is IEventMetric<TEvent> typedMetric)
            {
                metricEvent = typedMetric;
                return true;
            }

            metricEvent = null;
            return false;
        }

        public IEventMetric<TEvent> GetPayloadEventOrDefault<TEvent>(MetricId metricId)
        {
            if (TryGetEvent<TEvent>(metricId, out var metricEvent))
            {
                return metricEvent;
            }
            return null;
        }

        public int GetEventCount(MetricId metricId)
        {
            if (m_PayloadEvents.TryGetValue(metricId, out var eventMetric))
            {
                return eventMetric.Count;
            }
            return 0;
        }
    }
}
