using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

namespace Unity.Multiplayer.Tools.Adapters.Utp2
{
    class Utp2Adapter :
        INetworkAdapter,
        INetworkAvailability,
        ISimulateDisconnectAndReconnect,
        IHandleNetworkParameters
    {
        readonly NetworkDriver m_NetworkDriver;

        NetworkParameters m_CurrentNetworkParameters;
        bool m_IsDisabled;

        public Utp2Adapter(NetworkDriver networkDriver)
        {
            m_NetworkDriver = networkDriver;

            m_CurrentNetworkParameters = new NetworkParameters();
        }

        public AdapterMetadata Metadata { get; } = new AdapterMetadata
        {
            PackageInfo = new PackageInfo
            {
                PackageName = "com.unity.transport",
                Version = new PackageVersion
                {
                    Major = 2,
                    Minor = 0,
                    Patch = 0,
                    PreRelease = ""
                }
            }
        };

        public T GetComponent<T>() where T : class, IAdapterComponent
        {
            return this as T;
        }

        public bool IsConnected => m_NetworkDriver.IsCreated && !m_IsDisabled;

        public NetworkParameters NetworkParameters
        {
            get
            {
                if (m_NetworkDriver.CurrentSettings.TryGet<SimulatorUtility.Parameters>(out var parameters))
                {
                    var networkParameters = new NetworkParameters
                    {
                        PacketDelayMilliseconds = parameters.PacketDelayMs,
                        PacketDelayRangeMilliseconds = parameters.PacketJitterMs,
                        PacketLossIntervalMilliseconds = parameters.PacketDropInterval,
                        PacketLossPercent = parameters.PacketDropPercentage,
                    };

                    m_CurrentNetworkParameters = networkParameters;
                }

                return m_CurrentNetworkParameters;
            }
            set
            {
                if (m_NetworkDriver.CurrentSettings.TryGet<SimulatorUtility.Parameters>(out var parameters))
                {
                    m_CurrentNetworkParameters = value;

                    parameters.PacketDelayMs = m_CurrentNetworkParameters.PacketDelayMilliseconds;
                    parameters.PacketJitterMs = m_CurrentNetworkParameters.PacketDelayRangeMilliseconds;
                    parameters.PacketDropInterval = m_CurrentNetworkParameters.PacketLossIntervalMilliseconds;
                    parameters.PacketDropPercentage = m_CurrentNetworkParameters.PacketLossPercent;

                    m_NetworkDriver.ModifySimulatorStageParameters(parameters);
                }
            }
        }

        public void SimulateDisconnect()
        {
            m_IsDisabled = true;

            m_NetworkDriver.ModifyNetworkSimulatorParameters(new NetworkSimulatorParameter
            {
                ReceivePacketLossPercent = 100.0f,
                SendPacketLossPercent = 100.0f,
            });
        }

        public void SimulateReconnect()
        {
            m_IsDisabled = false;

            m_NetworkDriver.ModifyNetworkSimulatorParameters(new NetworkSimulatorParameter
            {
                ReceivePacketLossPercent = 0.0f,
                SendPacketLossPercent = 0.0f,
            });
        }
    }
}
