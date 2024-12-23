using System;

namespace Unity.Multiplayer.Tools.Common
{
    /// <summary>
    /// The <see cref="ContinuousExponentialMovingAverage"/> (CEMA) is similar to the <see cref="ExponentialMovingAverage"/> (EMA),
    /// as both compute a smoothed value similar to a Simple Moving Average (SMA) without a buffer of past values.
    /// Unlike EMA however, which can only be used with evenly spaced samples, CEMA accounts for the interval between each
    /// sample, and thus can be used in cases where new samples are received at irregular intervals.
    /// </summary>
    /// <remarks>
    /// Although this problem of an EMA with irregularly spaced samples seems to be a bit of a niche problem and much
    /// more rarely discussed than EMA, there is some discussion of this issue here: <br/>
    /// https://stackoverflow.com/questions/1023860/ <br/>
    ///
    /// The paper "Algorithms for Unevenly Spaced Time Series: Moving Averages and Other Rolling Operators" by
    /// statistician Andreas Eckner describes something similar here: <br/>
    /// http://www.eckner.com/research.html
    /// </remarks>
    internal class ContinuousExponentialMovingAverage
    {
        const double k_DefaultInitialTime = double.NegativeInfinity;

        public static readonly double k_ln2 = Math.Log(2);

        /// The decay constant is λ = ln(2) / HalfLife. A larger decay constant will results in a faster,
        /// more responsive CEMA with less smoothing, whereas a smaller decay constant will result in a slower,
        /// less responsive CEMA with more smoothing.
        public double DecayConstant { get; private set; }

        /// The existing smoothed value based on the most recent sample at T = LastTime.
        public double LastValue { get; private set; }

        /// The time at which the most recent sample was received
        public double LastTime { get; private set; }

        public static ContinuousExponentialMovingAverage CreateWithHalfLife(double halfLife) =>
            new ContinuousExponentialMovingAverage(GetDecayConstantForHalfLife(halfLife));

        /// The decay constant is λ = ln(2) / HalfLife. A larger decay constant will results in a faster,
        /// more responsive CEMA with less smoothing, whereas a smaller decay constant will result in a slower,
        /// less responsive CEMA with more smoothing.
        public static double GetDecayConstantForHalfLife(double halfLife) => k_ln2 / halfLife;

        public ContinuousExponentialMovingAverage(double decayConstant, double value = 0d, double time = k_DefaultInitialTime)
        {
            if (decayConstant < 0)
            {
                throw new ArgumentException(
                    $"ContinuousExponentialMovingAverage decay constant {decayConstant} should be >= 0; "
                    + "otherwise it will grow exponentially over time.");
            }
            DecayConstant = decayConstant;
            LastValue = value;
            LastTime = time;
        }

        public void Reset()
        {
            DecayConstant = 0;
            LastValue = 0;
            LastTime = k_DefaultInitialTime;
        }

        public void ClearValueAndTime()
        {
            LastValue = 0;
            LastTime = k_DefaultInitialTime;
        }

        /// Adds a new sample to the Continuous Exponential Moving Average (CEMA).
        /// Use this method if the input is a sample of an instantaneous value, such as the amount of memory in
        /// use, number of connections open, or number of objects in the scene.
        /// If this method is used, then the units of the CEMA output value will be the same as that of its
        /// input samples.
        public void AddSampleForGauge(double sample, double time)
        {
            var deltaTime = time - LastTime;
            var oldValueWeight = Math.Exp(-deltaTime * DecayConstant);
            var newSampleWeight = 1 - oldValueWeight;
            LastValue += newSampleWeight * (sample - LastValue);
            LastTime = time;
        }

        /// Adds a new sample to the Continuous Exponential Moving Average (CEMA),
        /// and divides it by the time elapsed since the last sample.
        /// Use this method if the input sample is an amount that has accumulated within the sample period,
        /// such as the number of bytes or messages sent or received or the number of objects spawned within this
        /// period of time.
        /// If this method is used, then the units of the CEMA output value will be the units of its input samples
        /// over the units of time, for example Bytes per Second or Objects Spawned per Second.
        public void AddSampleForCounter(double sample, double time)
        {
            var deltaTime = time - LastTime;
            var newRate = sample / deltaTime;
            var oldValueWeight = Math.Exp(-deltaTime * DecayConstant);
            var newSampleWeight = 1 - oldValueWeight;
            LastValue += newSampleWeight * (newRate - LastValue);
            LastTime = time;
        }

        /// Returns the last smoothed value.
        /// This is similar to GetCounterValue, except that the value does not decay over the time since
        /// the last sample was received.
        public double GetGaugeValue()
        {
            return LastValue;
        }

        /// Returns the smoothed value at the given point in time, with decay applied over the time since
        /// the last sample was received.
        public double GetCounterValue(double time)
        {
            // This is similar to AddSampleForGauge, but the new sample is implicitly zero
            // and the result is not stored
            var deltaTime = time - LastTime;
            var oldValueWeight = Math.Exp(-deltaTime * DecayConstant);
            return LastValue * oldValueWeight;
        }
    }
}