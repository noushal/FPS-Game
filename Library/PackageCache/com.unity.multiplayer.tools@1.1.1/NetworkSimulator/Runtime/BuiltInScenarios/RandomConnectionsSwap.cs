using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime.BuiltInScenarios
{
    /// <summary>
    /// <see cref="NetworkScenario"/> that iterates through the <see cref="Configurations"/> list in a random order.
    /// </summary>
    [UsedImplicitly, Serializable]
    public sealed class RandomConnectionsSwap : NetworkScenarioTask
    {
        /// <summary>
        /// Specifies a <see cref="PresetConfiguration.ConnectionPreset"/> that can be selected.
        /// <seealso cref="PresetConfiguration"/>
        /// </summary>
        [Serializable]
        public sealed class Configuration : PresetConfiguration
        {
        }

        /// <summary>
        /// Time in milliseconds to activate the next <see cref="PresetConfiguration.ConnectionPreset"/>.
        /// </summary>
        [SerializeField]
        public int ChangeIntervalMilliseconds = 5_000;

        [SerializeField]
        List<Configuration> m_Configurations = new(new[]
        {
            new Configuration { ConnectionPreset = NetworkSimulatorPresets.None }
        });

        /// <summary>
        /// Defines the available <see cref="PresetConfiguration.ConnectionPreset"/> list.
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

                var index = Random.Range(0, Configurations.Count);

                if (index >= m_Configurations.Count)
                {
                    Debug.LogWarning($"Skipping scenario item #{index} as {nameof(RandomConnectionsSwap)}.{nameof(Configurations)} doesn't have enough elements.");
                    await Task.Yield();
                    continue;
                }

                var connectionType = m_Configurations[index].ConnectionPreset;
                networkEventsApi.ChangeConnectionPreset(connectionType);

                if (ChangeIntervalMilliseconds <= 0)
                {
                    Debug.LogWarning($"Skipping scenario item #{index}. {nameof(RandomConnectionsSwap)}.{ChangeIntervalMilliseconds} parameter must be greater than 0.");
                    await Task.Yield();
                    continue;
                }

                await Task.Delay(ChangeIntervalMilliseconds, cancellationToken);
            }
        }
    }
}
