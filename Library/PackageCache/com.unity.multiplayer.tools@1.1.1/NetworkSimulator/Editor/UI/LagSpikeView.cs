using System;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    class LagSpikeView : VisualElement
    {
        const string UXML = "Packages/com.unity.multiplayer.tools/NetworkSimulator/Editor/UI/LagSpikeView.uxml";
        const string k_LagSpikeValueString = "LagSpikeValue";

        Button DurationButton100 => this.Q<Button>(nameof(DurationButton100));
        Button DurationButton200 => this.Q<Button>(nameof(DurationButton200));
        Button DurationButton500 => this.Q<Button>(nameof(DurationButton500));
        Button DurationButton1000 => this.Q<Button>(nameof(DurationButton1000));
        Button DurationButton2000 => this.Q<Button>(nameof(DurationButton2000));
        Button DurationButton5000 => this.Q<Button>(nameof(DurationButton5000));

        IntegerField DurationField => this.Q<IntegerField>(nameof(DurationField));
        Button TriggerButton => this.Q<Button>(nameof(TriggerButton));

        void Enter100() => EnterLagSpikeDuration(100);
        void Enter200() => EnterLagSpikeDuration(200);
        void Enter500() => EnterLagSpikeDuration(500);
        void Enter1000() => EnterLagSpikeDuration(1000);
        void Enter2000() => EnterLagSpikeDuration(2000);
        void Enter5000() => EnterLagSpikeDuration(5000);

        readonly INetworkEventsApi m_NetworkEventsApi;

        public LagSpikeView(INetworkEventsApi networkEventsApi)
        {
            m_NetworkEventsApi = networkEventsApi;

            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML).CloneTree(this);

            this.AddEventLifecycle(OnAttach, OnDetach);

            DurationField.viewDataKey = k_LagSpikeValueString;
        }

        void OnAttach(AttachToPanelEvent evt)
        {
            EditorApplication.update += OnUpdate;

            DurationButton100.clicked += Enter100;
            DurationButton200.clicked += Enter200;
            DurationButton500.clicked += Enter500;
            DurationButton1000.clicked += Enter1000;
            DurationButton2000.clicked += Enter2000;
            DurationButton5000.clicked += Enter5000;

            TriggerButton.clicked += TriggerLagSpike;
        }

        void OnDetach(DetachFromPanelEvent evt)
        {
            EditorApplication.update -= OnUpdate;

            DurationButton100.clicked -= Enter100;
            DurationButton200.clicked -= Enter200;
            DurationButton500.clicked -= Enter500;
            DurationButton1000.clicked -= Enter1000;
            DurationButton2000.clicked -= Enter2000;
            DurationButton5000.clicked -= Enter5000;

            TriggerButton.clicked -= TriggerLagSpike;
        }

        void EnterLagSpikeDuration(int durationMilliseconds)
        {
            DurationField.value = durationMilliseconds;
        }

        void TriggerLagSpike()
        {
            if (DurationField.value > 0)
            {
                m_NetworkEventsApi.TriggerLagSpike(TimeSpan.FromMilliseconds(DurationField.value));
            }
        }

        void OnUpdate()
        {
            TriggerButton.SetEnabled(DurationField.value > 0);
        }
    }
}