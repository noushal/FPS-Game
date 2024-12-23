using System;
using System.Linq;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime.BuiltInScenarios;
using UnityEditor;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RandomConnectionsSwap))]
    class RandomConnectionsSwapPropertyDrawer : PropertyDrawer
    {
        static string[] s_Presets;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            var changeInterval = property.FindPropertyRelative(nameof(RandomConnectionsSwap.ChangeIntervalMilliseconds));
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, changeInterval, new(ObjectNames.NicifyVariableName(nameof(RandomConnectionsSwap.ChangeIntervalMilliseconds))), true);

            var connections = property.FindPropertyRelative($"m_{nameof(RandomConnectionsSwap.Configurations)}");
            EditorGUILayout.PropertyField(connections, true);
            
            if (EditorGUI.EndChangeCheck())
            {
                changeInterval.intValue = Mathf.Clamp(changeInterval.intValue, 0, int.MaxValue);
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(RandomConnectionsSwap.Configuration))]
    class RandomConnectionsSwapPresetItemPropertyDrawer : PropertyDrawer
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

            var connectionType = property.FindPropertyRelative(nameof(RandomConnectionsSwap.Configuration.m_ClassPreset));
            var customPresetProperty = property.FindPropertyRelative(nameof(RandomConnectionsSwap.Configuration.m_ScriptableObjectPreset));
            var customPresetIndex = s_Presets.Length - 1;
            var dropdownIndex = customPresetIndex;

            if (connectionType.managedReferenceValue is NetworkSimulatorPreset classPreset && !string.IsNullOrWhiteSpace(classPreset.Name))
            {
                dropdownIndex = Array.IndexOf(s_Presets, classPreset.Name);
            }

            position.height = EditorGUIUtility.singleLineHeight;

            var labelName = ObjectNames.NicifyVariableName(nameof(RandomConnectionsSwap.Configuration.m_ClassPreset));
            var newIndex = EditorGUI.Popup(position, labelName, dropdownIndex, s_Presets);

            if (newIndex != dropdownIndex && newIndex == customPresetIndex)
            {
                // boxedValue can't be set to null if it's not an Unity Object.
                connectionType.boxedValue = new NetworkSimulatorPreset();
            }
            else if (newIndex != dropdownIndex && newIndex < customPresetIndex)
            {
                var newValue = s_Presets[newIndex];
                connectionType.boxedValue = NetworkSimulatorPresets.Values.First(x => x.Name == newValue);
                customPresetProperty.boxedValue = null;
            }

            position.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.ObjectField(position, customPresetProperty);

            if (customPresetProperty.boxedValue is INetworkSimulatorPreset && connectionType.boxedValue != EmptyPreset)
            {
                connectionType.boxedValue = EmptyPreset;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
