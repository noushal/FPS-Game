using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// The NetStatsMonitorConfiguration includes all fields required to
    /// configure the contents of the RuntimeNetStatsMonitor
    /// </summary>
    [CreateAssetMenu(
        fileName = "NetStatsMonitorConfiguration",
        menuName = "Multiplayer/NetStatsMonitorConfiguration",
        order = 900)]
    public class NetStatsMonitorConfiguration : ScriptableObject
    {
        /// <summary>
        /// List of elements to be rendered by the <see cref="RuntimeNetStatsMonitor"/>.
        /// </summary>
        [field: SerializeField]
        public List<DisplayElementConfiguration> DisplayElements { get; set; } = new();

        [field: SerializeField]
        internal int? ConfigurationHash { get; private set; } = null;

        /// <summary>
        /// Force a configuration reload.
        /// This needs to be called if the configuration has been modified at runtime
        /// by a script.
        /// </summary>
        public void OnConfigurationModified()
        {
            RecomputeConfigurationHash();
        }

        internal void OnValidate()
        {
            for (var i = 0; i < DisplayElements.Count; ++i)
            {
                if (!DisplayElements[i].FieldsInitialized)
                {
                    DisplayElements[i] = new DisplayElementConfiguration();
                }
                else
                {
                    DisplayElements[i].OnValidate();
                }
            }
            RecomputeConfigurationHash();
        }

        /// Re-computes the configuration hash and stores it in the ConfigurationHash field
        internal void RecomputeConfigurationHash()
        {
            int hashCode = 0;
            foreach (var displayElementConfiguration in DisplayElements)
            {
                hashCode = HashCode.Combine(hashCode, displayElementConfiguration.ComputeHashCode());
            }
            ConfigurationHash = hashCode;
        }
    }
}