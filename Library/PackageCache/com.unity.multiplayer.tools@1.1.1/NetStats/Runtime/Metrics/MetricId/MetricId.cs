using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// Wrapper around an enum with the <see cref="MetricTypeEnumAttribute"/>.
    /// The struct provide a way to create metric that can be used with multiplayer tools.
    /// </summary>
    [Serializable]
    public struct MetricId : IEquatable<MetricId>
    {
        [field: SerializeField]
        internal int TypeIndex { get; set; }

        [field: SerializeField]
        internal int EnumValue { get; set; }

        internal Type EnumType => MetricIdTypeLibrary.GetType(TypeIndex);

        [NotNull]
        internal string Name => MetricIdTypeLibrary.GetEnumName(TypeIndex, EnumValue);

        [NotNull]
        internal string DisplayName => MetricIdTypeLibrary.GetEnumDisplayName(TypeIndex, EnumValue);
        internal MetricKind MetricKind => MetricIdTypeLibrary.GetEnumMetricKind(TypeIndex, EnumValue);
        internal BaseUnits Units => MetricIdTypeLibrary.GetEnumUnit(TypeIndex, EnumValue);
        internal bool DisplayAsPercentage => MetricIdTypeLibrary.GetDisplayAsPercentage(TypeIndex, EnumValue);

        internal MetricId(int typeIndex, int enumValue)
        {
            if (!MetricIdTypeLibrary.IsValidTypeIndex(typeIndex))
            {
                throw new ArgumentOutOfRangeException(
                    $"Cannot construct {nameof(MetricId)} with out-of-range {nameof(TypeIndex)} {typeIndex}.");
            }
            TypeIndex = typeIndex;
            EnumValue = enumValue;
        }

        internal MetricId(Type enumType, int enumValue)
        {
            TypeIndex = MetricIdTypeLibrary.GetTypeIndex(enumType);
            EnumValue = enumValue;
        }

        /// <summary>
        /// Static function to create a <see cref="MetricId"/>.
        /// </summary>
        /// <param name="value">Enum value for the metric.</param>
        /// <typeparam name="T">An enum with the <see cref="MetricTypeEnumAttribute"/></typeparam>
        /// <returns></returns>
        public static MetricId Create<T>(T value)
            where T: struct, IConvertible
        {
            var enumType = typeof(T);
            var enumValue = value.ToInt32(CultureInfo.InvariantCulture);
            return new MetricId(enumType, enumValue);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MetricId"/> is equal to the current <see cref="MetricId"/>.
        /// </summary>
        /// <param name="other">The <see cref="MetricId"/> to compare with the current <see cref="MetricId"/>.</param>
        /// <returns>true if the specified <see cref="MetricId"/> is equal to the current <see cref="MetricId"/>; otherwise, false.</returns>
        public bool Equals(MetricId other)
        {
            return TypeIndex == other.TypeIndex && EnumValue == other.EnumValue;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object..</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MetricId)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="MetricId"/>.</returns>
        public override int GetHashCode()
        {
            return 173 * TypeIndex + 13 * EnumValue;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="MetricId"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="MetricId"/>.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Implicit operator to convert to string.
        /// </summary>
        /// <param name="metricId">The <see cref="MetricId"/> to convert to string.</param>
        /// <returns>The name of the <see cref="MetricId"/>.</returns>
        public static implicit operator string(MetricId metricId) => metricId.ToString();
    }
}
