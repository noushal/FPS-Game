using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Common.Tests
{
    internal class RingBufferExtensionTests
    {
        [Test]
        public void EmptyRingBufferSumIsZero()
        {
            var ring = new RingBuffer<int>(13);
            Assert.AreEqual(0, ring.Sum());
        }

        [Test]
        public void EmptyRingBufferAverageIsNan()
        {
            var ring = new RingBuffer<int>(13);
            Assert.IsTrue(float.IsNaN(ring.Average()));
        }

        [Test]
        public void RingBufferSumIsCorrect()
        {
            var ring = new RingBuffer<int>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
            Assert.AreEqual(20, ring.Sum());
        }

        [Test]
        public void RingBufferAverageIsCorrect()
        {
            var ring = new RingBuffer<int>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
            Assert.AreEqual(20f / 6f, ring.Average());
        }

        [Test]
        public void RingBufferSumIsCorrectAfterOverflow()
        {
            var ring = new RingBuffer<int>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
            ring.PushBack(13);
            ring.PushBack(21);
            ring.PushBack(34);
            ring.PushBack(55);
            ring.PushBack(89);

            Assert.AreEqual(
                5 + 8 + 13 + 21 + 34 + 55 + 89,
                ring.Sum());
        }

        [Test]
        public void RingBufferAverageIsCorrectAfterOverflow()
        {
            var ring = new RingBuffer<int>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
            ring.PushBack(13);
            ring.PushBack(21);
            ring.PushBack(34);
            ring.PushBack(55);
            ring.PushBack(89);

            Assert.AreEqual(
                (float)(5 + 8 + 13 + 21 + 34 + 55 + 89) / 7,
                ring.Average());
        }
    }
}