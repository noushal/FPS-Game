// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------


using JetBrains.Annotations;
using System;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
using Unity.Multiplayer.Tools.NetStatsMonitor.Implementation;
#endif

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
    /// <summary>
    /// The Runtime Net Stats Monitor component.
    /// Add this component to a game object in a scene to display network statistics on screen.
    /// </summary>
    [AddComponentMenu("Netcode/" + nameof(RuntimeNetStatsMonitor), 1000)]
    public class RuntimeNetStatsMonitor : MonoBehaviour
    {
        /// Visibility toggle to hide or show the on-screen display.
        [SerializeField]
        [Tooltip("Visibility toggle to hide or show the on-screen display.")]
        bool m_Visible = true;

        /// <summary>
        /// Visibility toggle to hide or show the on-screen display.
        /// </summary>
        public bool Visible
        {
            get => m_Visible;
            set
            {
                m_Visible = value;
#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
                UpdateUiVisibility();
#endif
            }
        }

        /// <summary>
        /// The maximum rate at which the Runtime Net Stats Monitor's on-screen display is updated (per second).
        /// The on-screen display will never be updated faster than the overall refresh rate.
        /// The default refresh rate is 30fps.
        /// </summary>
        public double MaxRefreshRate
        {
            get => m_MaxRefreshRate;
            set => m_MaxRefreshRate = Math.Max(value, ConfigurationLimits.k_RefreshRateMin);
        }

        /// The maximum rate at which the Runtime Net Stats Monitor's on-screen display is updated (per second).
        /// The on-screen display will never be updated faster than the overall refresh rate.
        /// The default refresh rate is 30fps.
        [SerializeField]
        [Min((float)ConfigurationLimits.k_RefreshRateMin)]
        [Tooltip("The maximum rate at which the Runtime Net Stats Monitor's on-screen display is updated " +
            "(per second). " +
            "The on-screen display will never be updated faster than the overall refresh rate.")]
        double m_MaxRefreshRate = 30;

        /// <summary>
        /// Custom stylesheet to override the default style of the Runtime Net Stats Monitor.
        /// </summary>
        [field:SerializeField]
        public StyleSheet CustomStyleSheet { get; set; }

        /// <summary>
        /// Optional panel settings that can be used to override the default.
        /// These panel settings can be used to control a number of things, including how the on-screen display
        /// of the Runtime Net Stats Monitor scales on different devices and displays.
        /// </summary>
        [field:Tooltip(
            "Optional panel settings that can be used to override the default. " +
            "These panel settings can be used to control a number of things, including how the on-screen display " +
            "of the Runtime Net Stats Monitor scales on different devices and displays. ")]
        [field:SerializeField]
        public PanelSettings PanelSettingsOverride { get; set; }

        /// <summary>
        /// Position configuration that allows custom positioning on screen
        /// The default position is the top left corner of the screen
        /// </summary>
        [field: SerializeField]
        public PositionConfiguration Position { get; set; } = new();

        /// <summary>
        /// The configuration asset used to configure the information displayed in this Runtime Net Stats Monitor.
        /// The NetStatsMonitorConfiguration can created from the Create menu, or from C# using
        /// ScriptableObject.CreateInstance.
        /// </summary>
        [CanBeNull]
        [field: SerializeField]
        [field: Tooltip(
            "The configuration asset used to configure the information displayed in this Runtime Net Stats Monitor. " +
            "The NetStatsMonitorConfiguration can created from the Create menu, or from C# using " +
            "ScriptableObject.CreateInstance."
        )]
        public NetStatsMonitorConfiguration Configuration { get; set; }

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
        [CanBeNull]
        internal RnsmComponentImplementation Implementation { get; private set; }
#endif

        void Start()
        {
            Setup();
        }

        void OnEnable()
        {
            Setup();
        }

        void OnDisable()
        {
            Teardown();
        }

        void OnDestroy()
        {
            Teardown();
        }

        void OnValidate()
        {
            if (enabled)
            {
                ApplyConfiguration();
            }
            else
            {
                Teardown();
            }
        }

        /// Perform any remaining setup steps and setup any missing fields, components, or events,
        /// but do not overwrite fields or components that are already set up
        internal void Setup()
        {
#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
            SetupImplementation();
            UpdateUiVisibility();
#endif
        }

        /// Teardown any fields/components that are not needed while this component is disabled
        /// to reduce resource usage
        internal void Teardown()
        {
#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
            PerformRemainingImplementationTeardownSteps();
#endif
        }

        /// <summary>
        /// Apply the CustomStyleSheet, Position, and Configuration to the monitor.
        /// This function must be called when these fields have been modified from C#
        /// in order to apply the changes. This function does not need to be called
        /// when these fields are modified in the inspector, as changes made in the
        /// inspector are detected and applied automatically
        /// </summary>
        public void ApplyConfiguration()
        {
            if (Configuration != null)
            {
                Configuration.RecomputeConfigurationHash();
            }
#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
            ConfigureImplementation();
            UpdateUiVisibility();
#endif
        }

        /// <summary>
        /// Add a custom value for this metricId, which can be displayed in the
        /// RuntimeNetStatsMonitor using a counter or graph configured to display
        /// this metric.
        /// </summary>
        /// <param name="metricId">The custom <see cref="MetricId"/> to provide a value for.</param>
        /// <param name="value">The value of the metric.</param>
        public void AddCustomValue(MetricId metricId, float value)
        {
#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
            Implementation?.AddCustomValue(metricId, value);
#endif
        }

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
        void UpdateUiVisibility()
        {
            Implementation?.UpdateUiVisibility(enabled, m_Visible);
        }

        void SetupImplementation()
        {
            Implementation ??= new RnsmComponentImplementation();
            Implementation?.SetupAndConfigure(Configuration, Position, CustomStyleSheet, PanelSettingsOverride, MaxRefreshRate);
        }

        void ConfigureImplementation()
        {
            Implementation?.Configure(Configuration, Position, CustomStyleSheet, PanelSettingsOverride);
        }

        void PerformRemainingImplementationTeardownSteps()
        {
            Implementation?.Teardown();
            Implementation = null;
        }

        internal void Update()
        {
            if (!Visible)
            {
                return;
            }
            Implementation?.Update(Configuration, MaxRefreshRate);
        }
#endif // UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
    }
}
