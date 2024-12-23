using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.Common
{
    static class VisualElementExtensions
    {
        public static void AddEventLifecycle(
            this VisualElement visualElement,
            EventCallback<AttachToPanelEvent> onAttach,
            EventCallback<DetachFromPanelEvent> onDetach)
        {
            visualElement.RegisterCallback(onAttach);
            visualElement.RegisterCallback(onDetach);
        }

        /// <summary>
        /// This differs from VisualElement.Visible, because a VisualElement that is
        /// not visible still takes up space in the layout, whereas a VisualElement
        /// that is not included is skipped and does not take up space.
        /// </summary>
        /// <remarks>
        /// This is syntactic sugar for setting the display style to either Flex or None
        /// </remarks>
        public static void SetInclude(this VisualElement visualElement, bool includeInLayout)
        {
            visualElement.style.display = includeInLayout
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        /// <summary>
        /// This differs from VisualElement.Visible, because a VisualElement that is
        /// not visible still takes up space in the layout, whereas a VisualElement
        /// that is not included is skipped and does not take up space.
        /// </summary>
        /// <remarks>
        /// This is syntactic sugar for getting the display style (either Flex or None)
        /// </remarks>
        public static bool GetInclude(this VisualElement visualElement) =>
            visualElement.style.display.value == DisplayStyle.Flex;
    }
}
