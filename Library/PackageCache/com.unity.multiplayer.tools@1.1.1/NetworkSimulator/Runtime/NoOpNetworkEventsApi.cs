using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Non-operational API used internally only when <see cref="NetworkSimulator" /> is disabled.
    /// </summary>
    class NoOpNetworkEventsApi : INetworkEventsApi
    {
        /// <inheritdoc />
        public bool IsAvailable => this.NoEffectWarning<bool>();

        /// <inheritdoc />
        public bool IsConnected => this.NoEffectWarning<bool>();

        /// <inheritdoc />
        public void Disconnect() => this.NoEffectWarning();

        /// <inheritdoc />
        public void Reconnect() => this.NoEffectWarning();

        /// <inheritdoc />
        public void TriggerLagSpike(TimeSpan duration) => this.NoEffectWarning();

        /// <inheritdoc />
        public Task TriggerLagSpikeAsync(TimeSpan duration) => this.NoEffectWarning<Task>();

        /// <inheritdoc />
        public void ChangeConnectionPreset(INetworkSimulatorPreset preset) => this.NoEffectWarning();

        /// <inheritdoc />
        public INetworkSimulatorPreset CurrentPreset  => this.NoEffectWarning<INetworkSimulatorPreset>();
    }
}
