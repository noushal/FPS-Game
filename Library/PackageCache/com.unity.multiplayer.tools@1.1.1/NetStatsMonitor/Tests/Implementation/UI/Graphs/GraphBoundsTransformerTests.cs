using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation.Graphs
{
    class GraphBoundsTransformerTests
    {
        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            1.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Simple bounds, no change")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 2.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            0.5f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Double the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 0.5f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            2.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Halve the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 2.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            2.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Double the graph height")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 0.5f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            0.5f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Halve the graph height")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 2.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 2.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            1.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Double the graph height and double the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 2.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 0.5f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            4.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Double the graph height and halve the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 0.5f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 2.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            0.25f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Halve the graph height and double the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 0.5f, // New bounds (xMin, xMax, yMin, yMax)
            0.0f, 0.5f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            1.0f, 0.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Halve the graph height and halve the max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.5f, 1.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            2.0f,-1.0f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Increase the min y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
            0.5f, 1.5f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            1.0f,-0.5f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Increase the min and max y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
           -1.0f, 1.0f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            0.5f, 0.5f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Decrease the min y-axis value")]

        [TestCase(
            0.0f, 1.0f, 0.0f, 1.0f, // Old bounds (xMin, xMax, yMin, yMax)
            0.0f, 1.0f,             // Old Y-Axis values (min, max)
            0.0f, 1.0f, 0.0f, 1.0f, // New bounds (xMin, xMax, yMin, yMax)
           -0.5f, 0.5f,             // New Y-Axis values (min, max)
            0,                      // Points to advance
            1.0f, 0.0f,             // Expected X-Axis transform (multiply, add)
            1.0f, 0.5f,             // Expected Y-Axis transform (multiply, add)
            TestName = "Decrease the min and max y-axis value")]

        public void GraphBoundsTransformerTest(
            float boundsXMin,
            float boundsXMax,
            float boundsYMin,
            float boundsYMax,
            float yAxisMin,
            float yAxisMax,

            float newBoundsXMin,
            float newBoundsXMax,
            float newBoundsYMin,
            float newBoundsYMax,
            float newYAxisMin,
            float newYAxisMax,

            int pointsToAdvance,

            float expectedXMultiply,
            float expectedXAdd,

            float expectedYMultiply,
            float expectedYAdd)
        {

            var transformer = new GraphBoundsTransformer(
                boundsXMin,
                boundsXMax,
                boundsYMin,
                boundsYMax,
                yAxisMin,
                yAxisMax);

            // Apply the initial bounds a second time, expect no change
            {
                var (xAxisTransform, yAxisTransform) = transformer.ComputeTransformsForNewBounds(
                    boundsXMin,
                    boundsXMax,
                    boundsYMin,
                    boundsYMax,
                    yAxisMin,
                    yAxisMax,
                    pointsToAdvance: 0);

                const string k_Message =
                    "An update with the same bounds and no points to advance should require no change, " +
                    "and the resulting transforms should be Identity.";
                Assert.IsTrue(xAxisTransform.IsIdentity, message: k_Message);
                Assert.IsTrue(yAxisTransform.IsIdentity, message: k_Message);
            }

            // Apply the new bounds and points to advance, there may be a change
            {
                var (xAxisTransform, yAxisTransform) = transformer.ComputeTransformsForNewBounds(
                    newBoundsXMin,
                    newBoundsXMax,
                    newBoundsYMin,
                    newBoundsYMax,
                    newYAxisMin,
                    newYAxisMax,
                    pointsToAdvance);

                const string k_Message =
                    "An update with the new bounds and points to advance should produce the expected transforms.";

                Assert.AreEqual(expectedXMultiply, xAxisTransform.A, message: k_Message);
                Assert.AreEqual(expectedXAdd,      xAxisTransform.B, message: k_Message);

                Assert.AreEqual(expectedYMultiply, yAxisTransform.A, message: k_Message);
                Assert.AreEqual(expectedYAdd,      yAxisTransform.B, message: k_Message);
            }

            // Apply the new bounds a second time, expect no change
            {
                var (xAxisTransform, yAxisTransform) = transformer.ComputeTransformsForNewBounds(
                    newBoundsXMin,
                    newBoundsXMax,
                    newBoundsYMin,
                    newBoundsYMax,
                    newYAxisMin,
                    newYAxisMax,
                    pointsToAdvance: 0);

                const string k_Message =
                    "An update with the same bounds and no points to advance should require no change, " +
                    "and the resulting transforms should be Identity.";
                Assert.IsTrue(xAxisTransform.IsIdentity, message: k_Message);
                Assert.IsTrue(yAxisTransform.IsIdentity, message: k_Message);
            }
        }
    }
}