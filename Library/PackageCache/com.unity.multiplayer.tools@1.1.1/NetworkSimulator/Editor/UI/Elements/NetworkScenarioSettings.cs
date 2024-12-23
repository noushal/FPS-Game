using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    class NetworkScenarioSettings : VisualElement
    {
        // Expose class to UXML:
        public new class UxmlFactory : UxmlFactory<NetworkScenarioSettings, UxmlTraits> { }

        readonly IMGUIContainer m_ScenarioSettings;

        Runtime.NetworkSimulator m_NetworkSimulator;
        SerializedProperty m_NetworkScenarioProperty;
        SerializedProperty m_SettingsFolded;

        public NetworkScenarioSettings()
        {
            // Using IMGUIContainer since the PropertyField doesn't properly support SerializedReferences until Unity 2022.1
            // Source: https://forum.unity.com/threads/propertyfields-dont-work-properly-with-serializereference-fields.796725/
            // We'll keep IMGUI for now as there's a crash with UI Toolkit version, once this is fixed we'll remove it.
            // More info: https://jira.unity3d.com/browse/MTT-4650
            m_ScenarioSettings = new IMGUIContainer(OnScenarioParametersGUIHandler);
        }

        public void BindProperty(SerializedObject serializedObject, Runtime.NetworkSimulator networkSimulator)
        {
            m_NetworkSimulator = networkSimulator;
            m_NetworkScenarioProperty = serializedObject.FindProperty(nameof(networkSimulator.m_Scenario));
            m_SettingsFolded = serializedObject.FindProperty(nameof(networkSimulator.m_IsScenarioSettingsFolded));

            if (m_NetworkSimulator.Scenario != null)
            {
                Show();
            }
        }

        void OnScenarioParametersGUIHandler()
        {
            m_NetworkScenarioProperty.serializedObject.Update();

            m_NetworkScenarioProperty.isExpanded = m_SettingsFolded.boolValue;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_NetworkScenarioProperty, true);

            if (EditorGUI.EndChangeCheck() == false)
            {
                return;
            }

            m_SettingsFolded.boolValue = m_NetworkScenarioProperty.isExpanded;
            m_NetworkScenarioProperty.serializedObject.ApplyModifiedProperties();
        }

        internal void DeselectScenario()
        {
            m_ScenarioSettings.RemoveFromHierarchy();
            m_NetworkSimulator.Scenario = null;
            Hide();
        }

        internal void SelectScenario(NetworkScenario scenario)
        {
            m_NetworkScenarioProperty.managedReferenceValue = scenario;
            m_NetworkScenarioProperty.serializedObject.ApplyModifiedProperties();
            Show();
        }

        void Show()
        {
            if (m_ScenarioSettings.parent != this)
            {
                Add(m_ScenarioSettings);
            }

            m_ScenarioSettings.style.display = DisplayStyle.Flex;
        }

        void Hide()
        {
            if (m_ScenarioSettings.parent == this)
            {
                m_ScenarioSettings.RemoveFromHierarchy();
            }

            m_ScenarioSettings.style.display = DisplayStyle.None;
        }
    }
}
