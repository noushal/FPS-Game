using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStats.Tests;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation.UI
{
    internal class CounterVisualElementTests
    {
        static readonly MetricId k_UnitlessGauge
            = MetricId.Create(TestMetric.UnitlessGauge);

        [TestCase("Total Bytes Sent")]
        [TestCase("totalbytesreceived")]
        [TestCase("TOTAL_MESSAGES_SENT")]
        [TestCase("total-messages-received")]
        [TestCase("148.2")]
        [TestCase("-9810")]
        [TestCase("with random numbers 0987134 and symbols )(*&# 818483)(*&$%")]
        [TestCase("\nars\tt0;834\n\t0193874")]
        public void CounterDisplaysCorrectLabel(string counterLabel)
        {
            var configuration = MakeEmaCounterConfiguration(counterLabel, new List<MetricId>{k_UnitlessGauge});
            var counter = new CounterVisualElement();
            counter.UpdateConfiguration(configuration);

            var label = counter.Q<Label>(classes: new[] { UssClassNames.k_DisplayElementLabel });
            Assert.AreEqual(counterLabel, label.text);
        }

        [TestCase(0, 1, -0.01)]
        [TestCase(0, 1, 0.5)]
        [TestCase(0, 1, 4.7)]
        [TestCase(-482, -231, -1111)]
        [TestCase(-482, -231, -337)]
        [TestCase(-482, -231, -112)]
        [TestCase(-482, -231, 483)]
        [TestCase(-482, 7193, -484)]
        [TestCase(-482, 7193, -483)]
        [TestCase(-482, 7193, -482)]
        [TestCase(-482, 7193, -481)]
        [TestCase(-482, 7193, 7192)]
        [TestCase(-482, 7193, 7193)]
        [TestCase(-482, 7193, 7194)]

        // Malformed cases where upperBound < lowerBound
        [TestCase(482, -7193, -7194)]
        [TestCase(482, -7193, -7193)]
        [TestCase(482, -7193, -7192)]
        [TestCase(482, -7193, -2000)]
        [TestCase(482, -7193, -200)]
        [TestCase(482, -7193, 0)]
        [TestCase(482, -7193, 481)]
        [TestCase(482, -7193, 482)]
        [TestCase(482, -7193, 483)]

        public void CountersWithOutOfBoundsValuesHaveUssHighlightClasses(
            float highlightLowerBound,
            float highlightUpperBound,
            double counterValue)
        {
            var counter = MakeEmaCounter(
                "Unit Test Counter",
                new List<MetricId>{k_UnitlessGauge},
                highlightLowerBound: highlightLowerBound,
                highlightUpperBound: highlightUpperBound);

            counter.DisplayValue = counterValue;

            var belowMin = counterValue < highlightLowerBound;
            var aboveMax = counterValue > highlightUpperBound;
            var outOfBounds = belowMin || aboveMax;

            Assert.AreEqual(
                belowMin,
                counter.ClassListContains(UssClassNames.k_CounterBelowThreshold),
                "Counters should have the 'below-threshold' USS class if-and-only-if they are below the threshold");

            Assert.AreEqual(
                aboveMax,
                counter.ClassListContains(UssClassNames.k_CounterAboveThreshold),
                "Counters should have the 'above-threshold' USS class if-and-only-if they are above the threshold");

            Assert.AreEqual(
                outOfBounds,
                counter.ClassListContains(UssClassNames.k_CounterOutOfBounds),
                "Counters should have the 'out-of-bounds' USS class if-and-only-if they are above the threshold");
        }

        [TestCase(TestMetric.UnitlessGauge, 1, 0.0, "0")]
        [TestCase(TestMetric.UnitlessGauge, 2, 0.0, "0.0")]
        [TestCase(TestMetric.UnitlessGauge, 4, 0.0, "0.000")]

        [TestCase(TestMetric.SecondsGauge, 1, 0.1453, "100"   + " " + "ms")]
        [TestCase(TestMetric.SecondsGauge, 2, 0.1453, "150"   + " " + "ms")]
        [TestCase(TestMetric.SecondsGauge, 3, 0.1453, "145"   + " " + "ms")]
        [TestCase(TestMetric.SecondsGauge, 4, 0.1453, "145.3" + " " + "ms")]

        [TestCase(TestMetric.SecondsGauge, 1, 145.3e-6, "100"   + " " + "μs")]
        [TestCase(TestMetric.SecondsGauge, 2, 145.3e-6, "150"   + " " + "μs")]
        [TestCase(TestMetric.SecondsGauge, 3, 145.3e-6, "145"   + " " + "μs")]
        [TestCase(TestMetric.SecondsGauge, 4, 145.3e-6, "145.3" + " " + "μs")]

        [TestCase(TestMetric.BytesGauge, 1, 117.0, "100"     + " " + "B")]
        [TestCase(TestMetric.BytesGauge, 2, 117.0, "120"     + " " + "B")]
        [TestCase(TestMetric.BytesGauge, 3, 117.0, "117"     + " " + "B")]
        [TestCase(TestMetric.BytesGauge, 4, 117.0, "117.0"   + " " + "B")]
        [TestCase(TestMetric.BytesGauge, 5, 117.0, "117.00"  + " " + "B")]
        [TestCase(TestMetric.BytesGauge, 6, 117.0, "117.000" + " " + "B")]

        // The values should be rounded to the correct number of significant digits
        [TestCase(TestMetric.BytesCounter, 3, 117.386959, "117"        + " " + "B/s")]
        [TestCase(TestMetric.BytesCounter, 4, 117.386959, "117.4"      + " " + "B/s")]
        [TestCase(TestMetric.BytesCounter, 5, 117.386959, "117.39"     + " " + "B/s")]
        [TestCase(TestMetric.BytesCounter, 6, 117.386959, "117.387"    + " " + "B/s")]

        // They should support large integral numbers
        [TestCase(TestMetric.BytesCounter, 3,         321,          "321" + " " +  "B/s")]
        [TestCase(TestMetric.BytesCounter, 4,        4321,        "4.321" + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 5,       54321,       "54.321" + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 6,      654321,      "654.321" + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 6,     7654321,     "7.65432"  + " " + "MB/s")]
        [TestCase(TestMetric.BytesCounter, 6,    87654321,    "87.6543"   + " " + "MB/s")]
        [TestCase(TestMetric.BytesCounter, 6,   987654321,   "987.654"    + " " + "MB/s")]
        [TestCase(TestMetric.BytesCounter, 6, 10987654321, "10.9877"      + " " + "GB/s")]

        // They should support large numbers with trailing decimals
        [TestCase(TestMetric.BytesCounter, 4,       321.386959,         "321.4"    + " " +  "B/s")]
        [TestCase(TestMetric.BytesCounter, 5,      4321.386959,       "4.3214"     + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 6,     54321.386959,      "54.3214"     + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 6,    654321.386959,     "654.321"      + " " + "kB/s")]
        [TestCase(TestMetric.BytesCounter, 6,   7654321.386959,   "7.65432"        + " " + "MB/s")]
        [TestCase(TestMetric.BytesCounter, 6,  87654321.386959,  "87.6543"         + " " + "MB/s")]
        [TestCase(TestMetric.BytesCounter, 6, 987654321.386959, "987.654"          + " " + "MB/s")]
        public static void TestCounterDisplayValue(
            TestMetric metric,
            int significantDigits,
            double counterValue,
            string expectedDisplayValue)
        {
            var counter = MakeEmaCounter(
                "Unit Test Counter",
                new List<MetricId> { MetricId.Create(metric), },
                significantDigits: significantDigits);

            counter.DisplayValue = counterValue;

            var value = counter.Q<Label>(classes: new[] { UssClassNames.k_CounterValue });
            Assert.AreEqual(expectedDisplayValue, value.text);
        }

        [TestCase("de-DE", TestMetric.UnitlessGauge, 7, 987654321.386959, "987,6543" + "M")]
        [TestCase("en-US", TestMetric.UnitlessGauge, 7, 987654321.386959, "987.6543" + "M")]
        [TestCase("en-ZA", TestMetric.UnitlessGauge, 7, 987654321.386959, "987,6543" + "M")]
        [TestCase("fr-FR", TestMetric.UnitlessGauge, 7, 987654321.386959, "987,6543" + "M")]
        [TestCase("hi-IN", TestMetric.UnitlessGauge, 7, 987654321.386959, "987.6543" + "M")]
        [TestCase("zh-CN", TestMetric.UnitlessGauge, 7, 987654321.386959, "987.6543" + "M")]
        public static void TestCounterDisplayValue_Localized(
            string cultureCode,
            TestMetric metric,
            int decimalPlaces,
            double counterValue,
            string expectedDisplayValue)
        {
            var prevCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo(cultureCode, false);

            TestCounterDisplayValue(metric, decimalPlaces, counterValue, expectedDisplayValue);

            CultureInfo.CurrentCulture = prevCulture;
        }

        static DisplayElementConfiguration MakeEmaCounterConfiguration(
            string label,
            List<MetricId> stats,
            AggregationMethod aggregationMethod = AggregationMethod.Average,
            float halfLife = 1f,
            int significantDigits = 3,
            float highlightLowerBound = float.MinValue,
            float highlightUpperBound = float.MaxValue) => new()
            {
                Label = label,
                Stats = stats,
                CounterConfiguration = new CounterConfiguration
                {
                    AggregationMethod = aggregationMethod,
                    SmoothingMethod = SmoothingMethod.ExponentialMovingAverage,
                    ExponentialMovingAverageParams = new ExponentialMovingAverageParams
                    {
                        HalfLife = halfLife
                    },
                    SignificantDigits = significantDigits,
                    HighlightLowerBound = highlightLowerBound,
                    HighlightUpperBound = highlightUpperBound,
                }
            };

        static CounterVisualElement MakeEmaCounter(
            string label,
            List<MetricId> stats,
            AggregationMethod aggregationMethod = AggregationMethod.Average,
            float halfLife = 1f,
            int significantDigits = 3,
            float highlightLowerBound = float.MinValue,
            float highlightUpperBound = float.MaxValue)
        {
            var configuration = MakeEmaCounterConfiguration(
                label,
                stats,
                aggregationMethod,
                halfLife,
                significantDigits,
                highlightLowerBound,
                highlightUpperBound);

            var counter = new CounterVisualElement();
            counter.UpdateConfiguration(configuration);
            return counter;
        }
    }
}