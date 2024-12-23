using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime.BuiltInScenarios
{
    /// <summary>
    /// Base class that provides support for Connection Presets, used for Network Scenario Configurations.
    /// </summary>
    [Serializable]
    public class PresetConfiguration
    {
        [SerializeReference]
        internal NetworkSimulatorPreset m_ClassPreset;

        [SerializeField]
        internal NetworkSimulatorPresetAsset m_ScriptableObjectPreset;

        internal bool IsClassPreset => m_ClassPreset != null && !string.IsNullOrEmpty(m_ClassPreset.Name);

        /// <summary>
        /// <inheritdoc cref="NetworkSimulator.ConnectionPreset"/>
        /// </summary>
        public INetworkSimulatorPreset ConnectionPreset
        {
            get => (INetworkSimulatorPreset)m_ScriptableObjectPreset ?? m_ClassPreset;
            set
            {
                if (value is NetworkSimulatorPresetAsset scriptableObject)
                {
                    m_ScriptableObjectPreset = scriptableObject;
                    return;
                }

                if (value is NetworkSimulatorPreset presetObject)
                {
                    m_ClassPreset = presetObject;
                }
            }
        }
    }
}
