using System;
using System.Linq;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime.BuiltInScenarios;
using UnityEditor;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ConnectionsCycle.Configuration))]
    class ConnectionsCycleSwitchPresetItemPropertyDrawer : PropertyDrawer
    {
        static readonly NetworkSimulatorPreset EmptyPreset = new();
        static string[] s_Presets;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            s_Presets ??= NetworkSimulatorPresets.Values
                .Select(preset => preset.Name)
                .Concat(new []{"Custom"})
                .ToArray();

            var connectionType = property.FindPropertyRelative(nameof(ConnectionsCycle.Configuration.m_ClassPreset));
            var customPresetProperty = property.FindPropertyRelative(nameof(ConnectionsCycle.Configuration.m_ScriptableObjectPreset));
            var changeIntervalProperty = property.FindPropertyRelative(nameof(ConnectionsCycle.Configuration.ChangeIntervalMilliseconds));
            var customPresetIndex = s_Presets.Length - 1;
            var dropdownIndex = customPresetIndex;

            if (connectionType.managedReferenceValue is NetworkSimulatorPreset classPreset && !string.IsNullOrWhiteSpace(classPreset.Name))
            {
                dropdownIndex = Array.IndexOf(s_Presets, classPreset.Name);
            }

            position.height = EditorGUIUtility.singleLineHeight;

            var labelName = ObjectNames.NicifyVariableName(nameof(ConnectionsCycle.Configuration.ConnectionPreset));
            var newIndex = EditorGUI.Popup(position, labelName, dropdownIndex, s_Presets);

            if (newIndex != dropdownIndex && newIndex == customPresetIndex)
            {
                // boxedValue can't be set to null if it's not an Unity Object.
                connectionType.boxedValue = new NetworkSimulatorPreset();
            }
            else if (newIndex != dropdownIndex && newIndex < customPresetIndex)
            {
                var newValue = s_Presets[newIndex];
                connectionType.managedReferenceValue = NetworkSimulatorPresets.Values.First(x => x.Name == newValue);
                customPresetProperty.boxedValue = null;
            }

            position.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.ObjectField(position, customPresetProperty);
            
            position.y += EditorGUIUtility.singleLineHeight;
            labelName = ObjectNames.NicifyVariableName(nameof(ConnectionsCycle.Configuration.ChangeIntervalMilliseconds));
            var guiContent = new GUIContent(labelName);
            EditorGUI.PropertyField(position, changeIntervalProperty, guiContent);

            if (customPresetProperty.boxedValue is INetworkSimulatorPreset && connectionType.boxedValue != EmptyPreset)
            {
                connectionType.boxedValue = EmptyPreset;
            }

            if (EditorGUI.EndChangeCheck())
            {
                changeIntervalProperty.intValue = Mathf.Clamp(changeIntervalProperty.intValue, 0, int.MaxValue);
                connectionType.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
