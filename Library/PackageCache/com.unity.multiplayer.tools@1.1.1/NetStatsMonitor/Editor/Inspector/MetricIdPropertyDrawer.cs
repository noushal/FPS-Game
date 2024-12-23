using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Editor
{
    [CustomPropertyDrawer(typeof(MetricId))]
    internal class MetricIdPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new MetricIdInspectorElement(property);
        }
    }

    class MetricIdInspectorElement : VisualElement
    {
        [NotNull]
        SerializedProperty m_TypeIndexProperty;

        [NotNull]
        SerializedProperty m_EnumValueProperty;

        [CanBeNull]
        readonly PopupField<string> m_TypeDropdown = new();

        [NotNull]
        readonly PopupField<string> m_ValueDropdown = new();

        static readonly string k_TypeIndexFieldName;
        static readonly string k_EnumValueFieldName;

        /// Array of types available for selection in the MetricIdInspector
        static readonly Type[] k_AvailableTypes;
        static readonly List<string> k_TypeNamesShown = new();

        static MetricIdInspectorElement()
        {
            const string k_TypeIndexPropertyName = nameof(MetricId.TypeIndex);
            const string k_EnumValuePropertyName = nameof(MetricId.EnumValue);

            var fields = typeof(MetricId)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            k_TypeIndexFieldName = fields
                .First(info => info.Name.Contains(k_TypeIndexPropertyName))
                .Name;

            k_EnumValueFieldName = fields
                .First(info => info.Name.Contains(k_EnumValuePropertyName))
                .Name;

            k_AvailableTypes = MetricIdTypeLibrary.Types
                .Where(t => t.CustomAttributes
                    .All(attr => attr.AttributeType != typeof(MetricTypeEnumHideInInspectorAttribute)))
                .ToArray();

            ComputeTypeDisplayNamesWithoutClashes();
        }

        static void ComputeTypeDisplayNamesWithoutClashes()
        {
            var typeCount = k_AvailableTypes.Length;

            var availableTypeShortNames = new string[typeCount];
            for (var i = 0; i < typeCount; ++i)
            {
                var type = k_AvailableTypes[i];
                var typeIndex = MetricIdTypeLibrary.GetTypeIndex(type);
                var displayName = MetricIdTypeLibrary.TypeDisplayNames[typeIndex];
                availableTypeShortNames[i] = displayName;
            }

            k_TypeNamesShown.Capacity = typeCount;
            for (var i = 0; i < typeCount; ++i)
            {
                var nameClash = false;
                for (var j = 0; j < typeCount; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (availableTypeShortNames[i] == availableTypeShortNames[j])
                    {
                        nameClash = true;
                    }
                }

                var nameShown = nameClash
                    ? k_AvailableTypes[i].FullName
                    : availableTypeShortNames[i];
                k_TypeNamesShown.Add(nameShown);
            }
        }

        internal MetricIdInspectorElement(SerializedProperty property)
        {
            m_TypeIndexProperty = property.FindPropertyRelative(k_TypeIndexFieldName);
            m_EnumValueProperty = property.FindPropertyRelative(k_EnumValueFieldName);

            VisualElement content = this;

            var typeIndex = m_TypeIndexProperty.intValue;
            var type = MetricIdTypeLibrary.GetType(m_TypeIndexProperty.intValue);

            if (k_AvailableTypes.Length > 1)
            {
                var foldout = new Foldout();
                foldout.text = property.displayName;
                Add(foldout);
                content = foldout.contentContainer;

                m_TypeDropdown = new PopupField<string>();
                m_TypeDropdown.label = "Type";
                m_TypeDropdown.choices = k_TypeNamesShown;
                m_TypeDropdown.index = Array.IndexOf(k_AvailableTypes, type);;
                content.Add(m_TypeDropdown);

                m_TypeDropdown.RegisterValueChangedCallback((evt =>
                {
                    var newTypeSelectionIndex = m_TypeDropdown.index;
                    var newType = k_AvailableTypes[newTypeSelectionIndex];
                    var newValueIndex = 0;

                    m_TypeIndexProperty.intValue = MetricIdTypeLibrary.GetTypeIndex(newType);
                    RefreshValueChoicesForType(newType);
                    m_ValueDropdown.index = newValueIndex;
                    UpdateStoredValueFromDropdownIndex();
                    CommitSerializedPropertyChanges();
                }));
            }

            m_ValueDropdown.label = "Value";
            RefreshValueChoicesForType(type);
            var values = MetricIdTypeLibrary.GetEnumValues(typeIndex);
            var value = m_EnumValueProperty.intValue;
            int index = values.IndexOf(value);
            m_ValueDropdown.index = index;
            m_ValueDropdown.RegisterValueChangedCallback((evt =>
            {
                UpdateStoredValueFromDropdownIndex();
                CommitSerializedPropertyChanges();
            }));

            content.Add(m_ValueDropdown);
        }

        void RefreshValueChoicesForType(Type type)
        {
            var typeIndex = MetricIdTypeLibrary.GetTypeIndex(type);
            var valueNames = MetricIdTypeLibrary.GetEnumDisplayNames(typeIndex);
            m_ValueDropdown.choices = valueNames.ToList();
        }

        void UpdateStoredValueFromDropdownIndex()
        {
            var typeIndex = m_TypeIndexProperty.intValue;
            var newValueIndex = m_ValueDropdown.index;
            var values = MetricIdTypeLibrary.GetEnumValues(typeIndex);
            var newValue = values[newValueIndex];
            m_EnumValueProperty.intValue = newValue;
        }

        void CommitSerializedPropertyChanges()
        {
            m_EnumValueProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
