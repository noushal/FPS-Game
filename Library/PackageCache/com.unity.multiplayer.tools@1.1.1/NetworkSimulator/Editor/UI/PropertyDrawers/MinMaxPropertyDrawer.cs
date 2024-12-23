using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEditor;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    class MinMaxDrawer : PropertyDrawer
    {
        const float k_HorizontalSpace = 5f;
        
        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var xProperty = property.FindPropertyRelative(nameof(Vector2.x));
            var yProperty = property.FindPropertyRelative(nameof(Vector2.y));
            var attr = attribute as MinMaxRangeAttribute ?? new MinMaxRangeAttribute(0, 1);
            var fieldWidth = GUI.skin.textField.CalcSize(new(attr.Max.ToString())).x + k_HorizontalSpace;

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var minValue = attr.RoundToInt ? Mathf.RoundToInt(xProperty.floatValue) : xProperty.floatValue;
            var maxValue = attr.RoundToInt ? Mathf.RoundToInt(yProperty.floatValue) : yProperty.floatValue;
            
            var left = new Rect(position.x, position.y, fieldWidth, position.height);
            var right = new Rect(position.x + position.width - left.width, position.y, fieldWidth, position.height);
            
            minValue = Mathf.Clamp(EditorGUI.FloatField(left, GUIContent.none, minValue), attr.Min, maxValue);
            maxValue = Mathf.Clamp(EditorGUI.FloatField(right, GUIContent.none, maxValue), minValue, attr.Max);

            position.x += fieldWidth + k_HorizontalSpace;
            position.width -= (fieldWidth + k_HorizontalSpace) * 2;
            EditorGUI.MinMaxSlider(position, GUIContent.none, ref minValue, ref maxValue, attr.Min, attr.Max);

            xProperty.floatValue = attr.RoundToInt ? Mathf.RoundToInt(minValue) : minValue;
            yProperty.floatValue = attr.RoundToInt ? Mathf.RoundToInt(maxValue) : maxValue;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
