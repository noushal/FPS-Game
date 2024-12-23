using System;
using System.Threading;
using NUnit.Framework;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    sealed class TimerTests
    {
        [Test]
        public void Set_Always_SetsUnderlyingValueToSpecifiedTimeSpan()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.ServerLogReceived);
            var timer = new Timer(id);
            var duration = TimeSpan.FromDays(1);

            // Act
            timer.Set(duration);

            // Assert
            Assert.AreEqual(id, timer.Id);
            Assert.AreEqual(duration, timer.Value);
        }

        [Test]
        public void Time_Always_SetsUnderlyingValueToScopeExecutionDuration()
        {
            // Arrange
            var id = MetricId.Create(DirectedMetricType.ServerLogSent);
            var timer = new Timer(id);

            // Act
            using (timer.Time())
            {
                Thread.Sleep(110);
            }

            // Assert
            Assert.AreEqual(id, timer.Id);
            Assert.Greater(timer.Value, TimeSpan.FromMilliseconds(100));
        }
    }
}