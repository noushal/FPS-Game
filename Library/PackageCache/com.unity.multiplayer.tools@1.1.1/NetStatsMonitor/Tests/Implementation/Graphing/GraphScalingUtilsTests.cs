using System;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation.Graphing
{
    internal static class GraphScalingUtilsTests
    {

        // NextHighestRoundNumberTests, numbers in [0..10]
        // --------------------------------------------------------------------
        // The "round" numbers are 1, 1.5, 2, 3, 4, 5, 6, 8, and 10,
        // multiplied by any power of 10

        [TestCase(0f       , 0f  , 0)]

        [TestCase(0.85f    , 1f  , 0)]
        [TestCase(0.97f    , 1f  , 0)]
        [TestCase(1f       , 1f  , 0)]

        [TestCase(1.000001f, 1.5f, 0)]
        [TestCase(1.32f    , 1.5f, 0)]
        [TestCase(1.45f    , 1.5f, 0)]
        [TestCase(1.49f    , 1.5f, 0)]
        [TestCase(1.499999f, 1.5f, 0)]
        [TestCase(1.5f     , 1.5f, 0)]

        [TestCase(1.500001f, 2f  , 0)]
        [TestCase(1.5001f  , 2f  , 0)]
        [TestCase(1.75f    , 2f  , 0)]
        [TestCase(1.99999f , 2f  , 0)]
        [TestCase(2f       , 2f  , 0)]

        [TestCase(2.000001f, 3f  , 0)]
        [TestCase(2.4879f  , 3f  , 0)]
        [TestCase(2.4879f  , 3f  , 0)]
        [TestCase(2.5f     , 3f  , 0)]
        [TestCase(2.763912f, 3f  , 0)]
        [TestCase(2.999999f, 3f  , 0)]
        [TestCase(3f       , 3f  , 0)]

        [TestCase(3.000001f, 4f  , 0)]
        [TestCase(3.5f     , 4f  , 0)]
        [TestCase(3.999999f, 4f  , 0)]
        [TestCase(4f       , 4f  , 0)]

        [TestCase(4.000001f, 5f  , 0)]
        [TestCase(4.5f     , 5f  , 0)]
        [TestCase(4.999999f, 5f  , 0)]
        [TestCase(5f       , 5f  , 0)]

        [TestCase(5.000001f, 6f  , 0)]
        [TestCase(5.5f     , 6f  , 0)]
        [TestCase(5.999999f, 6f  , 0)]
        [TestCase(6f       , 6f  , 0)]

        [TestCase(6.000001f, 8f  , 0)]
        [TestCase(6.5f     , 8f  , 0)]
        [TestCase(6.999999f, 8f  , 0)]
        [TestCase(7f       , 8f  , 0)]
        [TestCase(7.000001f, 8f  , 0)]
        [TestCase(7.5f     , 8f  , 0)]
        [TestCase(7.999999f, 8f  , 0)]
        [TestCase(8f       , 8f  , 0)]

        [TestCase(8.000001f, 1f  , 1)]
        [TestCase(8.5f     , 1f  , 1)]
        [TestCase(8.999999f, 1f  , 1)]
        [TestCase(9f       , 1f  , 1)]
        [TestCase(9.000001f, 1f  , 1)]
        [TestCase(9.5f     , 1f  , 1)]
        [TestCase(9.999999f, 1f  , 1)]

        public static void NextHighestRoundNumber_AtVariousOrdersOfMagnitude_Test(
            float input,
            float expectedMantissa,
            int expectedExponentOf10)
        {
            // Try various exponents automatically, to avoid needing to write additional tests by hand
            for (int i = -9; i <= 9; ++i)
            {
                var powerOf10 = MathF.Pow(10, i);
                var actual = GraphScalingUtils.NextLargestRoundNumber(input * powerOf10);
                Assert.AreEqual(expectedMantissa, actual.Mantissa);
                if (expectedMantissa != 0)
                {
                    Assert.AreEqual(expectedExponentOf10 + i, actual.Exponent);
                }
            }
        }
    }
}