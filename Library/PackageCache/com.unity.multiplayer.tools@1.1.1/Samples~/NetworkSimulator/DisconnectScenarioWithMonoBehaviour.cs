using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;

namespace Samples.NetworkScenarios
{
    public class DisconnectScenarioWithMonoBehaviour : NetworkScenarioBehaviour
    {
        const float m_DeltaTimeBetweenEvents = 1000f;

        INetworkEventsApi m_NetworkEventsApi;
        float m_ElapsedTime;

        public override void Start(INetworkEventsApi networkEventsApi)
        {
            m_NetworkEventsApi = networkEventsApi;
        }

        protected override void Update(float deltaTime)
        {
            m_ElapsedTime += deltaTime;

            if (m_ElapsedTime < m_DeltaTimeBetweenEvents)
            {
                return;
            }

            m_ElapsedTime -= m_DeltaTimeBetweenEvents;

            if (m_NetworkEventsApi.IsConnected)
            {
                m_NetworkEventsApi.Disconnect();
            }
            else
            {
                m_NetworkEventsApi.Reconnect();
            }
        }
    }
}