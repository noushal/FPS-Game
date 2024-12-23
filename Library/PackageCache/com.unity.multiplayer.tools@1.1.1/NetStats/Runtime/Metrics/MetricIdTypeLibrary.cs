using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// For internal use.
    /// Static class to register <see cref="MetricId"/> and make them available to all tools.
    /// </summary>
    /// <remarks>
    /// There is no need to manually call this class for custom type as they are automatically
    /// registered through CodeGen if they are marked with <see cref="MetricTypeEnumAttribute"/>.
    /// </remarks>
    public static class MetricIdTypeLibrary
    {
        static readonly List<Type> k_Types = new List<Type>();
        static readonly List<string> k_TypeDisplayNames = new List<string>();
        static readonly List<int[]> k_EnumValues = new List<int[]>();
        static readonly List<string[]> k_EnumNames = new List<string[]>();
        static readonly List<string[]> k_EnumDisplayNames = new List<string[]>();
        static readonly List<MetricKind[]> k_MetricKinds = new List<MetricKind[]>();
        static readonly List<BaseUnits[]> k_Units = new List<BaseUnits[]>();
        static readonly List<bool[]> k_DisplayAsPercentage = new List<bool[]>();

        internal static IReadOnlyList<Type> Types => k_Types;
        internal static IReadOnlyList<string> TypeDisplayNames => k_TypeDisplayNames;

        // NOTE: Be careful debugging this static class.
        // Examining a static field in the debugger before the static constructor has completed
        // can result in the static constructor running a second time, which can overwrite the
        // fields in the process. Encountered 2022-03.

        static MetricIdTypeLibrary()
        {
            TypeRegistration.RunIfNeeded();
        }

        /// <summary>
        /// For internal use.
        /// Register an enum type to be used as a <see cref="MetricId"/>.
        /// </summary>
        /// <typeparam name="TEnumType">The enum type to register.</typeparam>
        public static void RegisterType<TEnumType>()
        {
            k_Types.Add(typeof(TEnumType));
        }

        internal static void TypeRegistrationPostProcess()
        {
            k_Types.Sort((a, b) =>
            {
                var aSortPriorityAttr =
                    a.GetCustomAttribute<MetricTypeSortPriorityAttribute>();
                var aSortPriority = aSortPriorityAttr?.SortPriority ?? SortPriority.Neutral;

                var bSortPriorityAttr =
                    b.GetCustomAttribute<MetricTypeSortPriorityAttribute>();
                var bSortPriority = bSortPriorityAttr?.SortPriority ?? SortPriority.Neutral;

                // Order first by the sort priority
                var sortPriorityComparison = aSortPriority.CompareTo(bSortPriority);
                if (sortPriorityComparison != 0)
                {
                    return sortPriorityComparison;
                }

                // Then by name
                return StringComparer.InvariantCulture.Compare(a.FullName, b.FullName);
           });

            foreach (var type in k_Types)
            {
                var metricTypeAttr = type.GetCustomAttribute<MetricTypeEnumAttribute>();
                var typeDisplayName = metricTypeAttr?.DisplayName ?? type.Name;

                var values = type.GetEnumValues().Cast<int>().ToArray();
                var names = type.GetEnumNames();
                Array.Sort(names, values);

                var displayNames = new string[values.Length];
                var kinds = new MetricKind[values.Length];
                var units = new BaseUnits[values.Length];
                var displayAsPercentage = new bool[values.Length];
                for (var i = 0; i < values.Length; ++i)
                {
                    var name = names[i];
                    var enumMemberInfo = type.GetMember(name).FirstOrDefault();

                    var metadata = enumMemberInfo?.GetCustomAttribute<MetricMetadataAttribute>();
                    if (metadata != null)
                    {
                        displayNames[i] = metadata.DisplayName ?? StringUtil.AddSpacesToCamelCase(name);
                        kinds[i] = metadata.MetricKind;
                        units[i] = metadata.Units.GetBaseUnits();
                        displayAsPercentage[i] = metadata.DisplayAsPercentage;
                    }
                    else
                    {
                        // The array entries will default to null, 0, or the enum value corresponding to zero
                    }
                    displayNames[i] ??= StringUtil.AddSpacesToCamelCase(name);

                    if (kinds[i] == MetricKind.Counter)
                    {
                        var existingUnit = units[i];
                        units[i] = existingUnit.WithSeconds(
                            (sbyte)(existingUnit.SecondsExponent - 1));
                    }
                }

                k_TypeDisplayNames.Add(typeDisplayName);
                k_EnumValues.Add(values);
                k_EnumNames.Add(names);
                k_EnumDisplayNames.Add(displayNames);
                k_MetricKinds.Add(kinds);
                k_Units.Add(units);
                k_DisplayAsPercentage.Add(displayAsPercentage);
            }
        }

        internal static bool IsValidTypeIndex(int index)
        {
            return 0 <= index && index < k_Types.Count;
        }

        internal static int GetTypeIndex(Type type)
        {
            return k_Types.IndexOf(type);
        }

        internal static Type GetType(int typeIndex)
        {
            return k_Types[typeIndex];
        }

        internal static bool ContainsType(Type type)
        {
            return k_Types.Contains(type);
        }

        internal static IReadOnlyList<int> GetEnumValues(int typeIndex)
        {
            return k_EnumValues[typeIndex];
        }

        internal static IReadOnlyList<string> GetEnumNames(int typeIndex)
        {
            return k_EnumNames[typeIndex];
        }

        [NotNull]
        internal static string GetEnumName(int typeIndex, int enumValue)
        {
            return GetEnumMetadata(k_EnumNames, typeIndex, enumValue) ?? enumValue.ToString();
        }

        internal static MetricKind GetEnumMetricKind(int typeIndex, int enumValue)
        {
            return GetEnumMetadata(k_MetricKinds, typeIndex, enumValue);
        }

        internal static IReadOnlyList<string> GetEnumDisplayNames(int typeIndex)
        {
            return k_EnumDisplayNames[typeIndex];
        }

        [NotNull]
        internal static string GetEnumDisplayName(int typeIndex, int enumValue)
        {
            return GetEnumMetadata(k_EnumDisplayNames, typeIndex, enumValue) ?? "";
        }

        internal static BaseUnits GetEnumUnit(int typeIndex, int enumValue)
        {
            return GetEnumMetadata(k_Units, typeIndex, enumValue);
        }

        internal static bool GetDisplayAsPercentage(int typeIndex, int enumValue)
        {
            return GetEnumMetadata(k_DisplayAsPercentage, typeIndex, enumValue);
        }

        static T GetEnumMetadata<T>(List<T[]> data, int typeIndex, int enumValue)
        {
            if (typeIndex > k_EnumValues.Count)
            {
                return default(T);
            }
            var index = Array.IndexOf(k_EnumValues[typeIndex], enumValue);
            return index == -1 ? default(T) : data[typeIndex][index];
        }
    }
}
