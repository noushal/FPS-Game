using System;
using System.Diagnostics;

namespace Unity.Multiplayer.Tools.NetStats
{
    [Serializable]
    class Timer : Metric<TimeSpan>
    {
        public Timer(MetricId metricId, TimeSpan defaultValue = default)
            : base(metricId, defaultValue)
        {
        }

        public void Set(TimeSpan value)
        {
            Value = value;
        }

        public TimerScope Time()
        {
            return new TimerScope(Set);
        }

        public readonly struct TimerScope : IDisposable
        {
            readonly Action<TimeSpan> m_Callback;
            readonly Stopwatch m_Stopwatch;

            internal TimerScope(Action<TimeSpan> callback)
            {
                m_Callback = callback;

                m_Stopwatch = new Stopwatch();
                m_Stopwatch.Start();
            }

            public void Dispose()
            {
                m_Callback?.Invoke(m_Stopwatch.Elapsed);
            }
        }

        public override MetricContainerType MetricContainerType => MetricContainerType.Timer;
    }
}
