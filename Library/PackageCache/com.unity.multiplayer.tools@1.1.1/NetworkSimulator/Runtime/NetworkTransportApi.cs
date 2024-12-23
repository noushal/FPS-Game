// NetSim Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if !UNITY_MP_TOOLS_NETSIM_DISABLED && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NETSIM_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NETSIM_ENABLED_IN_RELEASE))
#define UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.Adapters;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    class NetworkTransportApi : INetworkTransportApi, IDisposable
    {
        readonly IList<INetworkAvailability> m_NetworkAvailabilityComponents = new List<INetworkAvailability>();
        readonly IList<ISimulateDisconnectAndReconnect> m_DisconnectAndReconnectComponents = new List<ISimulateDisconnectAndReconnect>();
        readonly IList<IHandleNetworkParameters> m_HandleNetworkParametersComponents = new List<IHandleNetworkParameters>();
        
        NetworkParameters m_NetworkParameters;

        public NetworkTransportApi()
        {
            SubscribeToAllAdapters();
        }

        public void Dispose()
        {
            UnsubscribeFromAllAdapters();
        }

        public bool IsAvailable => m_NetworkAvailabilityComponents.Any() &&
                                   m_DisconnectAndReconnectComponents.Any() &&
                                   m_HandleNetworkParametersComponents.Any();

        public bool IsConnected => IsAvailable && m_NetworkAvailabilityComponents.All(x => x.IsConnected);

        public void SimulateDisconnect()
        {
            foreach (var component in m_DisconnectAndReconnectComponents)
            {
                component.SimulateDisconnect();
            }
        }

        public void SimulateReconnect()
        {
            foreach (var component in m_DisconnectAndReconnectComponents)
            {
                component.SimulateReconnect();
            }
        }

        public void UpdateNetworkParameters(NetworkParameters networkParameters)
        {
            m_NetworkParameters = networkParameters;
            foreach (var component in m_HandleNetworkParametersComponents)
            {
                component.NetworkParameters = networkParameters;
            }
        }

        void SubscribeToAllAdapters()
        {
            foreach (var adapter in NetworkAdapters.Adapters)
            {
                SubscribeToAdapter(adapter);
            }

            NetworkAdapters.OnAdapterAdded += SubscribeToAdapter;
            NetworkAdapters.OnAdapterRemoved += UnsubscribeFromAdapter;
        }

        void UnsubscribeFromAllAdapters()
        {
            foreach (var adapter in NetworkAdapters.Adapters)
            {
                UnsubscribeFromAdapter(adapter);
            }

            NetworkAdapters.OnAdapterAdded -= SubscribeToAdapter;
            NetworkAdapters.OnAdapterRemoved -= UnsubscribeFromAdapter;
        }

        void SubscribeToAdapter(INetworkAdapter adapter)
        {
            var networkAvailability = adapter.GetComponent<INetworkAvailability>();
            if (networkAvailability != null)
            {
                m_NetworkAvailabilityComponents.Add(networkAvailability);
            }

            var disconnectAndReconnect = adapter.GetComponent<ISimulateDisconnectAndReconnect>();
            if (disconnectAndReconnect != null)
            {
                m_DisconnectAndReconnectComponents.Add(disconnectAndReconnect);
            }

            var handleNetworkParameters = adapter.GetComponent<IHandleNetworkParameters>();
            if (handleNetworkParameters != null)
            {
                m_HandleNetworkParametersComponents.Add(handleNetworkParameters);
            }

            if (m_NetworkParameters != null)
            {
                UpdateNetworkParameters(m_NetworkParameters);
            }
        }

        void UnsubscribeFromAdapter(INetworkAdapter adapter)
        {
            var networkAvailability = adapter.GetComponent<INetworkAvailability>();
            if (networkAvailability != null)
            {
                m_NetworkAvailabilityComponents.Remove(networkAvailability);
            }

            var disconnectAndReconnect = adapter.GetComponent<ISimulateDisconnectAndReconnect>();
            if (disconnectAndReconnect != null)
            {
                m_NetworkAvailabilityComponents.Remove(networkAvailability);
            }

            var handleNetworkParameters = adapter.GetComponent<IHandleNetworkParameters>();
            if (handleNetworkParameters != null)
            {
                m_HandleNetworkParametersComponents.Remove(handleNetworkParameters);
            }
        }
    }
}
#endif