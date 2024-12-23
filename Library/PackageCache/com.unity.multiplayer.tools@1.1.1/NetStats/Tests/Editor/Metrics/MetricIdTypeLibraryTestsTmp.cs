using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    class MetricIdTypeLibraryTestsTmp
    {
        [Test]
        // This test validates that the test metric is included in the type library
        //   which is an outcome of the Assembly processing code,
        //   which is difficult to test on its own so we just verify that the outcome is correct
        // If this test fails, the problem is most likely in `MetricIdTypeRegistrationAssemblyProcessor.cs`
        public void VerifyTestMetricsIncludedInLibrary()
        {
            Assert.IsTrue(MetricIdTypeLibrary.ContainsType(typeof(TestMetricId)));
        }

        [Test]
        public void VerifyCorrectDisplayName()
        {
            Assert.AreEqual(
                MetricIdTypeLibrary.GetEnumDisplayName(
                    MetricIdTypeLibrary.GetTypeIndex(typeof(TestMetricId)),
                    TestMetricIdConstants.Test1Value),
                TestMetricIdConstants.Test1DisplayName);
        }
        
        [Test]
        public void VerifyCorrectUnits()
        {
            Assert.AreEqual(
                MetricIdTypeLibrary.GetEnumUnit(
                    MetricIdTypeLibrary.GetTypeIndex(typeof(TestMetricId)),
                    TestMetricIdConstants.Test1Value),
                TestMetricIdConstants.Test1Units.GetBaseUnits());
        }
        
        [Test]
        public void VerifyCorrectMetricKind()
        {
            Assert.AreEqual(
                MetricIdTypeLibrary.GetEnumMetricKind(
                    MetricIdTypeLibrary.GetTypeIndex(typeof(TestMetricId)),
                    TestMetricIdConstants.Test1Value),
                TestMetricIdConstants.Test1MetricKind);
        }
        
        [Test]
        public void VerifyCorrectName()
        {
            Assert.AreEqual(
                MetricIdTypeLibrary.GetEnumName(
                    MetricIdTypeLibrary.GetTypeIndex(typeof(TestMetricId)),
                    TestMetricIdConstants.Test1Value),
                TestMetricIdConstants.Test1Name);
        }
    }
}
