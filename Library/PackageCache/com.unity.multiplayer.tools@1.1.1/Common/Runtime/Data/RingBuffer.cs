using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Multiplayer.Tools.Common
{
    /// A ring buffer, indexed from least recent (beginning at 0) to most recent (ending at length - 1).
    /// It is possible for a RingBuffer to have zero capacity, in which case it does not allocate storage,
    /// ignores values that are pushed to it, and its length will always be zero.
    class RingBuffer<T> : IEnumerable<T>
    {
        /// Backing buffer, null in the event that the buffer has zero capacity
        [CanBeNull]
        T[] m_Buffer;

        /// The index of the least-recent value
        int m_Begin;

        /// The number of values stored
        public int Length { get; set; }

        /// The capacity of the ring buffer.
        /// Writing to the capacity will resize the ring buffer if necessary,
        /// while preserving the most recent values that can fit within the new capacity.
        public int Capacity
        {
            get => m_Buffer?.Length ?? 0;
            set
            {
                ThrowIfCapacityLessThanZero(value);
                UpdateCapacity(value);
            }
        }

        /// <exception cref="ArgumentException">Throws ArgumentException if capacity is less than 0</exception>
        public RingBuffer(int capacity)
        {
            ThrowIfCapacityLessThanZero(capacity);
            if (capacity > 0)
            {
                m_Buffer = new T[capacity];
            }
            else
            {
                m_Buffer = null;
            }
            m_Begin = 0;
            Length = 0;
        }

        public RingBuffer(T[] values)
        {
            m_Buffer = values;
            m_Begin = 0;
            Length = values.Length;
        }

        /// Pushes a value to the back, overwriting values at the front if at capacity.
        /// If capacity is zero then there is nowhere to store the value and the value is ignored.
        public void PushBack(T value)
        {
            var capacity = Capacity;
            if (capacity <= 0)
            {
                return;
            }
            var next = (m_Begin + Length) % capacity;
            m_Buffer![next] = value; // Capacity > 0 implies that m_Buffer != null
            if (Length < capacity)
            {
                Length++;
            }
            else
            {
                // We're at capacity and have overwritten what was previously our first value
                m_Begin = (m_Begin + 1) % capacity;
            }
        }

        public void Clear()
        {
            Length = 0;
        }

        private void UpdateCapacity(int newCapacity)
        {
            var oldCapacity = Capacity;
            if (newCapacity == oldCapacity)
            {
                return;
            }
            if (newCapacity == 0)
            {
                m_Buffer = null;
                m_Begin = 0;
                Length = 0;
                return;
            }
            var oldBuffer = m_Buffer;
            var oldBegin = m_Begin;
            var oldLength = Length;

            m_Buffer = new T[newCapacity];
            m_Begin = 0;
            Length = Math.Min(oldLength, newCapacity);

            var newToOldOffset = oldBegin + (oldLength - Length);
            for (int newBufferIndex = 0; newBufferIndex < Length; ++newBufferIndex)
            {
                var oldBufferIndex = (newBufferIndex + newToOldOffset) % oldCapacity;
                m_Buffer[newBufferIndex] = oldBuffer![oldBufferIndex];
            }
        }

        bool ContainsIndex(int index)
        {
            return 0 <= index && index < Length;
        }

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        bool ContainsIndex(Index index)
        {
            return ContainsIndex(index.IsFromEnd ? index.Value - 1 : index.Value);
        }
#endif

        /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range</exception>
        void ThrowIfIndexOutOfRange(int index)
        {
            if (!ContainsIndex(index))
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Length})");
            }
        }

        /// <exception cref="ArgumentException">Thrown if the capacity is less than zero</exception>
        void ThrowIfCapacityLessThanZero(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException($"RingBuffer capacity argument {capacity} is < 0");
            }
        }

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range</exception>
        void ThrowIfIndexOutOfRange(Index index)
        {
            if (!ContainsIndex(index))
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Length})");
            }
        }
#endif

        /// Translates a logical index into an index in the underlying buffer.
        int GetBufferIndex(int index)
        {
            return (index + m_Begin) % Capacity;
        }

        /// Translates a logical index into an index in the underlying buffer.
        int GetBufferIndexFromEnd(int index)
        {
            return GetBufferIndex(Length - 1 - index);
        }

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        /// Translates a logical index into an index in the underlying buffer.
        int GetBufferIndex(Index index)
        {
            if (index.IsFromEnd)
            {
                return GetBufferIndexFromEnd(index.Value - 1);
            }
            else
            {
                return GetBufferIndex(index.Value);
            }
        }
#endif

        /// Ring buffer is indexed from least recent (beginning at 0) to most recent (ending at length - 1).
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T this[int index]
        {
            get
            {
                ThrowIfIndexOutOfRange(index);
                return m_Buffer![GetBufferIndex(index)];
            }
            set
            {
                ThrowIfIndexOutOfRange(index);
                m_Buffer![GetBufferIndex(index)] = value;
            }
        }

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        /// Ring buffer is indexed from least recent (beginning at 0) to most recent (ending at length - 1).
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T this[Index index]
        {
            get
            {
                ThrowIfIndexOutOfRange(index);
                return m_Buffer![GetBufferIndex(index)];
            }
            set
            {
                ThrowIfIndexOutOfRange(index);
                m_Buffer![GetBufferIndex(index)] = value;
            }
        }
#endif

        public T GetValueOrDefault(int index)
        {
            if (ContainsIndex(index))
            {
                return this[index];
            }
            return default;
        }

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        public T GetValueOrDefault(Index index)
        {
            if (ContainsIndex(index))
            {
                return this[index];
            }
            return default;
        }
#endif

        /// <exception cref="IndexOutOfRangeException">Throws IndexOutOfRange if length is 0</exception>
        public T LeastRecent => this[0];
        public T LeastRecentOrDefault => Length > 0 ? LeastRecent : default;

#if UNITY_2021_2_OR_NEWER // Index and ^i syntax are not available in lower versions
        /// <exception cref="IndexOutOfRangeException">Throws IndexOutOfRange if length is 0</exception>
        public T MostRecent => this[^1];
        public T MostRecentOrDefault => Length > 0 ? MostRecent : default;
#endif

        /// Enumerator from least to most recent
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; ++i)
            {
                yield return this[i];
            }
        }

        /// Enumerator from least to most recent
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}