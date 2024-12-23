using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    //
    // Note: Majority of this code implementation comes from UI Toolkit directly.
    //       We should replace these class with the UI Toolkit one when they are released.
    //
    interface IToggleButtonStrip : INotifyValueChanged<string>
    {
        IEnumerable<string> choices { get; set; }
        IEnumerable<string> labels { get; set; }
        Type enumType { get; set; }
    }

    class ToggleButtonStrip : BaseField<string>, IToggleButtonStrip
    {
        static readonly string s_UssPath = "Packages/com.unity.multiplayer.tools/NetworkSimulator/Editor/UI/ToggleButtonStrip/ToggleButtonStrip.uss";
        static readonly string s_UssClassName = "unity-toggle-button-strip";
        ButtonStrip m_ButtonStrip;

        MethodInfo m_IncrementVersionMethod;
        PropertyInfo m_PseudoStatesProperty;
        const int k_InvalidEnumValue = -1;
        int m_PseudoStatesCheckedValue = k_InvalidEnumValue;
        int m_PseudoStatesFocusValue = k_InvalidEnumValue;
        int m_VersionChangeTypeStylesValue = k_InvalidEnumValue;

        new class UxmlFactory : UxmlFactory<ToggleButtonStrip, UxmlTraits> {}
        new class UxmlTraits : BaseField<string>.UxmlTraits
        {
            public UxmlTraits()
            {
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
        public IEnumerable<string> choices
        {
            get { return m_ButtonStrip.choices; }
            set { m_ButtonStrip.choices = value; }
        }

        public IEnumerable<string> labels
        {
            get => choices;
            set => choices = value;
        }

        public Action<EventBase> onButtonClick
        {
            get => m_ButtonStrip.onButtonClick;
            set => m_ButtonStrip.onButtonClick += value;
        }

        public Type enumType { get; set; }

        public ToggleButtonStrip() : this(null, null) {}

        public ToggleButtonStrip(string label, IList<string> choices) : base(label, null)
        {
            AddToClassList(s_UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(s_UssPath));
            m_ButtonStrip = new ButtonStrip();
            m_ButtonStrip.onButtonClick += OnOptionChange;
            Add(m_ButtonStrip);
            this.choices = choices;

            m_IncrementVersionMethod = typeof(VisualElement).GetMethod("IncrementVersion", BindingFlags.Instance | BindingFlags.NonPublic);
            m_PseudoStatesProperty = typeof(VisualElement).GetProperty("pseudoStates", BindingFlags.Instance | BindingFlags.NonPublic);

            if (m_IncrementVersionMethod != null)
            {
                m_VersionChangeTypeStylesValue = GetIncrementVersion(m_IncrementVersionMethod.GetParameters()[0].ParameterType.GetEnumValues());
            }

            if (m_PseudoStatesProperty != null)
            {
                (m_PseudoStatesCheckedValue, m_PseudoStatesFocusValue) = GetPseudoStatesValues(m_PseudoStatesProperty.PropertyType.GetEnumValues());
            }
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            var button = m_ButtonStrip.Q<Button>(newValue);
            if (button == null)
                return;
            base.SetValueWithoutNotify(newValue);
            ToggleButtonStates(button);
        }

        void OnOptionChange(EventBase evt)
        {
            var button = evt.target as Button;
            var newValue = button.name;
            value = newValue;
            ToggleButtonStates(button);
        }

        void ToggleButtonStates(Button button)
        {
            if (m_IncrementVersionMethod == null
                || m_PseudoStatesProperty == null
                || m_VersionChangeTypeStylesValue == k_InvalidEnumValue
                || m_PseudoStatesCheckedValue == k_InvalidEnumValue
                || m_PseudoStatesFocusValue == k_InvalidEnumValue)
            {
                return;
            }

            m_ButtonStrip.Query<Button>().ForEach((b) =>
            {
                var pseudoStates = (int)m_PseudoStatesProperty.GetValue(b);
                pseudoStates &= ~m_PseudoStatesCheckedValue;
                m_PseudoStatesProperty.SetValue(b, pseudoStates);
            });

            var pseudoStates = (int)m_PseudoStatesProperty.GetValue(button);
            pseudoStates |= m_PseudoStatesCheckedValue;
            pseudoStates &= ~m_PseudoStatesFocusValue;
            m_PseudoStatesProperty.SetValue(button, pseudoStates);

            m_IncrementVersionMethod.Invoke(button, new object[] { m_VersionChangeTypeStylesValue });
        }

        (int pseudoStatesCheckedValue, int pseudoStatesFocusValue) GetPseudoStatesValues(Array values)
        {
            var pseudoStatesCheckedValue = k_InvalidEnumValue;
            var pseudoStatesFocusValue = k_InvalidEnumValue;

            foreach (var pseudoState in values)
            {
                if (pseudoState.ToString() == "Checked")
                {
                    pseudoStatesCheckedValue = (int)pseudoState;
                }

                if (pseudoState.ToString() == "Focus")
                {
                    pseudoStatesFocusValue = (int)pseudoState;
                }
            }

            return (pseudoStatesCheckedValue, pseudoStatesFocusValue);
        }

        int GetIncrementVersion(Array values)
        {
            var versionChangeTypeStylesValue = k_InvalidEnumValue;
            foreach (var changeTypeStyle in values)
            {
                if (changeTypeStyle.ToString() == "Styles")
                {
                    versionChangeTypeStylesValue = (int)changeTypeStyle;
                }
            }

            return versionChangeTypeStylesValue;
        }
    }
}