// Index and ^i syntax are not available in Unity < 2021.2
// so for simplicity's sake, only include these tests in higher versions
#if UNITY_2021_2_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Common.Tests
{
    internal class RingBufferTests
    {
        static void NoOp<T>(T t)
        {
            // No-op function: turns expression into statements to test exceptions
        }
        static void AssertIndexOutOfRange(RingBuffer<byte> ring, int i)
        {
            Assert.Throws<IndexOutOfRangeException>(() => NoOp(ring[i]));
        }
        static void AssertIndexOutOfRange(RingBuffer<byte> ring, Index i)
        {
            Assert.Throws<IndexOutOfRangeException>(() => NoOp(ring[i]));
        }

        [Test]
        public void RingBufferConstructedWithNegativeCapacityThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => NoOp(new RingBuffer<byte>(-1)));
            Assert.Throws<ArgumentException>(() => NoOp(new RingBuffer<byte>(-7)));
        }

        [Test]
        public void RingBufferConstructedWithZeroCapacityDoesNotStoreValues()
        {
            var ring = new RingBuffer<byte>(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);
            AssertIndexOutOfRange(ring, 1);
            AssertIndexOutOfRange(ring, -1);

            ring.PushBack(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);
            AssertIndexOutOfRange(ring, 1);
            AssertIndexOutOfRange(ring, -1);

            ring.PushBack(7);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);
            AssertIndexOutOfRange(ring, 1);
            AssertIndexOutOfRange(ring, -1);

            ring.PushBack(16);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);
            AssertIndexOutOfRange(ring, 1);
            AssertIndexOutOfRange(ring, -1);
        }

        [Test]
        public void EmptyRingBufferThrowsIndexOutOfRangeExceptionsWhenIndexed()
        {
            var ring = new RingBuffer<byte>(13);
            AssertIndexOutOfRange(ring, 0);
            AssertIndexOutOfRange(ring, 1);
            AssertIndexOutOfRange(ring, 3);
            AssertIndexOutOfRange(ring, -7);
            AssertIndexOutOfRange(ring, ^3);
        }

        [Test]
        public void RingBufferCanPushValues()
        {
            var ring = new RingBuffer<byte>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
        }

        [Test]
        public void RingBufferReportsLengthCorrectly()
        {
            var ring = new RingBuffer<byte>(7);
            Assert.AreEqual(ring.Length, 0);
            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 5);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 6);
        }

        [Test]
        public void RingBufferCanPushAndRead()
        {
            var ring = new RingBuffer<byte>(7);
            ring.PushBack(1);
            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            ring.PushBack(8);
            AssertIndexOutOfRange(ring, -1);
            Assert.AreEqual(ring[0], 1);
            Assert.AreEqual(ring[1], 1);
            Assert.AreEqual(ring[2], 2);
            Assert.AreEqual(ring[3], 3);
            Assert.AreEqual(ring[4], 5);
            Assert.AreEqual(ring[5], 8);
            AssertIndexOutOfRange(ring, 6);
            Assert.AreEqual(ring[^1], 8);
            Assert.AreEqual(ring[^2], 5);
            Assert.AreEqual(ring[^3], 3);
            Assert.AreEqual(ring[^4], 2);
            Assert.AreEqual(ring[^5], 1);
            Assert.AreEqual(ring[^6], 1);
            AssertIndexOutOfRange(ring, ^0);
            AssertIndexOutOfRange(ring, ^7);
        }

        [Test]
        public void RingBufferCanPushAndWriteAndRead()
        {
            var ring = new RingBuffer<byte>(7);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring[0] = 1;
            ring[^5] = 1;
            ring[2] = 2;
            ring[^3] = 3;
            ring[4] = 5;
            ring[^1] = 8;
            AssertIndexOutOfRange(ring, -1);
            Assert.AreEqual(ring[0], 1);
            Assert.AreEqual(ring[1], 1);
            Assert.AreEqual(ring[2], 2);
            Assert.AreEqual(ring[3], 3);
            Assert.AreEqual(ring[4], 5);
            Assert.AreEqual(ring[5], 8);
            AssertIndexOutOfRange(ring, 6);
            Assert.AreEqual(ring[^1], 8);
            Assert.AreEqual(ring[^2], 5);
            Assert.AreEqual(ring[^3], 3);
            Assert.AreEqual(ring[^4], 2);
            Assert.AreEqual(ring[^5], 1);
            Assert.AreEqual(ring[^6], 1);
            AssertIndexOutOfRange(ring, ^0);
            AssertIndexOutOfRange(ring, ^7);
        }

        [Test]
        public void RingBufferAtCapacityCanBePushed()
        {
            var ring = new RingBuffer<byte>(7);
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
        }

        [Test]
        public void RingBufferAtCapacityReportsLengthCorrectly()
        {
            var ring = new RingBuffer<byte>(7);
            Assert.AreEqual(ring.Length, 0);
            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 5);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 6);
            ring.PushBack(13);
            Assert.AreEqual(ring.Length, 7);
            ring.PushBack(21);
            Assert.AreEqual(ring.Length, 7);
            ring.PushBack(34);
            Assert.AreEqual(ring.Length, 7);
            ring.PushBack(55);
            Assert.AreEqual(ring.Length, 7);
            ring.PushBack(89);
            Assert.AreEqual(ring.Length, 7);
        }

        [Test]
        public void RingBufferAtCapacityCanPushAndRead()
        {
            var ring = new RingBuffer<byte>(7);
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
            AssertIndexOutOfRange(ring, -1);
            Assert.AreEqual(ring[0], 5);
            Assert.AreEqual(ring[1], 8);
            Assert.AreEqual(ring[2], 13);
            Assert.AreEqual(ring[3], 21);
            Assert.AreEqual(ring[4], 34);
            Assert.AreEqual(ring[5], 55);
            Assert.AreEqual(ring[6], 89);
            AssertIndexOutOfRange(ring, 7);
            Assert.AreEqual(ring[^1], 89);
            Assert.AreEqual(ring[^2], 55);
            Assert.AreEqual(ring[^3], 34);
            Assert.AreEqual(ring[^4], 21);
            Assert.AreEqual(ring[^5], 13);
            Assert.AreEqual(ring[^6], 8);
            Assert.AreEqual(ring[^7], 5);
            AssertIndexOutOfRange(ring, ^0);
            AssertIndexOutOfRange(ring, ^8);
        }

        [Test]
        public void RingBufferAtCapacityCanPushAndWriteAndRead()
        {
            var ring = new RingBuffer<byte>(7);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);
            ring.PushBack(0);

            ring[0] = 5;
            ring[^6] = 8;
            ring[2] = 13;
            ring[^4] = 21;
            ring[4] = 34;
            ring[^2] = 55;
            ring[6] = 89;

            AssertIndexOutOfRange(ring, -1);
            Assert.AreEqual(ring[0], 5);
            Assert.AreEqual(ring[1], 8);
            Assert.AreEqual(ring[2], 13);
            Assert.AreEqual(ring[3], 21);
            Assert.AreEqual(ring[4], 34);
            Assert.AreEqual(ring[5], 55);
            Assert.AreEqual(ring[6], 89);
            AssertIndexOutOfRange(ring, 7);
            Assert.AreEqual(ring[^1], 89);
            Assert.AreEqual(ring[^2], 55);
            Assert.AreEqual(ring[^3], 34);
            Assert.AreEqual(ring[^4], 21);
            Assert.AreEqual(ring[^5], 13);
            Assert.AreEqual(ring[^6], 8);
            Assert.AreEqual(ring[^7], 5);
            AssertIndexOutOfRange(ring, ^0);
            AssertIndexOutOfRange(ring, ^8);
        }

        [Test]
        public void CanIterateOverZeroCapacityRingBuffer()
        {
            var ring = new RingBuffer<byte>(0);
            ring.PushBack(0);
            ring.PushBack(7);
            Assert.IsEmpty(ring);
        }

        [Test]
        public void CanIterateOverEmptyRingBuffer()
        {
            var ring = new RingBuffer<byte>(8);
            Assert.IsEmpty(ring);
        }

        [Test]
        public void CanIterateOverRingBuffer()
        {
            var ring = new RingBuffer<byte>(8);
            ring.PushBack(1);
            Assert.AreEqual(
                new List<byte> { 1 },
                ring.ToList());

            ring.PushBack(1);
            ring.PushBack(2);
            ring.PushBack(3);
            ring.PushBack(5);
            Assert.AreEqual(
                new List<byte> { 1, 1, 2, 3, 5 },
                ring.ToList());
        }

        [Test]
        public void CanIterateOverRingBufferAtCapacity()
        {
            var ring = new RingBuffer<byte>(8);
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
                new List<byte> { 3, 5, 8, 13, 21, 34, 55, 89 },
                ring.ToList());
        }

        [Test]
        public void RingBufferCapacityCanBeChanged()
        {
            var ring = new RingBuffer<byte>(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);

            ring.Capacity = 4;
            Assert.AreEqual(ring.Capacity, 4);
            Assert.AreEqual(ring.Length, 0);

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 4);

            ring.Capacity = 2;
            Assert.AreEqual(ring.Capacity, 2);
        }

        [Test]
        public void RingBufferHasNoValuesAfterCapacitySetToZero()
        {
            var ring = new RingBuffer<byte>(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);

            ring.Capacity = 4;
            Assert.AreEqual(ring.Capacity, 4);
            Assert.AreEqual(ring.Length, 0);
            Assert.AreEqual(new List<byte> { }, ring.ToList());

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 4);
            Assert.AreEqual(new List<byte> { 2, 3, 5, 8 }, ring.ToList());

            ring.Capacity = 0;
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);
            Assert.AreEqual(new List<byte> { }, ring.ToList());
        }

        [Test]
        public void RingBufferPreservesValuesWhenCapacityIsIncreased()
        {
            var ring = new RingBuffer<byte>(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);

            ring.Capacity = 4;
            Assert.AreEqual(ring.Capacity, 4);
            Assert.AreEqual(ring.Length, 0);
            Assert.AreEqual(new List<byte> { }, ring.ToList());

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 4);
            Assert.AreEqual(
                new List<byte> { 2, 3, 5, 8 },
                ring.ToList());

            ring.Capacity = 8;
            Assert.AreEqual(ring.Capacity, 8);
            Assert.AreEqual(ring.Length, 4);
            Assert.AreEqual(
                new List<byte> { 2, 3, 5, 8 },
                ring.ToList());

            ring.PushBack(13);
            Assert.AreEqual(ring.Length, 5);
            ring.PushBack(21);
            Assert.AreEqual(ring.Length, 6);
            ring.PushBack(34);
            Assert.AreEqual(ring.Length, 7);
            ring.PushBack(55);
            Assert.AreEqual(ring.Length, 8);
            ring.PushBack(89);
            Assert.AreEqual(ring.Length, 8);
            Assert.AreEqual(
                new List<byte> { 3, 5, 8, 13, 21, 34, 55, 89 },
                ring.ToList());
        }

        [Test]
        public void RingBufferPreservesValuesWhenCapacityIsDecreased()
        {
            var ring = new RingBuffer<byte>(0);
            Assert.AreEqual(ring.Capacity, 0);
            Assert.AreEqual(ring.Length, 0);

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 0);
            AssertIndexOutOfRange(ring, 0);

            ring.Capacity = 6;
            Assert.AreEqual(ring.Capacity, 6);
            Assert.AreEqual(ring.Length, 0);
            Assert.AreEqual(new List<byte> { }, ring.ToList());

            ring.PushBack(1);
            Assert.AreEqual(ring.Length, 1);
            ring.PushBack(2);
            Assert.AreEqual(ring.Length, 2);
            ring.PushBack(3);
            Assert.AreEqual(ring.Length, 3);
            ring.PushBack(5);
            Assert.AreEqual(ring.Length, 4);
            ring.PushBack(8);
            Assert.AreEqual(ring.Length, 5);
            ring.PushBack(13);
            Assert.AreEqual(ring.Length, 6);
            ring.PushBack(21);
            Assert.AreEqual(ring.Length, 6);
            Assert.AreEqual(
                new List<byte> { 2, 3, 5, 8, 13, 21 },
                ring.ToList());

            ring.Capacity = 4;
            Assert.AreEqual(ring.Capacity, 4);
            Assert.AreEqual(ring.Length, 4);
            Assert.AreEqual(new List<byte> { 5, 8, 13, 21 }, ring.ToList());

            ring.PushBack(34);
            Assert.AreEqual(ring.Capacity, 4);
            Assert.AreEqual(ring.Length, 4);
            Assert.AreEqual(new List<byte> { 8, 13, 21, 34 }, ring.ToList());
        }

        [Test]
        public void RingBufferMostAndLeastRecentAreCorrect()
        {
            var ring = new RingBuffer<byte>(4);

            ring.PushBack(7); // => 7
            Assert.AreEqual(7, ring.MostRecent);
            Assert.AreEqual(7, ring.LeastRecent);

            ring.PushBack(3); // => 3 7
            Assert.AreEqual(3, ring.MostRecent);
            Assert.AreEqual(7, ring.LeastRecent);

            ring.PushBack(5); // => 5 3 7
            Assert.AreEqual(5, ring.MostRecent);
            Assert.AreEqual(7, ring.LeastRecent);

            ring.PushBack(11); // => 11 5 3 7
            Assert.AreEqual(11, ring.MostRecent);
            Assert.AreEqual(7, ring.LeastRecent);

            ring.PushBack(14); // => 14 11 5 3
            Assert.AreEqual(14, ring.MostRecent);
            Assert.AreEqual(3, ring.LeastRecent);

            ring.PushBack(6); // => 6 14 11 5
            Assert.AreEqual(6, ring.MostRecent);
            Assert.AreEqual(5, ring.LeastRecent);

            ring.PushBack(12); // => 12 6 14 11
            Assert.AreEqual(12, ring.MostRecent);
            Assert.AreEqual(11, ring.LeastRecent);

            ring.PushBack(4); // => 4 12 6 14
            Assert.AreEqual(4, ring.MostRecent);
            Assert.AreEqual(14, ring.LeastRecent);
        }
    }
}
#endif
