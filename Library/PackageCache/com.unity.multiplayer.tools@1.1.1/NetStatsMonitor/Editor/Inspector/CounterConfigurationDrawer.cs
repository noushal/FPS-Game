using System.Linq;
using System.Reflection;
using Unity.Multiplayer.Tools.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Editor
{
    [CustomPropertyDrawer(typeof(CounterConfiguration))]
    internal class CounterConfigurationDrawer : PropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty configurationProp)
        {
            return new CounterConfigurationInspector(configurationProp);
        }
    }

    class CounterConfigurationInspector : VisualElement
    {
        static readonly string k_SmoothingMethodFieldName;
        static readonly string k_AggregationMethodFieldName;
        static readonly string k_SignificantDigitsFieldName;
        static readonly string k_HighlightLowerBoundFieldName;
        static readonly string k_HighlightUpperBoundFieldName;
        static readonly string k_ExponentialMovingAverageFieldName;
        static readonly string k_SimpleMovingAverageFieldName;

        static CounterConfigurationInspector()
        {
            var fields = typeof(CounterConfiguration)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            string GetFieldName(string propertyName)
                => fields.First(field => field.Name.Contains(propertyName)).Name;

            k_SmoothingMethodFieldName = GetFieldName(nameof(CounterConfiguration.SmoothingMethod));
            k_AggregationMethodFieldName = GetFieldName(nameof(CounterConfiguration.AggregationMethod));
            k_SignificantDigitsFieldName = GetFieldName(nameof(CounterConfiguration.SignificantDigits));
            k_HighlightLowerBoundFieldName = GetFieldName(nameof(CounterConfiguration.HighlightLowerBound));
            k_HighlightUpperBoundFieldName = GetFieldName(nameof(CounterConfiguration.HighlightUpperBound));
            k_ExponentialMovingAverageFieldName = GetFieldName(nameof(CounterConfiguration.ExponentialMovingAverageParams));
            k_SimpleMovingAverageFieldName = GetFieldName(nameof(CounterConfiguration.SimpleMovingAverageParams));
        }

        readonly Foldout m_Foldout;
        VisualElement Content => m_Foldout.contentContainer;

        internal CounterConfigurationInspector(
            SerializedProperty configurationProp,
            PropertyField typeField = null)
        {
            m_Foldout = new Foldout();
            m_Foldout.text = configurationProp.displayName;
            Add(m_Foldout);

            var (smoothingMethodProp, smoothingMethodField) = Content.AddFieldForProperty(configurationProp, k_SmoothingMethodFieldName);

            Content.AddFieldForProperty(configurationProp, k_AggregationMethodFieldName);
            Content.AddFieldForProperty(configurationProp, k_SignificantDigitsFieldName);
            Content.AddFieldForProperty(configurationProp, k_HighlightLowerBoundFieldName);
            Content.AddFieldForProperty(configurationProp, k_HighlightUpperBoundFieldName);

            var (_, emaField) = Content.AddFieldForProperty(configurationProp, k_ExponentialMovingAverageFieldName);
            var (_, smaField) = Content.AddFieldForProperty(configurationProp, k_SimpleMovingAverageFieldName);

            void UpdateFieldVisibility()
            {
                var smoothingMethod = (SmoothingMethod)smoothingMethodProp.enumValueIndex;
                emaField.SetInclude(smoothingMethod == SmoothingMethod.ExponentialMovingAverage);
                smaField.SetInclude(smoothingMethod == SmoothingMethod.SimpleMovingAverage);
            }

            UpdateFieldVisibility();
            smoothingMethodField.RegisterValueChangeCallback(evt =>
            {
                UpdateFieldVisibility();
            });
        }
    }
}