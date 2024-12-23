using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// <para>Attribute used to make a <see cref="Vector2"/> field act as a range between <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.</para>
    /// <remarks>When this attribute is used, the <see cref="Vector2"/> will be shown as a <see cref="UnityEditor.EditorGUI.MinMaxSlider(UnityEngine.GUIContent,UnityEngine.Rect,ref float,ref float,float,float)">
    /// MinMaxSlider</see> in the Inspector instead of the default Vector2 field.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        /// <summary>
        /// The minimum allowed value for <see cref="Vector2.x"/>.
        /// </summary>
        public readonly float Min;
        
        /// <summary>
        /// The maximum allowed value for <see cref="Vector2.y"/>.
        /// </summary>
        public readonly float Max;
        
        /// <summary>
        /// Whether it should round <see cref="Vector2.x"/> and <see cref="Vector2.y"/> to integers.
        /// </summary>
        public readonly bool RoundToInt;
        
        /// <summary>
        /// <para>Attribute used to make a <see cref="Vector2"/> field act as a range between <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.</para>
        /// <remarks>When this attribute is used, the <see cref="Vector2"/> will be shown as a <see cref="UnityEditor.EditorGUI.MinMaxSlider(UnityEngine.GUIContent,UnityEngine.Rect,ref float,ref float,float,float)">
        /// MinMaxSlider</see> in the Inspector instead of the default Vector2 field.</remarks>
        /// </summary>
        /// <param name="min">The minimum allowed value for <see cref="Vector2.x"/>.</param>
        /// <param name="max">The maximum allowed value for <see cref="Vector2.y"/>.</param>
        /// <param name="roundToInt">Whether it should round <see cref="Vector2.x"/> and <see cref="Vector2.y"/> to integers.</param>
        public MinMaxRangeAttribute(float min, float max, bool roundToInt = false)
        {
            Min = min;
            Max = max;
            RoundToInt = roundToInt;
        }
    }
}
