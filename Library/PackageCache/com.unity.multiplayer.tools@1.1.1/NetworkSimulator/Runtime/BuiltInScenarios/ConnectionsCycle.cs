using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime.BuiltInScenarios
{
    /// <summary>
    /// <see cref="NetworkScenario"/> that iterates through the <see cref="Configurations"/> list by order.
    /// </summary>
    [UsedImplicitly, Serializable]
    public sealed class ConnectionsCycle : NetworkScenarioTask
    {
        /// <summary>
        /// Specifies the <see cref="PresetConfiguration.ConnectionPreset"/> and the <see cref="ChangeIntervalMilliseconds"/> configuration.
        /// <seealso cref="PresetConfiguration"/>
        /// </summary>
        [Serializable]
        public sealed class Configuration : PresetConfiguration
        {
            /// <summary>
            /// Time in milliseconds to activate the next <see cref="PresetConfiguration.ConnectionPreset"/>.
            /// </summary>
            [SerializeField]
            public int ChangeIntervalMilliseconds = 5_000;
        }

        int m_Index;

        [SerializeField]
        List<Configuration> m_Configurations = new(new[]
        {
            new Configuration { ConnectionPreset = NetworkSimulatorPresets.None }
        });

        /// <summary>
        /// The list of configuration used to define <see cref="PresetConfiguration.ConnectionPreset"/> and the <see cref="Configuration.ChangeIntervalMilliseconds"/>.
        /// <seealso cref="Configuration"/>
        /// </summary>
        public ICollection<Configuration> Configurations => m_Configurations;

        protected override async Task Run(INetworkEventsApi networkEventsApi, CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                if (IsPaused)
                {
                    await Task.Yield();
                    continue;
                }

                if (m_Index >= m_Configurations.Count)
                {
                    Debug.LogWarning($"Skipping scenario item #{m_Index} as {nameof(ConnectionsCycle)}.{nameof(Configurations)} doesn't have enough elements.");
                    await Task.Yield();
                    Iterate();
                    continue;
                }

                var config = m_Configurations[m_Index];
                var preset = config.ConnectionPreset;

                networkEventsApi.ChangeConnectionPreset(preset);

                if (config.ChangeIntervalMilliseconds <= 0)
                {
                    Debug.LogWarning($"Skipping scenario item #{m_Index}. {nameof(ConnectionsCycle)}.{nameof(Configurations)}[{m_Index}].{config.ChangeIntervalMilliseconds} must be greater than 0.");
                    await Task.Yield();
                    Iterate();
                    continue;
                }

                await Task.Delay(config.ChangeIntervalMilliseconds, cancellationToken);

                Iterate();
            }
        }
        void Iterate() => m_Index = ++m_Index >= m_Configurations.Count ? 0 : m_Index;
    }
}
