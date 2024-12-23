using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEngine;

namespace Samples.NetworkScenarios
{
    public class LagSpikeScenarioWithAsyncTask : NetworkScenarioTask
    {
        [SerializeField]
        int m_DurationBetweenLagSpikesMilliseconds;

        [SerializeField]
        int m_LagSpikeDurationMilliseconds;

        protected override async Task Run(INetworkEventsApi networkEventsApi, CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(m_DurationBetweenLagSpikesMilliseconds), cancellationToken);

                // Make sure to check if the user paused the scenario. This isn't pausing all waiting behavior
                // but will skip the lag spike, which is essentially the behavior we want to avoid when paused.
                if (IsPaused)
                {
                    await Task.Yield();
                    continue;
                }

                await networkEventsApi.TriggerLagSpikeAsync(TimeSpan.FromMilliseconds(m_LagSpikeDurationMilliseconds));
            }
        }
    }
}