using System.Linq;
using System.Reflection;
using Unity.Multiplayer.Tools.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Editor
{
    [CustomPropertyDrawer(typeof(GraphConfiguration))]
    class GraphConfigurationDrawer : PropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty configurationProp)
        {
            return new GraphConfigurationInspector(configurationProp);
        }
    }

    class GraphConfigurationInspector : VisualElement
    {
        static readonly string k_SampleCountFieldName;
        static readonly string k_SampleRateFieldName;
        static readonly string k_XAxisTypeFieldName;
        static readonly string k_VariableColorsFieldName;
        static readonly string k_LineGraphConfigurationFieldName;

        static GraphConfigurationInspector()
        {
            var fields = typeof(GraphConfiguration)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            string GetFieldName(string propertyName)
                => fields.First(field => field.Name.Contains(propertyName)).Name;

            k_SampleCountFieldName = GetFieldName(nameof(GraphConfiguration.SampleCount));
            k_SampleRateFieldName = GetFieldName(nameof(GraphConfiguration.SampleRate));
            k_XAxisTypeFieldName = GetFieldName(nameof(GraphConfiguration.XAxisType));
            k_VariableColorsFieldName = GetFieldName(nameof(GraphConfiguration.VariableColors));
            k_LineGraphConfigurationFieldName = GetFieldName(nameof(GraphConfiguration.LineGraphConfiguration));
        }

        readonly Foldout m_Foldout;
        VisualElement Content => m_Foldout.contentContainer;

        readonly SerializedProperty m_LineGraphProp;
        readonly PropertyField m_LineGraphField;

        internal GraphConfigurationInspector(SerializedProperty configurationProp)
        {
            m_Foldout = new Foldout();
            m_Foldout.text = configurationProp.displayName;
            Add(m_Foldout);

            Content.AddFieldForProperty(configurationProp, k_SampleCountFieldName);
            Content.AddFieldForProperty(configurationProp, k_SampleRateFieldName);
            Content.AddFieldForProperty(configurationProp, k_XAxisTypeFieldName);
            Content.AddFieldForProperty(configurationProp, k_VariableColorsFieldName);

            (_, m_LineGraphField) = Content.AddFieldForProperty(configurationProp, k_LineGraphConfigurationFieldName);

            var displayElementType = (DisplayElementType)configurationProp
                .GetParent()
                .FindPropertyRelative(DisplayElementConfigurationInspector.k_TypeFieldName)
                .enumValueIndex;
            OnTypeChanged(displayElementType);
        }

        public void OnTypeChanged(DisplayElementType type)
        {
            m_LineGraphField.SetInclude(type == DisplayElementType.LineGraph);
        }
    }
}