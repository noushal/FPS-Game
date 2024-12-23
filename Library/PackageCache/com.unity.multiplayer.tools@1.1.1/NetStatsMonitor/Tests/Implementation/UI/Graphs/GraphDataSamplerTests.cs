using System;
using NUnit.Framework;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation.Graphs
{
    class GraphDataSamplerTests
    {
        [TestCase(
            1f,
            new[] { 4, 5, 7, 9f },
            new[] { 4, 5, 7, 9f },
            TestName = "Stride: 1 sample")]
        [TestCase(
            0.5f,
            new[] {    4,    5,    7,    9f },
            new[] { 4, 4, 5, 5, 7, 7, 9, 9f },
            TestName = "Stride: 0.5 samples")]
        [TestCase(
            2f,
            new[] { 4, 5, 7, 9f },
            new[] { 4.5f, 8},
            TestName = "Stride: 2 samples")]
        [TestCase(
            0.75f,
            new[] { 4, 5, 7, 9f },
            new[] {
                4,
                (0.25f * 4 + .50f * 5) / 0.75f,
                (0.50f * 5 + .25f * 7) / 0.75f,
                7,
                9,
                9,
            },
            TestName = "Stride: 0.75 samples")]
        [TestCase(
            1.5f,
            new[] { 4, 5, 7, 9f },
            new[] {
                (1.0f * 4 + 0.5f * 5) / 1.5f,
                (0.5f * 5 + 1.0f * 7) / 1.5f,
                9,
            },
            TestName = "Stride: 1.5 samples")]
        [TestCase(
            4f,
            // Stat data is 20 random, non-zero digits
            new[] {
                9, 1, 4, 8,
                9, 6, 6, 6,
                4, 4, 1, 7,
                5, 1, 8, 7,
                3, 4, 8, 3f
            },
            new[] {
                (9 + 1 + 4 + 8) / 4f,
                (9 + 6 + 6 + 6) / 4f,
                (4 + 4 + 1 + 7) / 4f,
                (5 + 1 + 8 + 7) / 4f,
                (3 + 4 + 8 + 3) / 4f,
            },
            TestName = "Stride: 4 samples")]
        [TestCase(
            4.25f,
            // Stat data is 20 random, non-zero digits
            new[] {
                9, 1, 4, 8,
                9, 6, 6, 6,
                4, 4, 1, 7,
                5, 1, 8, 7,
                3, 4, 8, 3f
            },
            new[] {
                (1.00f * 9 + 1 + 4 + 8 + 0.25f * 9) / 4.25f,
                (0.75f * 9 + 6 + 6 + 6 + 0.50f * 4) / 4.25f,
                (0.50f * 4 + 4 + 1 + 7 + 0.75f * 5) / 4.25f,
                (0.25f * 5 + 1 + 8 + 7 + 1.00f * 3) / 4.25f,
                (1.00f * 4 + 8 + 3)                 / 3.00f,
            },
            TestName = "Stride: 4.25 samples")]
        public void SampleGauge_GivenInput_ProducesCorrectOutput(
            float stride,
            float[] input,
            float[] output)
        {
            var inputCount = input.Length;
            var pointCount = (int)(MathF.Ceiling(inputCount / stride));

            var samplesPerPoint = stride;

            var values = new RingBuffer<float>(input);

            var sampleIndex = 0f;
            for (var i = 0; i < pointCount; ++i)
            {
                var sampleValue = GraphDataSampler.SampleGauge(
                    graphSamplesPerPoint: samplesPerPoint,
                    sampleCount: inputCount,
                    statData: values,
                    sampleIndex: ref sampleIndex);
                var expected = output[i];
                Assert.AreEqual(expected, sampleValue, 0.0001f);
            }
        }

        [TestCase(
            1f,
            new[] { 0, 1, 2, 3, 4d },
            new[] {    4, 5, 7, 9f },
            new[] {    4, 5, 7, 9f },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 1s, Stride: 1 sample")]
        [TestCase(
            0.5f,
            new[] { 0,    1,    2,    3,    4d },
            new[] {       4,    5,    7,    9f },
            new[] {    4, 4, 5, 5, 7, 7, 9, 9f },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 1s, Stride: 0.5 samples")]
        [TestCase(
            2f,
            new[] { 0, 1, 2, 3, 4d },
            new[] {    4, 5, 7, 9f },
            new[] {    4.5f,    8f },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 1s, Stride: 2 samples")]
        [TestCase(
            0.75f,
            new[] { 0, 1, 2, 3, 4d },
            new[] {    4, 5, 7, 9f },
            new[] {
                4,
                (0.25f * 4 + .50f * 5) / 0.75f,
                (0.50f * 5 + .25f * 7) / 0.75f,
                7,
                9,
                9,
            },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 1s, Stride: 0.75 samples")]
        [TestCase(
            1.5f,
            new[] { 0, 1, 2, 3, 4d },
            new[] {    4, 5, 7, 9f },
            new[] {
                (1.0f * 4 + 0.5f * 5) / 1.5f,
                (0.5f * 5 + 1.0f * 7) / 1.5f,
                9,
            },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 1s, Stride: 1.5 samples")]

        [TestCase(
            1f,
            new[] { 0.00,  0.25,  0.50,  0.75,  1.00 },
            new[] {        4.0f,  5.0f,  7.0f,  9.0f },
            new[] {       16.0f, 20.0f, 28.0f, 36.0f },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 0.25s, Stride: 1 samples")]
        [TestCase(
            0.5f,
            new[] { 0.00,        0.50,         1.00,         1.50,         2.00 },
            new[] {              4.0f,         5.0f,         7.0f,         9.0f },
            new[] {        8.0f, 8.0f, 10.0f, 10.0f, 14.0f, 14.0f, 18.0f, 18.0f },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 0.5s, Stride: 0.5 samples")]
        [TestCase(
            2f,
            new[] { 0,  2,  4,  6, 8d },
            new[]    {  4,  5,  7, 9f },
            new[]    {  2.25f,  4f    },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 2s, Stride: 2 samples")]
        [TestCase(
            0.75f,
            new[] { 0,  4,  8, 12, 16d },
            new[] {     4,  5,  7,  9f },
            new[] {
                0.25f * 4,
                0.25f * (0.25f * 4 + .50f * 5) / 0.75f,
                0.25f * (0.50f * 5 + .25f * 7) / 0.75f,
                0.25f * 7,
                0.25f * 9,
                0.25f * 9,
            },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 4s, Stride: 0.75 samples")]
        [TestCase(
            1.5f,
            new[] { 0,  8, 16, 24, 32d },
            new[] {     4,  5,  7,  9f },
            new[] {
                0.125f * (1.0f * 4 + 0.5f * 5) / 1.5f,
                0.125f * (0.5f * 5 + 1.0f * 7) / 1.5f,
                0.125f * 9,
            },
            Category = "Regularly Spaced Timestamps",
            TestName = "Interval: 8s, Stride = 1.5 samples")]

        [TestCase(
            1f,
            new[] { 0, 0.125, 0.375, 0.875, 1.875 },
            new[] {        4,     5,     7,    9f },
            new[] {       32,    20,    14,    9f },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 1 sample")]
        [TestCase(
            0.5f,
            new[] { 0, 0.125,  0.375,  0.875,  1.875 },
            new[] {        4,      5,      7,     9f },
            new[] {   32, 32, 20, 20, 14, 14, 9, 9f },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 0.5 samples")]
        [TestCase(
            2f,
            new[] { 0,  0.125,  0.375,  0.875,  1.875 },
            new[] {         4,      5,      7,    9f },
            new[] {  (4 + 5) / 0.375f, (7 + 9) / 1.5f },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 2 samples")]
        [TestCase(
            0.75f,
            new[] { 0, 0.125, 0.375, 0.875, 1.875 },
            new[] {        4,     5,     7,    9f },
            new[] {
                 0.75f * 4              / (0.75f * 0.125f),
                (0.25f * 4 + 0.50f * 5) / (0.25f * 0.125f + 0.50f * 0.25f),
                (0.50f * 5 + 0.25f * 7) / (0.50f * 0.25f  + 0.25f * 0.50f),
                 0.75f * 7              / (0.75f * 0.5f),
                 0.75f * 9              / (0.75f * 1.0f),
                 0.25f * 9              / (0.25f * 1.0f),
            },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 0.75 samples")]
        [TestCase(
            1.5f,
            new[] { 0, 0.125, 0.375, 0.875, 1.875 },
            new[] {        4,     5,     7,    9f },
            new[] {
                (1.0f * 4 + 0.5f * 5) / (1.0f * 0.125f + 0.5f * 0.25f),
                (0.5f * 5 + 1.0f * 7) / (0.5f * 0.25f  + 1.0f * 0.50f),
                 1.0f * 9             /  1.0f * 1.0f,
            },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 1.5 samples")]

        [TestCase(
            4f,
            // Intervals between each timestamp are 20 random, non-zero digits:
            new[] { 0d,
                 6,   7,  16,  19, // Intervals: 6, 1, 9, 3
                20,  27,  36,  39, // Intervals: 1, 7, 9, 3
                47,  51,  52,  61, // Intervals: 8, 4, 1, 9
                64,  73,  80,  81, // Intervals: 3, 9, 7, 1
                86,  91,  98, 102, // Intervals: 5, 5, 7, 4
            },
            // Stat data is 20 random, non-zero digits
            new[] {
                9, 1, 4, 8,
                9, 6, 6, 6,
                4, 4, 1, 7,
                5, 1, 8, 7,
                3, 4, 8, 3f
            },
            new[] {
                (9 + 1 + 4 + 8) / (6 + 1 + 9 + 3f),
                (9 + 6 + 6 + 6) / (1 + 7 + 9 + 3f),
                (4 + 4 + 1 + 7) / (8 + 4 + 1 + 9f),
                (5 + 1 + 8 + 7) / (3 + 9 + 7 + 1f),
                (3 + 4 + 8 + 3) / (5 + 5 + 7 + 4f),
            },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 4 samples")]
        [TestCase(
            4.25f,
            // Intervals between each timestamp are 20 random, non-zero digits:
            new[] { 0d,
                 6,   7,  16,  19, // Intervals: 6, 1, 9, 3
                20,  27,  36,  39, // Intervals: 1, 7, 9, 3
                47,  51,  52,  61, // Intervals: 8, 4, 1, 9
                64,  73,  80,  81, // Intervals: 3, 9, 7, 1
                86,  91,  98, 102, // Intervals: 5, 5, 7, 4
            },
            // Stat data is 20 random, non-zero digits
            new[] {
                9, 1, 4, 8,
                9, 6, 6, 6,
                4, 4, 1, 7,
                5, 1, 8, 7,
                3, 4, 8, 3f
            },
            new[] {
                (1.00f * 9 + 1 + 4 + 8 + 0.25f * 9) / (1.00f * 6 + 1 + 9 + 3 + 0.25f * 1),
                (0.75f * 9 + 6 + 6 + 6 + 0.50f * 4) / (0.75f * 1 + 7 + 9 + 3 + 0.50f * 8),
                (0.50f * 4 + 4 + 1 + 7 + 0.75f * 5) / (0.50f * 8 + 4 + 1 + 9 + 0.75f * 3),
                (0.25f * 5 + 1 + 8 + 7 + 1.00f * 3) / (0.25f * 3 + 9 + 7 + 1 + 1.00f * 5),
                (1.00f * 4 + 8 + 3)                 / (1.00f * 5 + 7 + 4),
            },
            Category = "Irregularly Spaced Timestamps",
            TestName = "Stride: 4.25 samples")]
        public void SampleCounter_GivenInput_ProducesCorrectOutput(
            float stride,
            double[] timeStamps,
            float[] input,
            float[] output)
        {
            var inputCount = input.Length;
            var pointCount = (int)(MathF.Ceiling(inputCount / stride));

            var samplesPerPoint = stride;

            var timeStampRingBuffer = new RingBuffer<double>(timeStamps);
            var statData = new RingBuffer<float>(input);

            var timeStampOffset = timeStamps.Length - input.Length;

            var sampleIndex = 0f;
            for (var i = 0; i < pointCount; ++i)
            {
                var sampleValue = GraphDataSampler.SampleCounter(
                    timeStamps: timeStampRingBuffer,
                    timeStampOffset: timeStampOffset,
                    statData: statData,
                    sampleCount: inputCount,
                    graphSamplesPerPoint: samplesPerPoint,
                    sampleIndex: ref sampleIndex);
                var expected = output[i];
                Assert.AreEqual(expected, sampleValue, 0.0001f,
                    $"GraphDataSampler.SampleCounter failed during iteration {i}:\n" +
                    $"    Expected: {expected}\n" +
                    $"    Received: {sampleValue}");
            }
        }
    }
}