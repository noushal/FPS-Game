using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Common.Tests
{
    internal class ContinuousExponentialMovingAverageTests
    {
        [TestCase(0.3)]
        [TestCase(0.5)]
        [TestCase(0.7)]
        [TestCase(0.3)]
        public void InitializedCorrectlyWithOneArgument(double decayConstant)
        {
            var ema = new ContinuousExponentialMovingAverage(decayConstant);
            Assert.AreEqual(decayConstant, ema.DecayConstant);
            Assert.AreEqual(0f, ema.LastValue);
            Assert.AreEqual(Double.NegativeInfinity, ema.LastTime);
        }

        [TestCase(0.5)]
        [TestCase(0.7, -342.74)]
        [TestCase(0.3, 49.63)]
        public void InitializedCorrectlyWithTwoArguments(
            double decayConstant,
            double initialValue = 0f)
        {
            var ema = new ContinuousExponentialMovingAverage(decayConstant, initialValue);
            Assert.AreEqual(decayConstant, ema.DecayConstant);
            Assert.AreEqual(initialValue, ema.LastValue);
        }

        [TestCase(0.5)]
        [TestCase(0.7, -342.74)]
        [TestCase(0.3, 49.63)]
        [TestCase(0.3, 49.63, 0)]
        [TestCase(0.3, 147.2, -7)]
        [TestCase(0.3, 147.2, 38)]
        public void InitializedCorrectlyWithThreeArguments(
            double decayConstant,
            double initialValue = 0,
            double initialTime = Double.MinValue)
        {
            var ema = new ContinuousExponentialMovingAverage(decayConstant, initialValue, initialTime);
            Assert.AreEqual(decayConstant, ema.DecayConstant);
            Assert.AreEqual(initialValue, ema.LastValue);
        }

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
        public void GetDecayConstantForHalfLifeIsWorkingCorrectly(double halfLife)
        {
            var decayConstant = ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(halfLife);
            Assert.AreEqual(ContinuousExponentialMovingAverage.k_ln2 / halfLife, decayConstant);
        }

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
        public void CreateWithHalfLifeIsWorkingCorrectly(double halfLife)
        {
            var cema = ContinuousExponentialMovingAverage.CreateWithHalfLife(halfLife);
            Assert.AreEqual(ContinuousExponentialMovingAverage.k_ln2 / halfLife, cema.DecayConstant);
            Assert.AreEqual(0d, cema.LastValue);
            Assert.AreEqual(Double.NegativeInfinity, cema.LastTime);
        }

        [TestCase(-89.30,    4.7)]
        [TestCase(-12.80,   16.37)]
        [TestCase(  0.00, -148.6)]
        [TestCase(  0.47,    1.23)]
        [TestCase(111.70,  785.0)]
        public void ConstructedWithDefaultTimeAssumesNextInstantaneousSample(double time, double value)
        {
            var cema = ContinuousExponentialMovingAverage.CreateWithHalfLife(0.5);
            Assert.AreEqual(0, cema.LastValue);
            Assert.AreEqual(Double.NegativeInfinity, cema.LastTime);

            cema.AddSampleForGauge(value, time);
            Assert.AreEqual(value, cema.LastValue);
            Assert.AreEqual(value, cema.GetCounterValue(time));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 0, 1)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(0, 0, 1, 1)]
        [TestCase(0, 1, 0, 0)]
        [TestCase(0, 1, 0, 1)]
        [TestCase(0, 1, 1, 0)]
        [TestCase(0, 1, 1, 1)]
        [TestCase(0.003, -485, 0.005, 184.5)]
        [TestCase(0.004,  485, 0.078, -184.5)]
        [TestCase(0.003,  485, 10089, 184.5)]
        [TestCase(0.004,  485, 1e9, -184.5)]
        public void ConstructedWithInitialTimeDoesNotAssumeNextInstantaneousSample(
            double t0, // Time 0
            double v0, // Value at time 0
            double t1, // Time 1
            double v1) // Value at time 1
        {
            var decayConstant = ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(2);
            var cema = new ContinuousExponentialMovingAverage(decayConstant, v0, t0);
            Assert.AreEqual(v0, cema.LastValue);
            Assert.AreEqual(t0, cema.LastTime);
            Assert.AreEqual(cema.LastValue, cema.GetCounterValue(t0));

            cema.AddSampleForGauge(v1, t1);
            Assert.AreEqual(t1, cema.LastTime);
            Assert.AreEqual(cema.LastValue, cema.GetCounterValue(t1));

            // After adding a new sample, the result should be between the min and max sample values
            var minValue = Math.Min(v0, v1);
            var maxValue = Math.Max(v0, v1);

            Assert.LessOrEqual(minValue, cema.LastValue);
            Assert.LessOrEqual(cema.LastValue, maxValue);
        }

        [TestCase(4.89, 0, 1, 0.1)]
        [TestCase(-1178000, -964, 258, 100)]
        public void ValueRemainsConstantWithConstantInput(
            double value,
            double startTime,
            double endTime,
            double timeStep)
        {
            var cema = ContinuousExponentialMovingAverage.CreateWithHalfLife(7.5);
            for (double time = startTime; time < endTime; time += timeStep)
            {
                cema.AddSampleForGauge(value, time);
                Assert.AreEqual(time, cema.LastTime);
                Assert.AreEqual(value, cema.LastValue);
                Assert.AreEqual(value, cema.GetCounterValue(time));
            }
        }

        [TestCase(100, 0, 1, 0.1)]
        [TestCase(487.5, -192, 289, 34)]
        [TestCase(-487.5, -192, 289, 34)]
        public void ValueDecaysFromInitialValueWithZeroInput(
            double initialValue,
            double startTime,
            double endTime,
            double timeStep)
        {
            var decayConstant = ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(timeStep);
            var cema = new ContinuousExponentialMovingAverage(decayConstant, initialValue, startTime);

            Assert.AreEqual(startTime, cema.LastTime);
            Assert.AreEqual(initialValue, cema.LastValue);
            Assert.AreEqual(initialValue, cema.GetCounterValue(startTime));

            for (double time = startTime + timeStep; time < endTime; time += timeStep)
            {
                var lastValue = cema.LastValue;
                cema.AddSampleForGauge(0, time);
                Assert.AreEqual(time, cema.LastTime);
                Assert.Less(Math.Abs(cema.LastValue), Math.Abs(lastValue));
                Assert.Less(0, Math.Abs(cema.LastValue));
            }
        }

        [TestCase(100, 0, 1, 0.1)]
        [TestCase(487.5, -192, 289, 34)]
        [TestCase(-487.5, -192, 289, 34)]
        public void ValueRisesToMeetConstantInputWhenStartingFromZero(
            double constantValue,
            double startTime,
            double endTime,
            double timeStep)
        {
            var decayConstant = ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(timeStep);
            var cema = new ContinuousExponentialMovingAverage(decayConstant, 0d, startTime);

            Assert.AreEqual(startTime, cema.LastTime);
            Assert.AreEqual(0d, cema.LastValue);
            Assert.AreEqual(0d, cema.GetCounterValue(startTime));

            for (double time = startTime + timeStep; time < endTime; time += timeStep)
            {
                var lastValue = cema.LastValue;
                cema.AddSampleForGauge(constantValue, time);
                Assert.AreEqual(time, cema.LastTime);

                // The value is "charging" like a capacitor from zero to the constant value,
                // so will always be larger than the previous value but also always smaller
                // than the target value
                Assert.Less(Math.Abs(lastValue), Math.Abs(cema.LastValue));
                Assert.Less(Math.Abs(cema.LastValue), Math.Abs(constantValue));
            }
        }

        [TestCase(7)]
        [TestCase(1847)]
        [TestCase(4485741)]
        public void VerifyContinuousExponentialMovingAverageOfRandomSamples(int seed)
        {
            var random = new Random();
            var totalTime = random.NextDouble() * 5 + 5;
            var halfLife = random.NextDouble() * 99 + 1;
            var cema = ContinuousExponentialMovingAverage.CreateWithHalfLife(halfLife);

            var expectedResult = 0d;
            for (var time = 0d; time < totalTime; time += random.NextDouble() + 0.1)
            {
                var deltaTime = time - cema.LastTime;

                var newSample = random.NextDouble() * 100 - 50;
                cema.AddSampleForGauge(newSample, time);

                var blendFactor = 1d - Math.Exp(-deltaTime * cema.DecayConstant);
                var remainingDecayTime = totalTime - time;
                expectedResult += newSample * blendFactor * Math.Exp(-remainingDecayTime * cema.DecayConstant);

                Assert.AreEqual(expectedResult, cema.GetCounterValue(totalTime), 1e-7);
            }
        }

        [TestCase(7)]
        [TestCase(1847)]
        [TestCase(4485741)]
        public void VerifyContinuousExponentialMovingAverageOfRandomSamplesOverTime(int seed)
        {
            var random = new Random();
            var totalTime = random.NextDouble() * 5 + 5;
            var halfLife = random.NextDouble() * 99 + 1;
            var cema = ContinuousExponentialMovingAverage.CreateWithHalfLife(halfLife);

            var expectedResult = 0d;
            for (var time = 0d; time < totalTime; time += random.NextDouble() + 0.1)
            {
                var deltaTime = time - cema.LastTime;

                var newSample = random.NextDouble() * 100 - 50;
                cema.AddSampleForCounter(newSample, time);

                var blendFactor = 1d - Math.Exp(-deltaTime * cema.DecayConstant);
                var remainingDecayTime = totalTime - time;

                expectedResult += (newSample / deltaTime)
                                  * blendFactor
                                  * Math.Exp(-remainingDecayTime * cema.DecayConstant);

                Assert.AreEqual(expectedResult, cema.GetCounterValue(totalTime), 1e-7);
            }
        }
    }
}