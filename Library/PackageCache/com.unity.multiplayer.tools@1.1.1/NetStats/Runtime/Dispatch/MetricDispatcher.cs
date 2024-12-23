using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
    class MetricDispatcher : IMetricDispatcher
    {
        readonly MetricCollection m_Collection;
        readonly IReadOnlyList<IResettable> m_Resettables;
        readonly IReadOnlyList<IEventMetric> m_EventMetrics;

        readonly IList<IMetricObserver> m_Observers = new List<IMetricObserver>();

        [CanBeNull]
        StringBuilder m_OverLimitMessageStringBuilder;

        internal MetricDispatcher(
            MetricCollection collection,
            IReadOnlyList<IResettable> resettables,
            IReadOnlyList<IEventMetric> eventMetrics)
        {
            m_Collection = collection;
            m_Resettables = resettables;
            m_EventMetrics = eventMetrics;
        }

        public void RegisterObserver(IMetricObserver observer)
        {
            m_Observers.Add(observer);
        }

        public void SetConnectionId(ulong connectionId)
        {
            m_Collection.ConnectionId = connectionId;
        }

        public void Dispatch()
        {
            for (var i = 0; i < m_EventMetrics.Count; i++)
            {
                var metric = m_EventMetrics[i];
                if (metric.WentOverLimit())
                {
                    m_OverLimitMessageStringBuilder ??= new StringBuilder();
                    m_OverLimitMessageStringBuilder.AppendLine(metric.WentOverLimitMessage());
                }
            }
            if (m_OverLimitMessageStringBuilder?.Length > 0)
            {
                Debug.LogWarning(m_OverLimitMessageStringBuilder);
                m_OverLimitMessageStringBuilder.Clear();
            }

            for (var i = 0; i < m_Observers.Count; i++)
            {
                var snapshotObserver = m_Observers[i];
                snapshotObserver.Observe(m_Collection);
            }

            for (var i = 0; i < m_Resettables.Count; i++)
            {
                var resettable = m_Resettables[i];
                if (resettable.ShouldResetOnDispatch)
                {
                    resettable.Reset();
                }
            }
        }
    }
}