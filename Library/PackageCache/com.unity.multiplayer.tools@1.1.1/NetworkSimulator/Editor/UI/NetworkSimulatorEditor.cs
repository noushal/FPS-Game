using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    [CustomEditor(typeof(Runtime.NetworkSimulator))]
    class NetworkSimulatorEditor : UnityEditor.Editor
    {
        Runtime.NetworkSimulator m_NetworkSimulator;

        VisualElement m_Inspector;

        public override VisualElement CreateInspectorGUI()
        {
            m_NetworkSimulator = (Runtime.NetworkSimulator)target;

            m_Inspector = new VisualElement();
            m_Inspector.Add(new NetworkTypeView(serializedObject, m_NetworkSimulator));
            m_Inspector.Add(new NetworkEventsView(m_NetworkSimulator.NetworkEventsApi));
            m_Inspector.Add(new LagSpikeView(m_NetworkSimulator.NetworkEventsApi));
            m_Inspector.Add(new NetworkScenarioView(serializedObject, m_NetworkSimulator));

            return m_Inspector;
        }
    }
}
