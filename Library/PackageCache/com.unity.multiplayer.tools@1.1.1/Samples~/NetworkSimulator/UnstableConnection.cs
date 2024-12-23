using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEngine;
using Random = System.Random;

namespace Samples.NetworkScenario
{
    [UsedImplicitly, Serializable]
    public class UnstableConnection : NetworkScenarioTask
    {
#region Scenario Event Configuration classes
        [Serializable]
        abstract class EventConfiguration
        {
            [SerializeField]
            [Tooltip("Toggle to activate this type of event.")]
            bool m_Active;

            internal bool IsActive => m_Active;

            internal abstract void Activate(INetworkEventsApi networkEventsApi, UnstableConnection scenario);
        }

        [Serializable]
        class LagSpikeConfiguration : EventConfiguration
        {
            [SerializeField, MinMaxRange(0, 5000, true)]
            [Tooltip("Time range (in milliseconds) the lag spike will last.")]
            Vector2 m_TimeRangeInMs = new(0, 5000);

            internal override void Activate(INetworkEventsApi networkEventsApi, UnstableConnection scenario)
            {
                var timeSpan = TimeSpan.FromMilliseconds(scenario.Randomizer.Next((int)m_TimeRangeInMs.x, (int)m_TimeRangeInMs.y));
                networkEventsApi.TriggerLagSpike(timeSpan);
            }
        }

        [Serializable]
        class PacketDelayConfiguration : EventConfiguration
        {
            [SerializeField, MinMaxRange(0, 5000, true)]
            [Tooltip("Delay range (in milliseconds) that will be added to the simulator configuration.")]
            Vector2 m_DelayRangeInMs = new(50, 150);

            internal override void Activate(INetworkEventsApi _, UnstableConnection scenario)
            {
                if (scenario.ShouldResetBetweenEvents)
                {
                    scenario.ResetScenarioConfiguration();
                }

                scenario.ScenarioConfiguration.PacketDelayMs = scenario.Randomizer.Next((int)m_DelayRangeInMs.x, (int)m_DelayRangeInMs.y);
            }
        }

        [Serializable]
        class PacketJitterConfiguration : EventConfiguration
        {
            [SerializeField, MinMaxRange(0, 5000, true)]
            [Tooltip("Jitter range (in milliseconds) that will be added to the simulator configuration.")]
            Vector2 m_JitterRangeInMs = new(50, 100);

            internal override void Activate(INetworkEventsApi _, UnstableConnection scenario)
            {
                if (scenario.ShouldResetBetweenEvents)
                {
                    scenario.ResetScenarioConfiguration();
                }

                scenario.ScenarioConfiguration.PacketJitterMs = scenario.Randomizer.Next((int)m_JitterRangeInMs.x, (int)m_JitterRangeInMs.y);
            }
        }

        [Serializable]
        class PacketLossConfiguration : EventConfiguration
        {
            [SerializeField, MinMaxRange(0, 100, true)]
            [Tooltip("Packet loss percentage range that will be set in the simulator configuration. Minimum is 0. Maximum is 100.")]
            Vector2 m_PacketLossRangeInPercent = new(0, 10);

            internal override void Activate(INetworkEventsApi _, UnstableConnection scenario)
            {
                if (scenario.ShouldResetBetweenEvents)
                {
                    scenario.ResetScenarioConfiguration();
                }

                scenario.ScenarioConfiguration.PacketLossPercent = scenario.Randomizer.Next((int)m_PacketLossRangeInPercent.x, (int)m_PacketLossRangeInPercent.y);
            }
        }

        [Serializable]
        class PacketLossIntervalConfiguration : EventConfiguration
        {
            [SerializeField, MinMaxRange(0, 9999, true)]
            [Tooltip("Packet loss range interval that will be set in the simulator configuration.")]
            Vector2 m_MinimumInterval = new(0, 0);

            internal override void Activate(INetworkEventsApi _, UnstableConnection scenario)
            {
                if (scenario.ShouldResetBetweenEvents)
                {
                    scenario.ResetScenarioConfiguration();
                }

                scenario.ScenarioConfiguration.PacketLossInterval = scenario.Randomizer.Next((int)m_MinimumInterval.x, (int)m_MinimumInterval.y);
            }
        }
#endregion

        [SerializeField, Min(0)]
        [Tooltip("Minimum time (in milliseconds) before an event happen. No event will happen until this amount of time has passed.")]
        int m_MinimumWaitTimeMs = 3000;

        [SerializeField, Min(0)]
        [Tooltip("Maximum time (in milliseconds) before an event happen. If it goes beyond that, an event will happen automatically.")]
        int m_MaximumWaitTimeMs = 5000;

        [SerializeField, Range(0, 100)]
        [Tooltip("How often should an event occur.")]
        int m_EventFrequency = 50;

        [SerializeField]
        [Tooltip("Seed to use for the randomizer. If set to -1, the default seed by System.Random is used.")]
        int m_RandomizerSeed = -1;

        [SerializeField, Range(1, 5)]
        [Tooltip("How many events should occur at a time. Events will not repeat each other on the same trigger.")]
        int m_EventCount = 1;

        [SerializeField]
        [Tooltip("Should the network configuration reset between events. If toggled, every new trigger will start from a clean slate." +
            "If not toggled, every new trigger will change the current configuration.")]
        bool m_ResetConfigurationBetweenEvents;

        [SerializeField]
        LagSpikeConfiguration m_LagSpikeConfiguration = new();

        [SerializeField]
        PacketDelayConfiguration m_PacketDelayConfiguration = new();

        [SerializeField]
        PacketJitterConfiguration m_PacketJitterConfiguration = new();

        [SerializeField]
        PacketLossConfiguration m_PacketLossConfiguration = new();

        [SerializeField]
        PacketLossIntervalConfiguration m_PacketLossIntervalConfiguration = new();

        INetworkSimulatorPreset m_SimulatorCache;
        bool m_IsFirstEventOfTrigger;

        internal Random Randomizer { get; private set; }
        internal INetworkSimulatorPreset ScenarioConfiguration { get; private set; }
        internal bool ShouldResetBetweenEvents => m_ResetConfigurationBetweenEvents && m_IsFirstEventOfTrigger;

        EventConfiguration[] m_EventConfigurations;

        protected override async Task Run(INetworkEventsApi networkEventsApi, CancellationToken cancellationToken)
        {
            m_EventConfigurations = new EventConfiguration[]
            {
                m_LagSpikeConfiguration,
                m_PacketDelayConfiguration,
                m_PacketJitterConfiguration,
                m_PacketLossConfiguration,
                m_PacketLossIntervalConfiguration
            };

            Randomizer = m_RandomizerSeed == -1
                ? new Random()
                : new Random(m_RandomizerSeed);
            
            m_SimulatorCache = networkEventsApi.CurrentPreset;

            ResetScenarioConfiguration();
            var lastTriggerTime = 0f;

            try
            {
                while (cancellationToken.IsCancellationRequested == false)
                {
                    if (IsPaused)
                    {
                        await Task.Yield();
                        continue;
                    }

                    var shouldDoSomething = lastTriggerTime > m_MinimumWaitTimeMs &&
                        (Randomizer.Next(0, 100) < m_EventFrequency
                            || lastTriggerTime > m_MaximumWaitTimeMs);

                    if (shouldDoSomething)
                    {
                        await TriggerNetworkEvents(networkEventsApi, cancellationToken);
                        lastTriggerTime = m_MinimumWaitTimeMs;
                        await Task.Delay(m_MinimumWaitTimeMs, cancellationToken);
                    }
                    else
                    {
                        lastTriggerTime += Time.deltaTime * 1000;
                        await Task.Yield();
                    }
                }
            }
            finally
            {
                //Setting back the original preset at the end of the scenario
                networkEventsApi.ChangeConnectionPreset(m_SimulatorCache);
            }
        }

        internal void ResetScenarioConfiguration()
        {
            ScenarioConfiguration = NetworkSimulatorPreset.Create("Unstable Connection Configuration",
                packetDelayMs: m_SimulatorCache.PacketDelayMs,
                packetJitterMs: m_SimulatorCache.PacketJitterMs,
                packetLossPercent: m_SimulatorCache.PacketLossPercent,
                packetLossInterval: m_SimulatorCache.PacketLossInterval);
        }

        async Task TriggerNetworkEvents(INetworkEventsApi networkEventsApi, CancellationToken cancellationToken)
        {
            var activeConfigurations = m_EventConfigurations.Where(x => x?.IsActive ?? false).ToList();

            if (activeConfigurations.Count == 0)
            {
                Debug.Log("No Active Configurations");
                await Task.Yield();
                return;
            }

            var eventCount = m_EventCount > activeConfigurations.Count ? activeConfigurations.Count : m_EventCount;

            m_IsFirstEventOfTrigger = true;
            while (eventCount > 0)
            {
                var configIndex = Randomizer.Next(0, activeConfigurations.Count);
                var configuration = activeConfigurations.ToList()[configIndex];
                configuration.Activate(networkEventsApi, this);
                activeConfigurations.Remove(configuration);
                --eventCount;
                m_IsFirstEventOfTrigger = false;
            }

            networkEventsApi.ChangeConnectionPreset(ScenarioConfiguration);
        }
    }
}