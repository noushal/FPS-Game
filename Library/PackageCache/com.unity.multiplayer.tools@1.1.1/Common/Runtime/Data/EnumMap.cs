using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Unity.Multiplayer.Tools.Common
{
    /// <summary>
    /// An <see cref="EnumMap{TEnum, TValue}"/> is analogous to a <see cref="System.Collections.Generic.Dictionary{TKey,TValue}"/>,
    /// except that it is optimized for continuous storage of a fixed number of keys in a fixed-length array.
    /// </summary>
    /// <typeparam name="TEnum">
    /// The type of enum used as the key of this EnumMap. In order to be used as the key of an enum map, the enum must:
    /// <list type="number">
    /// <item> Be backed by a 32-bit int (the default backing type) </item>
    /// <item> Have at least one element </item>
    /// <item> Start at 0 </item>
    /// <item> Be continuous and have no gaps </item>
    /// </list>
    /// If your enum does not meet these requirements you may want to use a dictionary instead.
    /// </typeparam>
    /// <remarks>
    /// In read/write benchmarks, this class is ~3.6× faster than a dictionary and allocates ~8× less.
    /// </remarks>
    class EnumMap<TEnum, TValue> : IEnumerable<KeyValuePair<TEnum, TValue>>
        where TEnum : unmanaged, Enum
    {
        // ReSharper disable StaticMemberInGenericType
        /// The number of values in TEnum
        static readonly int s_Count;

        readonly TValue[] m_Values;

        static EnumMap()
        {
            s_Count = EnumContinuity.ValidateEnumForEnumMap<TEnum, TValue>();
        }

        public EnumMap()
        {
            m_Values = new TValue[s_Count];
        }

        /// <param name="value">
        /// Value to be copied to each element in the underlying array
        /// </param>
        public EnumMap(TValue value)
            : this()
        {
#if UNITY_2021_3_OR_NEWER
            Array.Fill(m_Values, value);
#else
            // Array.Fill doesn't appear to be available in Unity < 2021.3
            for (var i = 0; i < m_Values.Length; ++i)
            {
                m_Values[i] = value;
            }
#endif
        }

        public EnumMap(TValue[] values)
        {
            Assert.AreEqual(values.Length, s_Count);
            m_Values = values;
        }

        public TValue this[TEnum key]
        {
            get => m_Values[CastEnumToInt(key)];
            set => m_Values[CastEnumToInt(key)] = value;
        }

        public int Count => s_Count;

        public TValue[] Values => m_Values;

        public void Add(TEnum key, TValue value)
        {
            m_Values[CastEnumToInt(key)] = value;
        }

        /// <remarks>
        /// Convert.ToInt32 is orders of magnitude slower than this method,
        /// and allocates with each call, so this method is needed in order
        /// for this class to perform better than a Dictionary.
        /// <br/>
        /// Using Convert.ToInt32 this class will perform a little worse
        /// (maybe 20-30% worse) than a Dictionary, and will allocate with
        /// each read and write.
        /// <br/>
        /// Using the unsafe CastEnumToInt method, this class performs
        /// ~3.6 times faster than a dictionary, and allocates 8 times less.
        /// </remarks>
        static unsafe int CastEnumToInt(TEnum enumValue)
        {
            return *(int*)(&enumValue);
        }

        static unsafe TEnum CastIntToEnum(int value)
        {
            return *(TEnum*)(&value);
        }

        public IEnumerator<KeyValuePair<TEnum, TValue>> GetEnumerator()
        {
            for (int i = 0; i < s_Count; ++i)
            {
                yield return new KeyValuePair<TEnum, TValue>(CastIntToEnum(i), m_Values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    static class EnumContinuity
    {
        public static (int min, int max, int uniqueValueCount)
            GetMinMaxAndUniqueValueCount<TEnum>()
            where TEnum : Enum
        {
            var enumValues = EnumUtil.GetValues<TEnum>();

            var max = int.MinValue;
            var min = int.MaxValue;
            var uniqueValues = new HashSet<int>();

            foreach (var enumValue in enumValues)
            {
                var value = Convert.ToInt32(enumValue);
                max = Math.Max(max, value);
                min = Math.Min(min, value);
                uniqueValues.Add(value);
            }

            return (min, max, uniqueValues.Count);
        }

        /// <summary>
        /// Ensure that the enum values are continuous for efficient storage.
        /// If not, a Dictionary should be used instead.
        /// </summary>
        /// <typeparam name="TEnum">
        /// The type of the enum used as the <see cref="EnumMap{TEnum, TValue}"/> key
        /// </typeparam>
        /// <exception cref="EmptyEnumException{TEnum,TValue}">
        /// Thrown if the enum is empty
        /// </exception>
        /// <exception cref="NonZeroEnumMinimumValueException{TEnum,TValue}">
        /// Thrown if the minimum value of the enum is non-zero
        /// </exception>
        /// <exception cref="DiscontinuousEnumException{TEnum,TValue}">
        /// Thrown if the enum is discontinuous
        /// </exception>
        /// <returns>
        /// Returns the number of unique enum elements
        /// </returns>
        public static int ValidateEnumForEnumMap<TEnum, TValue>()
            where TEnum : unmanaged, Enum
        {
            if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(int))
            {
                throw new UnhandledEnumBackingTypeException<TEnum, TValue>();
            }
            var (min, max, uniqueCount) = GetMinMaxAndUniqueValueCount<TEnum>();
            if (uniqueCount <= 0)
            {
                throw new EmptyEnumException<TEnum, TValue>();
            }
            if (min != 0)
            {
                throw new NonZeroEnumMinimumValueException<TEnum, TValue>();
            }
            var count = max - min + 1;
            if (count != uniqueCount)
            {
                throw new DiscontinuousEnumException<TEnum, TValue>();
            }
            return uniqueCount;
        }
    }

    class UnhandledEnumBackingTypeException<TEnum, TValue> : Exception
        where TEnum : unmanaged, Enum
    {
        public UnhandledEnumBackingTypeException()
            : base($"The enum {nameof(TEnum)} cannot be used as a key in an {nameof(EnumMap<TEnum, TValue>)} " +
                   $"because its backing type {Enum.GetUnderlyingType(typeof(TEnum))} is not {nameof(Int32)}. " +
                   $"This constraint is required by EnumMap.CastEnumToInt.")
        {}
    }
    class EmptyEnumException<TEnum, TValue> : Exception
        where TEnum : unmanaged, Enum
    {
        public EmptyEnumException()
            : base($"The enum {nameof(TEnum)} cannot be used as a key in an {nameof(EnumMap<TEnum, TValue>)} " +
                   $"because it is empty and has no values.")
        {}
    }
    class NonZeroEnumMinimumValueException<TEnum, TValue> : Exception
        where TEnum : unmanaged, Enum
    {
        public NonZeroEnumMinimumValueException()
            : base($"The enum {nameof(TEnum)} cannot be used as a key in an {nameof(EnumMap<TEnum, TValue>)} " +
                   $"because its minimum value is non-zero. Consider using a dictionary instead.")
        {}
    }
    class DiscontinuousEnumException<TEnum, TValue> : Exception
        where TEnum : unmanaged, Enum
    {
        public DiscontinuousEnumException()
            : base($"The enum {nameof(TEnum)} cannot be used as a key in an {nameof(EnumMap<TEnum, TValue>)} " +
                   $"because it is discontinuous, and {nameof(EnumMap<TEnum, TValue>)} requires continuous " +
                   $"keys for storage in a fixed array. Consider using a dictionary instead.")
        {}
    }
}
