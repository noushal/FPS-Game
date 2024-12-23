using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Editor
{
    static class CustomInspectorUtils
    {
        internal static (SerializedProperty, PropertyField) AddFieldForProperty(
            this VisualElement parentVisualElement,
            SerializedProperty parentProperty,
            string fieldName)
        {
            var property = parentProperty.FindPropertyRelative(fieldName);
            var field = new PropertyField(property);
            parentVisualElement.Add(field);
            return (property, field);
        }

        /// <summary>
        /// Extensions method to compensate for the fact that <see cref="SerializedProperty"/> does not provide a
        /// method for accessing the parent property
        /// </summary>
        internal static SerializedProperty GetParent(this SerializedProperty property)
        {
            const char separator = '.';
            var obj = property.serializedObject;
            var pathSplit = property.propertyPath.Split(separator).ToList();
            pathSplit.RemoveAt(pathSplit.Count - 1);
            var parentPath = String.Join(separator, pathSplit);
            return obj.FindProperty(parentPath);
        }
    }
}