using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class CounterConfigurationTests
    {
        private CounterConfiguration m_CounterConfiguration = new();
        
        [TestCase(7, ConfigurationLimits.k_CounterSampleMin)]
        [TestCase(-1, ConfigurationLimits.k_CounterSampleMin)]
        [TestCase(8, 8)]
        [TestCase(4096, 4096)]
        [TestCase(4097, ConfigurationLimits.k_CounterSampleMax)]
        [TestCase(int.MinValue, ConfigurationLimits.k_CounterSampleMin)]
        [TestCase(int.MaxValue, ConfigurationLimits.k_CounterSampleMax)]
        [Description("Test counter configuration sample count. If the smoothing method" +
                     " is set to ExponentialMovingAverage, the sample count will be 0")]
        public void SetCounterSampleCount_WithInBoundsOrOutOfBoundsValues_ReturnInBoundValueOrClampedValue(int sampleCountInput, int sampleCountExpected)
        {
            m_CounterConfiguration = new CounterConfiguration()
            {
                SmoothingMethod = SmoothingMethod.ExponentialMovingAverage,
                SimpleMovingAverageParams = new SimpleMovingAverageParams() { SampleCount = sampleCountInput }
            };
            Assert.AreEqual(0, m_CounterConfiguration.SampleCount);
            
            m_CounterConfiguration = new CounterConfiguration()
            {
                SmoothingMethod = SmoothingMethod.SimpleMovingAverage,
                SimpleMovingAverageParams = new SimpleMovingAverageParams() { SampleCount = sampleCountInput }
            };
            Assert.IsNotNull(m_CounterConfiguration.SimpleMovingAverageParams);
            Assert.AreEqual(sampleCountExpected, m_CounterConfiguration.SampleCount);
            Assert.That(m_CounterConfiguration.SampleCount, Is.InRange(ConfigurationLimits.k_CounterSampleMin, ConfigurationLimits.k_CounterSampleMax));
        }
        
        [TestCase(SmoothingMethod.ExponentialMovingAverage)]
        [TestCase(SmoothingMethod.SimpleMovingAverage)]
        public void SetCounterSmoothingMethod_WithSmoothingType_SmoothingMethodSetCorrectly(SmoothingMethod input)
        {
            m_CounterConfiguration.SmoothingMethod = input;
            Assert.AreEqual(input, m_CounterConfiguration.SmoothingMethod);
        }
        
        [TestCase(AggregationMethod.Average)]
        [TestCase(AggregationMethod.Sum)]
        public void SetCounterAggregationMethod_WithAggregationType_AggregationMethodSetCorrectly(AggregationMethod input)
        {
            m_CounterConfiguration.AggregationMethod = input;
            Assert.AreEqual(input, m_CounterConfiguration.AggregationMethod);
        }
        
        [TestCase(0, ConfigurationLimits.k_CounterSignificantDigitsMin)]
        [TestCase(-1, ConfigurationLimits.k_CounterSignificantDigitsMin)]
        [TestCase(1, 1)]
        [TestCase(7, 7)]
        [TestCase(8, ConfigurationLimits.k_CounterSignificantDigitsMax)]
        [TestCase(int.MinValue, ConfigurationLimits.k_CounterSignificantDigitsMin)]
        [TestCase(int.MaxValue, ConfigurationLimits.k_CounterSignificantDigitsMax)]
        public void SetCounterSignificantDigits_WithInBoundsOrOutOfBoundsValues_ReturnInBoundValueOrClampedValue(int significantDigitInput, int significantDigitsExpected)
        {
            m_CounterConfiguration.SignificantDigits = significantDigitInput;
            Assert.AreEqual(significantDigitsExpected, m_CounterConfiguration.SignificantDigits);
            Assert.That(m_CounterConfiguration.SignificantDigits, 
                Is.InRange(ConfigurationLimits.k_CounterSignificantDigitsMin, ConfigurationLimits.k_CounterSignificantDigitsMax));
        }
        
        [TestCase(0)]
        [TestCase(0.02f)]
        [TestCase(0.25f)]
        [TestCase(0.5f)]
        [TestCase(2.624f)]
        [TestCase(5.5f)]
        [TestCase(-1)]
        [TestCase(-100f)]
        [TestCase(-1000f)]
        [TestCase(float.MinValue)]
        [TestCase(float.MaxValue)]
        public void SetCounterHighlightUpperAndLowerBound_WithPositiveAndNegativeValue_HighlightUpperAndLowerBoundSetsCorrectly(float input)
        {
            m_CounterConfiguration.HighlightLowerBound = input;
            m_CounterConfiguration.HighlightUpperBound = input;
            Assert.AreEqual(input, m_CounterConfiguration.HighlightLowerBound);
            Assert.AreEqual(input, m_CounterConfiguration.HighlightUpperBound);
        }
        
        [TestCase(0, 0)]
        [TestCase(-0.1, ConfigurationLimits.k_ExponentialMovingAverageHalfLifeMin)]
        [TestCase(1, 1)]
        [TestCase(5, 5)]
        [TestCase(117, 117)]
        [TestCase(double.MinValue, ConfigurationLimits.k_ExponentialMovingAverageHalfLifeMin)]
        [TestCase(double.MaxValue, double.MaxValue)]
        public void SetCounterExponentialMovingAverageHalfLife_WithInBoundsOrOutOfBoundsValues_ReturnInBoundValueOrClampedValue(double input, double expectedOutput)
        {
            m_CounterConfiguration.ExponentialMovingAverageParams =
                new ExponentialMovingAverageParams() {HalfLife = input};
            Assert.IsNotNull(m_CounterConfiguration.ExponentialMovingAverageParams);
            Assert.AreEqual(expectedOutput, m_CounterConfiguration.ExponentialMovingAverageParams.HalfLife);
        }
    }
}