using System;

namespace Unity.Multiplayer.Tools.Common
{
    /// Exponential Moving Averages are great!
    /// They provide much of the same utility as a Simple Moving Average
    /// (such as the average of a ring buffer),
    /// but do not require multiple historical values.
    /// They are also more responsive than Simple Moving Averages, which tend to
    /// lag the input signal and have some other artifacts.
    /// Additional information is available here:
    /// https://en.wikipedia.org/wiki/Moving_average
    internal class ExponentialMovingAverage
    {
        public float Parameter { get; private set; }
        public float Value { get; private set; }

        /// Returns an Exponential Moving Average with a parameter that approximates
        /// a Simple Moving Average with N samples.
        /// A proof of this relationship is described here:
        /// https://en.wikipedia.org/wiki/Moving_average#Relationship_between_SMA_and_EMA
        public static ExponentialMovingAverage ApproximatingSimpleMovingAverage(int sampleCount) =>
            new ExponentialMovingAverage(GetParameterApproximatingSimpleMovingAverage(sampleCount));

        /// Returns an Exponential Moving Average parameter that approximates
        /// a Simple Moving Average with N samples.
        /// A proof of this relationship is described here:
        /// https://en.wikipedia.org/wiki/Moving_average#Relationship_between_SMA_and_EMA
        public static float GetParameterApproximatingSimpleMovingAverage(int sampleCount)
            => 2f / (sampleCount + 1);

        public ExponentialMovingAverage(float parameter, float value = 0f)
        {
            if (!(0 <= parameter && parameter <= 1f))
            {
                throw new ArgumentException(
                    $"ExponentialMovingAverage parameter {parameter} should be in range [0, 1]");
            }
            Parameter = parameter;
            Value = value;
        }

        public void ClearValue()
        {
            Value = 0;
        }

        public void ClearValueAndParameter()
        {
            Parameter = 0;
            Value = 0;
        }

        /// Adds a new sample to the Exponential Moving Average
        public void AddSample(float x)
        {
            Value = Parameter * x + (1f - Parameter) * Value;
        }
    }
}