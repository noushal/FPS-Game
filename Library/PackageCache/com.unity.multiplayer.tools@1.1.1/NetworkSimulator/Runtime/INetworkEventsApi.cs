using System;
using System.Threading.Tasks;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// API that can be used to inspect the state of the simulated network and trigger events.
    /// </summary>
    public interface INetworkEventsApi
    {
        /// <summary>
        /// Returns whether the Network Simulator fulfills all required dependencies and is available to be used.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Returns true when Network Simulator is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Simulates a network disconnection.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Reconnects after simulating a network disconnection.
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Simulates a lag spike for the specified duration.
        /// </summary>
        /// <param name="duration">The duration for which the lag spike shall last.</param>
        void TriggerLagSpike(TimeSpan duration);

        /// <summary>
        /// Simulates a lag spike for the specified duration.
        /// </summary>
        /// <param name="duration">The duration for which the lag spike shall last.</param>
        /// <returns>The task that runs for the duration of the lag spike.</returns>
        Task TriggerLagSpikeAsync(TimeSpan duration);

        /// <summary>
        /// Changes the current connection preset used to simulate network condition parameters.
        /// </summary>
        /// <param name="preset">The Network Simulator Preset being set.</param>
        void ChangeConnectionPreset(INetworkSimulatorPreset preset);

        /// <summary>
        /// Returns the current connection preset used to simulate network conditions.
        /// </summary>
        INetworkSimulatorPreset CurrentPreset { get; }
    }
}
