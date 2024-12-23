using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// An API to trigger network simulation events.
    /// </summary>
    class NetworkEventsApi : INetworkEventsApi
    {
        readonly NetworkSimulator m_NetworkSimulator;
        readonly INetworkTransportApi m_NetworkTransportApi;

        internal NetworkEventsApi(NetworkSimulator networkSimulator, INetworkTransportApi networkTransportApi)
        {
            m_NetworkSimulator = networkSimulator;
            m_NetworkTransportApi = networkTransportApi;
        }

        public bool IsAvailable => m_NetworkTransportApi.IsAvailable;

        public bool IsConnected => m_NetworkTransportApi.IsConnected;

        public void Disconnect()
        {
            if (m_NetworkSimulator.enabled == false)
            {
                return;
            }

            m_NetworkTransportApi.SimulateDisconnect();
        }

        public void Reconnect()
        {
            if (m_NetworkSimulator.enabled == false)
            {
                return;
            }

            m_NetworkTransportApi.SimulateReconnect();
        }

        public void TriggerLagSpike(TimeSpan duration)
        {
            if (m_NetworkSimulator.enabled == false)
            {
                return;
            }

            RunLagSpikeAsync(duration).Forget();
        }

        public Task TriggerLagSpikeAsync(TimeSpan duration)
        {
            if (m_NetworkSimulator.enabled == false)
            {
                return Task.CompletedTask;
            }

            return RunLagSpikeAsync(duration);
        }

        public void ChangeConnectionPreset(INetworkSimulatorPreset newNetworkSimulatorPreset)
        {
            if (m_NetworkSimulator.enabled == false)
            {
                return;
            }

            m_NetworkSimulator.ConnectionPreset = newNetworkSimulatorPreset;
        }

        public INetworkSimulatorPreset CurrentPreset => m_NetworkSimulator.ConnectionPreset;

        async Task RunLagSpikeAsync(TimeSpan duration)
        {
            Disconnect();

            await Task.Delay(duration);

            Reconnect();
        }
    }
}
