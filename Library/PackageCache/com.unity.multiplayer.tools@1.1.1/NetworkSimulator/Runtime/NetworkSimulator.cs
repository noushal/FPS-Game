// NetSim Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------

#if !UNITY_MP_TOOLS_NETSIM_DISABLED && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NETSIM_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NETSIM_ENABLED_IN_RELEASE))
#define UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Runtime
{
    /// <summary>
    /// Add this component to any game object to configure network simulation parameters.
    /// </summary>
    public partial class NetworkSimulator : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField]
        internal NetworkSimulatorPresetAsset m_PresetAsset;

        [SerializeReference, HideInInspector]
        internal INetworkSimulatorPreset m_PresetReference = new NetworkSimulatorPreset();

        [SerializeReference]
        internal NetworkScenario m_Scenario;

        [SerializeField, HideInInspector]
        internal bool m_IsScenarioSettingsFolded;

        /// <summary>
        /// Allows to determine if network scenarios should start automatically or not.
        /// </summary>
        [SerializeField]
        public bool AutoRunScenario;

#if UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED
        readonly INetworkTransportApi m_NetworkTransportApi = new NetworkTransportApi();
#else
        readonly INetworkTransportApi m_NetworkTransportApi = new NoOpNetworkTransportApi();
#endif
        INetworkEventsApi m_NetworkEventsApi;
        INetworkSimulatorPreset m_CachedPreset;
        bool m_CachedScenarioIsPaused;

        internal PropertyChangedEventHandler m_PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => m_PropertyChanged += value;
            remove => m_PropertyChanged -= value;
        }

        internal INetworkEventsApi NetworkEventsApi
        {
            get
            {
                if (m_NetworkEventsApi != null)
                {
                    return m_NetworkEventsApi;
                }
#if UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED
                m_NetworkEventsApi = new NetworkEventsApi(this, m_NetworkTransportApi);
#else
                m_NetworkEventsApi = new NoOpNetworkEventsApi();
#endif
                return m_NetworkEventsApi;
            }
        }

        /// <summary>
        /// The Connection Preset used to define a set of connection parameters to simulate the network condition at runtime.
        /// </summary>
        public INetworkSimulatorPreset ConnectionPreset
        {
            get => m_PresetAsset != null
                ? m_PresetAsset
                : m_PresetReference;
            set
            {
                if (value is NetworkSimulatorPresetAsset presetAsset)
                {
                    m_PresetAsset = presetAsset;
                    m_PresetReference = null;
                }
                else
                {
                    m_PresetReference = value;
                    m_PresetAsset = null;
                }

                OnConnectionPresetChanged();
            }
        }

        /// <summary>
        /// The Network Scenario used to modify network connection parameters at runtime.
        /// </summary>
        public NetworkScenario Scenario
        {
            get => m_Scenario;
            set
            {
                var previousValue = Scenario;
                m_Scenario = value;
                OnScenarioChanged(previousValue, value);
            }
        }

        internal void UpdateLiveParameters(bool forceUpdate = false)
        {
            if (forceUpdate == false && enabled == false)
            {
                return;
            }

            m_NetworkTransportApi.UpdateNetworkParameters(
                new()
                {
                    PacketDelayMilliseconds = ConnectionPreset?.PacketDelayMs ?? 0,
                    PacketDelayRangeMilliseconds = ConnectionPreset?.PacketJitterMs ?? 0,
                    PacketLossIntervalMilliseconds = ConnectionPreset?.PacketLossInterval ?? 0,
                    PacketLossPercent = ConnectionPreset?.PacketLossPercent ?? 0
                });
        }

        void OnEnable()
        {
            if (m_CachedPreset != null)
            {
                ConnectionPreset = m_CachedPreset;
            }

            if (Scenario != null)
            {
                Scenario.IsPaused = m_CachedScenarioIsPaused;
            }

            UpdateLiveParameters();
        }

        void Start()
        {
            Scenario?.InitializeScenario(NetworkEventsApi, AutoRunScenario);
        }

        void OnDisable()
        {
            m_CachedPreset = ConnectionPreset;
            ConnectionPreset = NetworkSimulatorPresets.None;
            UpdateLiveParameters(true);
        }

        void OnDestroy()
        {
            Scenario?.Dispose();
        }

        void Update()
        {
            if (Scenario is NetworkScenarioBehaviour scenarioBehaviour)
            {
                scenarioBehaviour.UpdateScenario(Time.deltaTime);
            }
        }

        void OnConnectionPresetChanged()
        {
            UpdateLiveParameters();
            OnPropertyChanged(nameof(ConnectionPreset));
        }

        void OnScenarioChanged([CanBeNull] NetworkScenario previousValue, [CanBeNull] NetworkScenario newValue)
        {
            if (Application.isPlaying && Equals(previousValue, newValue) == false)
            {
                previousValue?.Dispose();

                if (newValue == null)
                {
                    return;
                }

                var shouldAutoRun = AutoRunScenario && previousValue != null && !previousValue.IsPaused;
                newValue.InitializeScenario(NetworkEventsApi, shouldAutoRun);
            }

            OnPropertyChanged(nameof(Scenario));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            m_PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
