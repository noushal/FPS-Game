using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Common.Tests
{
    class ExponentialMovingAverageTestsTmp
    {

        [TestCase(0.3f)]
        [TestCase(0.5f)]
        [TestCase(0.7f)]
        [TestCase(0.3f)]
        public void InitializedCorrectlyWithOneArgument(float parameter)
        {
            var ema = new ExponentialMovingAverage(parameter);
            Assert.AreEqual(ema.Parameter, parameter);
            Assert.AreEqual(ema.Value, 0f);
        }

        [TestCase(0.5f)]
        [TestCase(0.7f, -342.74f)]
        [TestCase(0.3f, 49.63f)]
        public void InitializedCorrectlyWithTwoArguments(
            float parameter,
            float initialValue = 0f)
        {
            var ema = new ExponentialMovingAverage(parameter, initialValue);
            Assert.AreEqual(ema.Parameter, parameter);
            Assert.AreEqual(ema.Value, initialValue);
        }

        [Test]
        public void ValueRemainsConstantWithConstantInput()
        {
            var ema = new ExponentialMovingAverage(0.5f, 4.8f);
            Assert.AreEqual(4.8f, ema.Value);

            ema.AddSample(4.8f);
            Assert.AreEqual(4.8f, ema.Value);

            ema.AddSample(4.8f);
            Assert.AreEqual(4.8f, ema.Value);

            ema.AddSample(4.8f);
            Assert.AreEqual(4.8f, ema.Value);

            ema.AddSample(4.8f);
            Assert.AreEqual(4.8f, ema.Value);
        }

        [Test]
        public void ValueDecaysFromInitialValueWithZeroInput()
        {
            var ema = new ExponentialMovingAverage(0.5f, 1f);
            Assert.AreEqual(1f, ema.Value, delta: 1e-7f);

            ema.AddSample(0f);
            Assert.AreEqual(0.5f, ema.Value, delta: 1e-7f);

            ema.AddSample(0f);
            Assert.AreEqual(0.25f, ema.Value, delta: 1e-7f);

            ema.AddSample(0f);
            Assert.AreEqual(0.125f, ema.Value, delta: 1e-7f);

            ema.AddSample(0f);
            Assert.AreEqual(0.0625f, ema.Value, delta: 1e-7f);
        }

        [Test]
        public void ValueRisesToMeetConstantInputWhenStartingFromZero()
        {
            var ema = new ExponentialMovingAverage(0.5f, 0f);
            Assert.AreEqual(0f, ema.Value);

            ema.AddSample(16f);
            Assert.AreEqual(8f, ema.Value);

            ema.AddSample(16f);
            Assert.AreEqual(12f, ema.Value);

            ema.AddSample(16f);
            Assert.AreEqual(14f, ema.Value);

            ema.AddSample(16f);
            Assert.AreEqual(15f, ema.Value);
        }

        [Test]
        public void ValueIsComputedCorrectlyInResponseToVaryingInput()
        {
            var ema = new ExponentialMovingAverage(parameter: 0.5f, value: 0f);
            Assert.AreEqual(0.5f, ema.Parameter);
            Assert.AreEqual(0f, ema.Value);

            // With a parameter of 0.5, the moving average after each new value is the
            // average of the most recent value and the previous moving average

            ema.AddSample(10f);
            Assert.AreEqual(5f, ema.Value);

            ema.AddSample(13f);
            Assert.AreEqual(9f, ema.Value);

            ema.AddSample(9f);
            Assert.AreEqual(9f, ema.Value);

            ema.AddSample(11f);
            Assert.AreEqual(10f, ema.Value);

            ema.AddSample(7f);
            Assert.AreEqual(8.5f, ema.Value);

            ema.AddSample(7f);
            Assert.AreEqual(7.75f, ema.Value);

            ema.AddSample(8.25f);
            Assert.AreEqual(8f, ema.Value);

            ema.AddSample(4f);
            Assert.AreEqual(6f, ema.Value);

            ema.AddSample(4f);
            Assert.AreEqual(5f, ema.Value);

            ema.AddSample(4f);
            Assert.AreEqual(4.5f, ema.Value);

            ema.AddSample(2.5f);
            Assert.AreEqual(3.5f, ema.Value);
        }

        /// Just some tests that the formula in ApproximatingSimpleMovingAverage are working correctly.
        /// For more information, see that functions doc-comment and attached link
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(87)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(4967)]
        public void ApproximatingSimpleMovingAverageIsCorrect(int sampleCount)
        {
            var ema = ExponentialMovingAverage.ApproximatingSimpleMovingAverage(sampleCount);
            Assert.AreEqual(2f / (sampleCount + 1), ema.Parameter);
        }
    }
}