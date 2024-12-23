using System;

using NUnit.Framework;

using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation.Graphs
{
    class GraphInputSynchronizerTest
    {
        /// <summary>
        /// Test the graph input synchronizer across multiple simulated frames.
        /// During each simulated frame the GraphInputSynchronizer is given N samples
        /// (provided in the `numberOfSamplesToEmitEachFrame` array), and returns the
        /// number of points to advance that frame. The number of points to advance it
        /// returns each frame is checked against the corresponding entry in the
        /// `expectedPointToAdvanceEachFrame` array.
        /// </summary>
        /// <param name="timeStamps">
        /// An optional list of preset timestamps. If not enough timestamps are provided,
        /// additional timestamps will be created automatically.
        /// </param>
        /// <param name="timeStampsBufferCapacity">
        /// The capacity of the timestamp buffer.
        /// </param>
        /// <param name="numberOfSamplesToEmitEachFrame">
        /// The number of samples to emit during each simulated frame.
        /// </param>
        /// <param name="samplesPerPoint">
        /// The number of samples per point in the graph.
        /// </param>
        /// <param name="expectedPointsToAdvanceEachFrame">
        /// The expected number of point to advance during each simulated frame.
        /// </param>

        [TestCase(
            new double[]{},                        // timeStamps
            1,                                     // timeStampBufferCapacity
            new []{ 1, 0, 2, 0, 1, 5, 0, 3, 7 },   // numberOfSamplesToEmitEachFrame
            1f,                                    // samplesPerPoint
            new []{ 1, 0, 1, 0, 1, 1, 0, 1, 1 },   // expectedPointsToAdvanceEachFrame
            TestName = "Simplest Case"
        )]

        [TestCase(
            new double[]{},                         // timeStamps
            1,                                      // timeStampBufferCapacity
            new []{ 0, 1, 0, 2, 0, 1, 5, 0, 3, 7 }, // numberOfSamplesToEmitEachFrame
            1f,                                     // samplesPerPoint
            new []{ 0, 1, 0, 1, 0, 1, 1, 0, 1, 1 }, // expectedPointsToAdvanceEachFrame
            TestName = "No Input on First Frame"
        )]

        [TestCase(
            new double[]{},                         // timeStamps
            5,                                      // timeStampBufferCapacity
            new []{ 0, 1, 0, 2, 0, 1, 5, 0, 3, 7 }, // numberOfSamplesToEmitEachFrame
            1f,                                     // samplesPerPoint
            new []{ 0, 1, 0, 2, 0, 1, 5, 0, 3, 5 }, // expectedPointsToAdvanceEachFrame
            TestName = "Larger buffer size"
        )]

        [TestCase(
            new double[]{},                         // timeStamps
            13,                                     // timeStampBufferCapacity
            new []{ 0, 1, 0, 2, 0, 1, 5, 0, 3, 7 }, // numberOfSamplesToEmitEachFrame
            1.5f,                                   // samplesPerPoint
            new []{ 0, 0, 0, 2, 0, 0, 4, 0, 2, 4 }, // expectedPointsToAdvanceEachFrame
            TestName = "1.5 samples per point"
        )]

        [TestCase(
            new double[]{},
            13,                                     // timeStampBufferCapacity
            new []{ 0, 1, 0, 2, 0, 1, 5, 0, 3, 7 }, // numberOfSamplesToEmitEachFrame
            0.75f,                                  // samplesPerPoint
            new []{ 0, 1, 0, 3, 0, 1, 7, 0, 4, 9 }, // expectedPointsToAdvanceEachFrame
            TestName = "0.75 samples per point"
        )]

        [TestCase(
            new double[]{},                         // timeStamps
            13,                                     // timeStampBufferCapacity
            new []
            {
                0,
                1, // 1    - 0 × 1.17 => 1
                0,
                2, // 3    - 2 × 1.17 => 0.66
                0,
                1, // 1.66 - 1 × 1.17 => 0.49
                5, // 5.49 - 4 × 1.17 => 0.81
                0,
                3, // 3.81 - 3 × 1.17 => 0.3
                7  // 7.30 - 6 × 1.17 => 0.28
            },                                      // numberOfSamplesToEmitEachFrame
            1.17f,                                  // SamplesPerPoint
            new []{ 0, 0, 0, 2, 0, 1, 4, 0, 3, 6 }, // expectedPointsToAdvanceEachFrame
            TestName = "1.17 samples per point"
        )]

        [TestCase(
            new double[]{},                         // timeStamps
            13,                                     // timeStampBufferCapacity
            new []
            {
                0,
                1, // 1   - 0 × 3.5 => 0
                0,
                2, // 3   - 0 × 3.5 => 0
                0,
                1, // 4   - 1 × 3.5 => 0.5
                5, // 5.5 - 1 × 3.5 => 2
                0,
                3, // 5   - 1 × 3.5 => 1.5
                7  // 8.5 - 2 × 3.5 => 1.5
            },                                      // numberOfSamplesToEmitEachFrame
            3.5f,                                   // SamplesPerPoint
            new []{ 0, 0, 0, 0, 0, 1, 1, 0, 1, 2 }, // expectedPointsToAdvanceEachFrame
            TestName = "3.5 samples per point"
        )]

        public void ComputeNumberOfPointsToAdvanceTest(
            double[] timeStamps,
            int timeStampsBufferCapacity,
            int[] numberOfSamplesToEmitEachFrame,
            float samplesPerPoint,
            int[] expectedPointsToAdvanceEachFrame)
        {
            var timeStampCount = timeStamps.Length;
            var lastTimeStamp = timeStamps.Length >= 1 ? timeStamps[^1] : 0d;
            var frameCount = Math.Min(
                numberOfSamplesToEmitEachFrame.Length,
                expectedPointsToAdvanceEachFrame.Length);

            var synchronizer = new GraphInputSynchronizer();

            var timeStampRingBuffer = new RingBuffer<double>(timeStampsBufferCapacity);

            var timeStampIndex = 0;
            for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex)
            {
                var samplesToEmitThisFrame = numberOfSamplesToEmitEachFrame[frameIndex];
                for (var sampleIndex = 0; sampleIndex < samplesToEmitThisFrame; ++sampleIndex)
                {
                    var remainingTimeStamps = timeStampCount - timeStampIndex;

                    // If we run out of explicit test stamps just increment from last value.
                    // Means not all tests need to write these out by hand
                    var timeStamp = remainingTimeStamps > 0
                        ? timeStamps[timeStampIndex]
                        : (lastTimeStamp - remainingTimeStamps);
                    ++timeStampIndex;

                    timeStampRingBuffer.PushBack(timeStamp);
                }
                var expectedPointsToAdvance = expectedPointsToAdvanceEachFrame[frameIndex];

                var pointsToAdvance = synchronizer.ComputeNumberOfPointsToAdvance(
                    timeStampRingBuffer,
                    samplesPerPoint);

                Assert.AreEqual(
                    expectedPointsToAdvance,
                    pointsToAdvance,
                    $"Unexpected pointsToAdvance in frame {frameIndex}.\n" +
                    $"  Expected: {expectedPointsToAdvance}\n" +
                    $"  Received: {pointsToAdvance}"
                );
            }
        }
    }
}