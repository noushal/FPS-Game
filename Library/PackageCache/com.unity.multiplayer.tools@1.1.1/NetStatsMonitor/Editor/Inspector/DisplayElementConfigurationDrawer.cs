using System.Linq;
using System.Reflection;
using Unity.Multiplayer.Tools.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Editor
{
    [CustomPropertyDrawer(typeof(DisplayElementConfiguration))]
    class DisplayElementConfigurationDrawer : PropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty configurationProp)
        {
            return new DisplayElementConfigurationInspector(configurationProp);
        }
    }

    class DisplayElementConfigurationInspector : VisualElement
    {
        internal static readonly string k_TypeFieldName;
        static readonly string k_LabelFieldName;
        static readonly string k_StatsFieldName;
        static readonly string k_CounterFieldName;
        static readonly string k_GraphFieldName;

        static DisplayElementConfigurationInspector()
        {
            var fields = typeof(DisplayElementConfiguration)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            string GetFieldName(string propertyName)
                => fields.First(field => field.Name.Contains(propertyName)).Name;

            k_TypeFieldName = GetFieldName(nameof(DisplayElementConfiguration.Type));
            k_LabelFieldName = GetFieldName(nameof(DisplayElementConfiguration.Label));
            k_StatsFieldName = GetFieldName(nameof(DisplayElementConfiguration.Stats));
            k_CounterFieldName = GetFieldName(nameof(DisplayElementConfiguration.CounterConfiguration));
            k_GraphFieldName = GetFieldName(nameof(DisplayElementConfiguration.GraphConfiguration));
        }

        readonly Foldout m_Foldout;
        VisualElement Content => m_Foldout.contentContainer;

        int m_DisplayElementConfigurationCount;

        PropertyField m_LabelField;
        PropertyField m_CounterField;
        PropertyField m_GraphField;

        internal DisplayElementConfigurationInspector(SerializedProperty configurationProp)
        {
            var parentOwner = configurationProp.GetParent();
            m_DisplayElementConfigurationCount = GetDisplayElementCount(configurationProp);

            m_Foldout = new Foldout();
            m_Foldout.text = configurationProp.displayName;
            Add(m_Foldout);

            var (typeProp, typeField) = Content.AddFieldForProperty(configurationProp, k_TypeFieldName);

            (_, m_LabelField) = Content.AddFieldForProperty(configurationProp, k_LabelFieldName);
            Content.AddFieldForProperty(configurationProp, k_StatsFieldName);

            (_, m_CounterField) = Content.AddFieldForProperty(configurationProp, k_CounterFieldName);
            (_, m_GraphField) = Content.AddFieldForProperty(configurationProp, k_GraphFieldName);

            OnTypeChanged(typeProp);
            typeField.RegisterValueChangeCallback(evt =>
            {
                OnTypeChanged(evt.changedProperty);
            });
        }

        void OnTypeChanged(SerializedProperty property)
        {
            var newCount = GetDisplayElementCount(property);
            if (m_DisplayElementConfigurationCount == 0
                || newCount < m_DisplayElementConfigurationCount)
            {
                 // Due to how UI Toolkit updates, this event will be propagated after an element was added, removed
                 // or moved in the list view. In the case of added or moved, things will be fine.
                 // But in the case of removal, we need an early return because other wise the display element
                 // will try to access data that is now gone, resulting in a backing field disappeared message.
                m_DisplayElementConfigurationCount = newCount;
                return;
            }

            m_DisplayElementConfigurationCount = newCount;

            if (property.propertyType != SerializedPropertyType.Enum)
            {
                return;
            }

            var type = (DisplayElementType)property.enumValueIndex;

            var counterVisible = type == DisplayElementType.Counter;

            var graphVisible =
                type == DisplayElementType.LineGraph ||
                type == DisplayElementType.StackedAreaGraph;

            // Requested by our UX designer (Selena). We want to indicate to users that the Label field
            // is optional for graphs. A default of null will provide the default label for the Label field.
            m_LabelField.label = "Label" + (graphVisible ? " (Optional)" : "");

            m_CounterField.SetInclude(counterVisible);
            m_GraphField.SetInclude(graphVisible);

            if (graphVisible)
            {
                m_GraphField.Q<GraphConfigurationInspector>()?.OnTypeChanged(type);
            }
        }

        int GetDisplayElementCount(SerializedProperty property)
        {
            return (property.serializedObject.targetObject as NetStatsMonitorConfiguration)?.DisplayElements?.Count ?? 0;
        }
    }
}