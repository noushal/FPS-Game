using System;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEngine;

namespace Samples.NetworkScenarios
{
    [Serializable]
    public class ConnectionParametersWithCurves : NetworkScenarioBehaviour
    {
        [SerializeField]
        float m_LoopDurationMilliseconds = 10f;

        [SerializeField]
        AnimationCurve m_PacketDelayMilliseconds = new(default, new(.5f, 100f), new(1f, 0f));

        [SerializeField]
        AnimationCurve m_PacketJitterMilliseconds = new(default, new(.5f, 50f), new(1f, 0f));

        [SerializeField]
        AnimationCurve m_PacketLossInterval = AnimationCurve.Constant(0f, 1f, 0f);

        [SerializeField]
        AnimationCurve m_PacketLossPercent = new(default, new(.5f, 3f), new(1f, 0f));

        INetworkEventsApi m_NetworkEventsApi;
        INetworkSimulatorPreset m_CustomPreset;
        INetworkSimulatorPreset m_DefaultPreset;
        float m_ElapsedTime;

        public override void Start(INetworkEventsApi networkEventsApi)
        {
            // Keep a reference to the NetworkEventsApi so then we can change the preset in the update.
            m_NetworkEventsApi = networkEventsApi;

            // Store the current preset so then we can revert once the scenario finishes.
            m_DefaultPreset = m_NetworkEventsApi.CurrentPreset;

            // Create a custom preset so then we can change the parameters.
            m_CustomPreset = NetworkSimulatorPreset.Create(nameof(ConnectionParametersWithCurves));

            UpdateParameters();
        }

        protected override void Update(float deltaTime)
        {
            // Calculate the elapsed time for the current loop.
            m_ElapsedTime += deltaTime;

            // Once the elapsed time passes the duration, resets it keeping track of any extra milliseconds.
            if (m_ElapsedTime >= m_LoopDurationMilliseconds)
            {
                m_ElapsedTime -= m_LoopDurationMilliseconds;
            }

            UpdateParameters();
        }

        void UpdateParameters()
        {
            // Calculates the progress of the current loop based on the time elapsed.
            var progress = m_ElapsedTime / m_LoopDurationMilliseconds;

            // Update all curves based on the loop progress.
            m_CustomPreset.PacketDelayMs = (int)m_PacketDelayMilliseconds.Evaluate(progress);
            m_CustomPreset.PacketJitterMs = (int)m_PacketJitterMilliseconds.Evaluate(progress);
            m_CustomPreset.PacketLossInterval = (int)m_PacketLossInterval.Evaluate(progress);
            m_CustomPreset.PacketLossPercent = (int)m_PacketLossPercent.Evaluate(progress);

            // Apply the changed parameters using the NetworkEventsApi.
            m_NetworkEventsApi.ChangeConnectionPreset(m_CustomPreset);
        }

        public override void Dispose()
        {
            // After finishing the current scenario, reset preset to the previous value if there were one.
            m_NetworkEventsApi.ChangeConnectionPreset(m_DefaultPreset);
        }
    }
}
