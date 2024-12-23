using NUnit.Framework;

using Unity.Multiplayer.Tools.MetricTypes;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    internal class MetricIdTests
    {
        [TestCase(TestMetric.BytesCounter)]
        [TestCase(TestMetric.BytesGauge)]
        [TestCase(TestMetric.SecondsCounter)]
        [TestCase(TestMetric.SecondsGauge)]
        [TestCase(TestMetric.UnitlessCounter)]
        [TestCase(TestMetric.UnitlessGauge)]
        public void GivenMetricId_WhenToString_ReturnsCorrectValue(TestMetric value)
        {
            var metricId = MetricId.Create(value);

            var stringValue = metricId.ToString();

            Assert.AreEqual(typeof(TestMetric), metricId.EnumType);
            Assert.AreEqual((int)value, metricId.EnumValue);
            Assert.AreEqual(value.ToString(), stringValue);
        }
    }
}
