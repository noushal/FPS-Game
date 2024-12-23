using NUnit.Framework;

namespace Unity.Multiplayer.Tools.NetStats.Tests
{
    struct TestEventData
    {
        public int Int1;
        public bool Bool1;

        public void AssertEquals(TestEventData other)
        {
            Assert.AreEqual(Int1, other.Int1);
            Assert.AreEqual(Bool1, other.Bool1);
        }
    }
    
    class TestEventDataUsage
    {
        public EventMetric<TestEventData> eventData;
    }
}
