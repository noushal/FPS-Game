// RNSM Implementation compilation boilerplate
// All references to UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED should be defined in the same way,
// as any discrepancies are likely to result in build failures
// ---------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR || ((DEVELOPMENT_BUILD && !UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP) || (!DEVELOPMENT_BUILD && UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE))
    #define UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED
#endif
// ---------------------------------------------------------------------------------------------------------------------

#if UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED

using System;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.Adapters;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Implementation
{
    class RnsmComponentImplementation
    {
        /// The amount of time in seconds to wait before displaying a network timeout message when no
        /// data is received. A delay of 0 can be used to suppress this message.
        const double k_NoDataReceivedMessageDelaySeconds = 1;

        const double k_MinCollectionInterval_PerFrame = 5e-3;
        const double k_MinCollectionInterval_PerSecond = 1;
        static double MinCollectionInterval(SampleRate rate)
        {
            switch (rate)
            {
                case SampleRate.PerFrame:
                    return k_MinCollectionInterval_PerFrame;
                case SampleRate.PerSecond:
                    return k_MinCollectionInterval_PerSecond;
                default:
                    throw new ArgumentOutOfRangeException($"Unhandled {nameof(SampleRate)} {rate}");
            }
        }

        static readonly PanelSettings k_DefaultPanelSettings;
        static readonly StyleSheet k_DefaultStyleSheet;

        internal event Action OnDisplayUpdate;

        double m_LastDisplayUpdateTime = Constants.k_DefaultTimestamp;

        /// Property to get the current time, which can be mocked for testing purposes
        [NotNull]
        internal Func<double> GetCurrentTime { get; set; } = () => Time.timeAsDouble;

        [NotNull]
        readonly EnumMap<SampleRate, StatsAccumulator> m_Accumulators = new EnumMap<SampleRate, StatsAccumulator>()
        {
            { SampleRate.PerFrame, new StatsAccumulator() },
            { SampleRate.PerSecond, new StatsAccumulator() },
        };

        /// <remarks>
        /// This data is overwritten each frame in update. The only reason it's a member variable
        /// and not a local variable is to avoid reallocating space for it each frame
        /// </remarks>>
        readonly EnumMap<SampleRate, bool> m_NewDataAvailable = new EnumMap<SampleRate, bool>(false);

        [NotNull]
        readonly MultiStatHistory m_MultiStatHistory = new();

        [CanBeNull]
        StyleSheet m_CustomStyleSheet;

        /// Previous hash of all configuration fields
        int? m_PreviousConfigurationHash;

        /// Previous hash of all configuration fields that could impact history requirements
        int? m_PreviousHistoryRequirementsHash;

        internal UIDocument UiDoc { get; private set; }

        [NotNull]
        internal RnsmVisualElement RnsmVisualElement { get; } = new();

        static RnsmComponentImplementation()
        {
            k_DefaultPanelSettings =
                Resources.Load<PanelSettings>(StringConstants.k_ResourcePrefixRnsmDefault + "PanelSettings");
            k_DefaultStyleSheet =
                Resources.Load<StyleSheet>(StringConstants.k_ResourcePrefixRnsmDefault + "StyleSheet");
        }

        internal void UpdateUiVisibility(bool enabled, bool visible)
        {
            RnsmVisualElement.visible = enabled && visible;
        }

        internal RnsmComponentImplementation()
        {
            SubscribeToAllAdapters();
        }

        UnsubscribeFromAllAdapters m_UnsubscribeFromAllAdapters;
        void SubscribeToAllAdapters()
        {
            m_UnsubscribeFromAllAdapters = NetworkAdapters.SubscribeToAll(
                SubscribeToAdapter,
                UnsubscribeFromAdapter);
        }

        void SubscribeToAdapter(INetworkAdapter adapter)
        {
            var metricEvent = adapter.GetComponent<IMetricCollectionEvent>();
            if (metricEvent != null)
            {
                metricEvent.MetricCollectionEvent += OnMetricsReceived;
            }
        }

        void UnsubscribeFromAdapter(INetworkAdapter adapter)
        {
            var metricEvent = adapter.GetComponent<IMetricCollectionEvent>();
            if (metricEvent != null)
            {
                metricEvent.MetricCollectionEvent -= OnMetricsReceived;
            }
        }

        void SetupUiDoc()
        {
            if (UiDoc == null)
            {
                var uiDocGameObject = new GameObject();
                uiDocGameObject.name = "__NetStatsMonitorUiDocObject";
                uiDocGameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSave;

                UiDoc = uiDocGameObject.AddComponent<UIDocument>();
            }

            var rootVisualElement = UiDoc.rootVisualElement;
            if (RnsmVisualElement.parent != rootVisualElement)
            {
                rootVisualElement?.Add(RnsmVisualElement);
            }

            RnsmVisualElement.styleSheets.Add(k_DefaultStyleSheet);
        }

        internal void SetupAndConfigure(
            NetStatsMonitorConfiguration configuration,
            PositionConfiguration position,
            StyleSheet styleSheet,
            PanelSettings panelSettingsOverride,
            double maxRefreshRate)
        {
            SetupUiDoc();
            if (configuration != null)
            {
                configuration.RecomputeConfigurationHash();
            }
            Configure(configuration, position, styleSheet, panelSettingsOverride);

            // The RNSM and stats accumulator keep track of when they were
            // last updated to know when to update next, and to display a
            // message when data has not been received in a configurable
            // amount of time.
            // When first created they have never been updated, but are due
            // for an update. However, if they are given a previous update time
            // that is too far in the past, then they will immediately display
            // a message indicating they have not received data in a _very_ long
            // time.
            // To work around this, provide them with a previous update time that
            // is _just_ long enough to require an update now without triggering
            // the erroneous message.
            var curFrameTime = GetCurrentTime();
            var prevFrameTime = curFrameTime - (1 / maxRefreshRate) - Double.Epsilon;
            m_LastDisplayUpdateTime = prevFrameTime;
            for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
            {
                m_Accumulators[rate].LastAccumulationTime = prevFrameTime;
                m_Accumulators[rate].LastCollectionTime = prevFrameTime;
            }
        }

        internal void Teardown()
        {
            m_UnsubscribeFromAllAdapters?.Invoke();

            m_MultiStatHistory.Clear();

            if (UiDoc != null && UiDoc.gameObject != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(UiDoc.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(UiDoc.gameObject);
                }
            }
        }

        internal void Configure(
            NetStatsMonitorConfiguration configuration,
            PositionConfiguration positionConfiguration,
            StyleSheet customStyleSheet,
            PanelSettings panelSettingsOverride)
        {
            if (customStyleSheet != m_CustomStyleSheet)
            {
                if (m_CustomStyleSheet != null)
                {
                    RnsmVisualElement.styleSheets.Remove(m_CustomStyleSheet);
                }
                if (customStyleSheet != null)
                {
                    RnsmVisualElement.styleSheets.Add(customStyleSheet);
                }
                m_CustomStyleSheet = customStyleSheet;
            }

            UiDoc.panelSettings = panelSettingsOverride != null
                ? panelSettingsOverride
                : k_DefaultPanelSettings;

            RnsmVisualElement.ApplyPosition(positionConfiguration);

            ApplyConfigurationChangesIfHashHasChanged(configuration);
        }

        void ApplyConfigurationChangesIfHashHasChanged(NetStatsMonitorConfiguration configuration)
        {
            // Can't use ?. syntax, as this bypasses internal Unity lifetime check. Ternary is equivalent.
            int? newConfigurationHash = configuration != null ? configuration.ConfigurationHash : null;
            if (newConfigurationHash != m_PreviousConfigurationHash)
            {
                RnsmVisualElement.UpdateConfiguration(configuration);

                // Can't use ?. syntax, as this bypasses internal Unity lifetime check. Ternary is equivalent.
                int? newHistoryRequirementsHash = configuration != null ? configuration.GetHistoryRequirementsHash() : null;

                if (newHistoryRequirementsHash != m_PreviousHistoryRequirementsHash)
                {
                    m_PreviousHistoryRequirementsHash = newHistoryRequirementsHash;
                    var requirements = MultiStatHistoryRequirements.FromConfiguration(configuration);
                    for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
                    {
                        m_Accumulators[rate].UpdateRequirements(requirements, rate);
                    }

                    m_MultiStatHistory.UpdateRequirements(requirements);
                }

                m_PreviousConfigurationHash = newConfigurationHash;
            }

        }

        void CollectStatsIfEnoughTimeHasElapsed(SampleRate rate, double time)
        {
            var accumulator = m_Accumulators[rate];
            var timeSinceLastCollection = time - accumulator.LastCollectionTime;
            var minCollectionInterval = MinCollectionInterval(rate);
            var statsCollectionPending = timeSinceLastCollection > minCollectionInterval;
            if (!statsCollectionPending)
            {
                return;
            }
            m_MultiStatHistory.Collect(rate, accumulator, time);
        }

        void OnMetricsReceived(MetricCollection metricCollection)
        {
            var time = GetCurrentTime();

            for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
            {
                // This additional call to CollectStatsIfEnoughTimeHasElapsed is beneficial in the following scenario:
                // 1. A long time passes without receiving data (so no samples are recorded).
                // 2. After this long empty interval, data is received (possibly following a reconnect).
                // 3. If the new data is included along with the many empty frames, then the new data would be
                //    flattened in the graph over this long period of time.
                // 4. Instead it seems better to first collect this long empty interval, and then start
                //    accumulating from this newly received data.
                CollectStatsIfEnoughTimeHasElapsed(rate, time);

                StatsAggregator.UpdateAccumulatorWithStatsFromMetrics(metricCollection, m_Accumulators[rate], time);
            }
        }

        internal void Update(NetStatsMonitorConfiguration configuration, double maxRefreshRate)
        {
            ApplyConfigurationChangesIfHashHasChanged(configuration);

            var time = GetCurrentTime();

            var lastAccumulationTime = double.MinValue;
            var anyNewDataAvailable = false;
            for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
            {
                var accumulator = m_Accumulators[rate];
                if (accumulator.HasAccumulatedStats)
                {
                    CollectStatsIfEnoughTimeHasElapsed(rate, time);
                }
                lastAccumulationTime = Math.Max(lastAccumulationTime, accumulator.LastAccumulationTime);

                var newDataAvailable = accumulator.LastAccumulationTime > m_LastDisplayUpdateTime;
                m_NewDataAvailable[rate] = newDataAvailable;
                anyNewDataAvailable |= newDataAvailable;
            }

            var timeSinceLastDisplayUpdate = time - m_LastDisplayUpdateTime;
            var displayUpdatePending = maxRefreshRate * timeSinceLastDisplayUpdate >= 1;
            if (!displayUpdatePending)
            {
                return;
            }

            if (!anyNewDataAvailable)
            {
                var secondsSinceDataReceived = time - lastAccumulationTime;
                if (secondsSinceDataReceived > k_NoDataReceivedMessageDelaySeconds)
                {
                    RnsmVisualElement.DisplayDataNotReceivedMessage(secondsSinceDataReceived);
                }
            }
            else
            {
                RnsmVisualElement.UpdateDisplayData(m_MultiStatHistory, m_NewDataAvailable, time);
                OnDisplayUpdate?.Invoke();
                m_LastDisplayUpdateTime = time;
            }
        }

        public void AddCustomValue(MetricId metricId, float value)
        {
            var time = GetCurrentTime();
            for (var rate = SampleRates.k_First; rate <= SampleRates.k_Last; rate = rate.Next())
            {
                var accumulator = m_Accumulators[rate];
                if (!accumulator.Contains(metricId))
                {
                    // There is no display element configured to display
                    // this stat, so there is no point in storing it.
                    return;
                }

                // This additional call to CollectStatsIfEnoughTimeHasElapsed is beneficial in the following scenario:
                // 1. A long time passes without receiving data (so no samples are recorded).
                // 2. After this long empty interval, data is received (possibly following a reconnect).
                // 3. If the new data is included along with the many empty frames, then the new data would be
                //    flattened in the graph over this long period of time.
                // 4. Instead it seems better to first collect this long empty interval, and then start
                //    accumulating from this newly received data.
                CollectStatsIfEnoughTimeHasElapsed(rate, time);

                accumulator.Accumulate(metricId, value);
                accumulator.LastAccumulationTime = time;
            }
        }
    }
}
#endif