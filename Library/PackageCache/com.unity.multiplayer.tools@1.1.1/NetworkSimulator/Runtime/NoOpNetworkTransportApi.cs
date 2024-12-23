using System;
using Unity.Multiplayer.Tools.Adapters;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Non-operational Transport API used internally only when <see cref="NetworkSimulator" /> is disabled.
    /// </summary>
    class NoOpNetworkTransportApi : INetworkTransportApi, IDisposable
    {
        /// <inheritdoc />
        public void Dispose() => this.NoEffectWarning();

        /// <inheritdoc />
        public bool IsAvailable => this.NoEffectWarning<bool>();

        /// <inheritdoc />
        public bool IsConnected => this.NoEffectWarning<bool>();

        /// <inheritdoc />
        public void SimulateDisconnect() => this.NoEffectWarning();

        /// <inheritdoc />
        public void SimulateReconnect() => this.NoEffectWarning();

        /// <inheritdoc />
        public void UpdateNetworkParameters(NetworkParameters networkParameters) => this.NoEffectWarning();
    }
}