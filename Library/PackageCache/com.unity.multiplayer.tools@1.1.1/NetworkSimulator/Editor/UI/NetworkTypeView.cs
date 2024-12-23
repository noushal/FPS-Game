using System.ComponentModel;
using System.Linq;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    class NetworkTypeView : VisualElement
    {
        const string UXML = "Packages/com.unity.multiplayer.tools/NetworkSimulator/Editor/UI/NetworkTypeView.uxml";

        const string CustomTooltipText = "Changes to the parameters in the inspector will be reflected in the Custom Connection Type asset.";
        const string BuiltInTooltipText = "Built-in connection types cannot be edited.";

        const int k_MaxDelay = 5_000;

        readonly Runtime.NetworkSimulator m_NetworkSimulator;

        NetworkPresetDropdown PresetDropdown => this.Q<NetworkPresetDropdown>(nameof(PresetDropdown));
        ObjectField CustomPresetValue => this.Q<ObjectField>(nameof(CustomPresetValue));
        HelpBox HelpBox => this.Q<HelpBox>(nameof(HelpBox));
        VisualElement PacketDelayRangeContainer => this.Q<VisualElement>(nameof(PacketDelayRangeContainer));
        IntegerField PacketDelayRangeMinValue => this.Q<IntegerField>(nameof(PacketDelayRangeMinValue));
        IntegerField PacketDelayRangeMaxValue => this.Q<IntegerField>(nameof(PacketDelayRangeMaxValue));
        MinMaxSlider PacketDelayRange => this.Q<MinMaxSlider>(nameof(PacketDelayRange));
        SliderInt PacketDelaySlider => this.Q<SliderInt>(nameof(PacketDelaySlider));
        SliderInt PacketJitterSlider => this.Q<SliderInt>(nameof(PacketJitterSlider));
        SliderInt PacketLossIntervalSlider => this.Q<SliderInt>(nameof(PacketLossIntervalSlider));
        SliderInt PacketLossPercentSlider => this.Q<SliderInt>(nameof(PacketLossPercentSlider));

        readonly SerializedObject m_SerializedObject;
        readonly SerializedProperty m_ConfigurationObject;
        readonly SerializedProperty m_ConfigurationReference;

        bool HasCustomValue => m_NetworkSimulator.m_PresetAsset != default;

        public NetworkTypeView(SerializedObject serializedObject, Runtime.NetworkSimulator networkSimulator)
        {
            m_NetworkSimulator = networkSimulator;
            m_SerializedObject = serializedObject;
            m_ConfigurationObject = m_SerializedObject.FindProperty(nameof(Runtime.NetworkSimulator.m_PresetAsset));
            m_ConfigurationReference = m_SerializedObject.FindProperty(nameof(Runtime.NetworkSimulator.m_PresetReference));
            m_SerializedObject.Update();

            if (m_NetworkSimulator.ConnectionPreset == null)
            {
                SetSimulatorConfiguration(NetworkSimulatorPresets.None);
            }

            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML).CloneTree(this);

            CustomPresetSetup();

            this.AddEventLifecycle(OnAttach, OnDetach);
        }

        void OnAttach(AttachToPanelEvent evt)
        {
            PresetDropdown.UpdatePresetDropdown(m_NetworkSimulator.ConnectionPreset?.Name);

            UpdateHelpText();
            UpdateSliders(m_NetworkSimulator.ConnectionPreset);
            UpdateEnabled();

            CustomPresetValue.RegisterCallback<ChangeEvent<Object>>(OnCustomPresetChange);
            PacketDelayRange.RegisterCallback<ChangeEvent<Vector2>>(OnPacketDelayRangeChange);
            PacketDelayRangeMinValue.RegisterCallback<ChangeEvent<int>>(OnPacketDelayRangeMinValueChange);
            PacketDelayRangeMaxValue.RegisterCallback<ChangeEvent<int>>(OnPacketDelayRangeMaxValueChange);
            PacketDelaySlider.RegisterCallback<ChangeEvent<int>>(OnPacketDelayChange);
            PacketJitterSlider.RegisterCallback<ChangeEvent<int>>(OnPacketJitterChanged);
            PacketLossIntervalSlider.RegisterCallback<ChangeEvent<int>>(OnPacketLossIntervalChange);
            PacketLossPercentSlider.RegisterCallback<ChangeEvent<int>>(OnPacketLossPercentChange);
            PresetDropdown.RegisterCallback<ChangeEvent<string>>(OnPresetSelected);

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            m_NetworkSimulator.m_PropertyChanged += OnNetworkSimulatorPropertyChanged;
        }

        void OnDetach(DetachFromPanelEvent evt)
        {
            CustomPresetValue.UnregisterCallback<ChangeEvent<Object>>(OnCustomPresetChange);
            PacketDelayRange.UnregisterCallback<ChangeEvent<Vector2>>(OnPacketDelayRangeChange);
            PacketDelayRangeMinValue.UnregisterCallback<ChangeEvent<int>>(OnPacketDelayRangeMinValueChange);
            PacketDelayRangeMaxValue.UnregisterCallback<ChangeEvent<int>>(OnPacketDelayRangeMaxValueChange);
            PacketDelaySlider.UnregisterCallback<ChangeEvent<int>>(OnPacketDelayChange);
            PacketJitterSlider.UnregisterCallback<ChangeEvent<int>>(OnPacketJitterChanged);
            PacketLossIntervalSlider.UnregisterCallback<ChangeEvent<int>>(OnPacketLossIntervalChange);
            PacketLossPercentSlider.UnregisterCallback<ChangeEvent<int>>(OnPacketLossPercentChange);
            PresetDropdown.UnregisterCallback<ChangeEvent<string>>(OnPresetSelected);

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            m_NetworkSimulator.m_PropertyChanged -= OnNetworkSimulatorPropertyChanged;
        }

        void OnPacketDelayRangeChange(ChangeEvent<Vector2> change)
        {
            UpdatePacketDelayRange(change.newValue);
        }

        void OnPacketDelayRangeMinValueChange(ChangeEvent<int> change)
        {
            var min = change.newValue;
            var max = Mathf.Max(min, PacketDelayRangeMaxValue.value);
            PacketDelayRange.value = new(min, max);
        }

        void OnPacketDelayRangeMaxValueChange(ChangeEvent<int> change)
        {
            var max = change.newValue;
            var min = Mathf.Min(max, PacketDelayRangeMinValue.value);
            PacketDelayRange.value = new(min, max);
        }

        void OnPacketDelayChange(ChangeEvent<int> change)
        {
            var min = change.newValue - PacketJitterSlider.value;
            var max = change.newValue + PacketJitterSlider.value;

            if (min < 0)
            {
                max += min;
                min = 0;
            }
            else if (max > k_MaxDelay)
            {
                min += max - k_MaxDelay;
                max = k_MaxDelay;
            }
            
            PacketDelayRange.value = new(min, max);
        }

        void OnPacketJitterChanged(ChangeEvent<int> change)
        {
            var min = PacketDelaySlider.value - change.newValue;
            var max = PacketDelaySlider.value + change.newValue;
            
            if (min < 0)
            {
                max -= min;
                min = 0;
            }
            else if (max > k_MaxDelay)
            {
                min -= max - k_MaxDelay;
                max = k_MaxDelay;
            }
            
            PacketDelayRange.value = new(min, max);
        }

        void OnPacketLossIntervalChange(ChangeEvent<int> change)
        {
            UpdatePacketLossInterval(change.newValue);
            UpdatePacketLossPercent(0);
        }

        void OnPacketLossPercentChange(ChangeEvent<int> change)
        {
            UpdatePacketLossInterval(0);
            UpdatePacketLossPercent(change.newValue);
        }

        void OnUndoRedoPerformed()
        {
            PresetDropdown.UpdatePresetDropdown(m_NetworkSimulator.ConnectionPreset?.Name);
        }

        void OnNetworkSimulatorPropertyChanged(object sender, PropertyChangedEventArgs _)
        {
            PresetDropdown.UpdatePresetDropdown(m_NetworkSimulator.ConnectionPreset?.Name);
            UpdateSliders(m_NetworkSimulator.ConnectionPreset);

            if (m_NetworkSimulator.ConnectionPreset is Object configurationObject)
            {
                CustomPresetValue.SetValueWithoutNotify(configurationObject);
            }
        }

        void CustomPresetSetup()
        {
            CustomPresetValue.objectType = typeof(NetworkSimulatorPresetAsset);

            if (HasCustomValue && m_NetworkSimulator.ConnectionPreset is Object configurationObject)
            {
                CustomPresetValue.value = configurationObject;
            }
        }

        void OnPresetSelected(ChangeEvent<string> changeEvent)
        {
            if (string.IsNullOrWhiteSpace(changeEvent.newValue) || changeEvent.newValue == NetworkPresetDropdown.Custom)
            {
                PresetDropdown.SetValueWithoutNotify(NetworkPresetDropdown.Custom);
            }
            else
            {
                var preset = NetworkSimulatorPresets.Values.First(x => x.Name == changeEvent.newValue);
                SetSimulatorConfiguration(preset);
                UpdateSliders(preset);

                CustomPresetValue.SetValueWithoutNotify(null);
            }

            UpdateHelpText();
            UpdateEnabled();
            UpdateLiveIfPlaying();
        }

        void OnCustomPresetChange(ChangeEvent<Object> evt)
        {
            var configuration = evt.newValue as INetworkSimulatorPreset;
            SetSimulatorConfiguration(configuration);

            UpdateHelpText();
            UpdateEnabled();
            UpdateSliders(m_NetworkSimulator.ConnectionPreset);

            PresetDropdown.SetValueWithoutNotify(NetworkPresetDropdown.Custom);
        }

        void UpdateEnabled()
        {
            PacketDelayRangeContainer.SetEnabled(HasCustomValue);
            PacketDelaySlider.SetEnabled(HasCustomValue);
            PacketJitterSlider.SetEnabled(HasCustomValue);
            PacketLossIntervalSlider.SetEnabled(HasCustomValue);
            PacketLossPercentSlider.SetEnabled(HasCustomValue);
        }

        void UpdateSliders(INetworkSimulatorPreset preset)
        {
            var delay = preset?.PacketDelayMs ?? 0;
            var jitter = preset?.PacketJitterMs ?? 0;
            var min = delay - jitter;
            var max = delay + jitter;
            PacketDelayRange.value = new(min, max);
            UpdatePacketLossInterval(preset?.PacketLossInterval ?? 0);
            UpdatePacketLossPercent(preset?.PacketLossPercent ?? 0);
        }

        void UpdatePacketDelayRange(Vector2 range)
        {
            UpdatePacketDelayRangeMinValue(Mathf.FloorToInt(range.x));
            UpdatePacketDelayRangeMaxValue(Mathf.FloorToInt(range.y));
            UpdatePacketJitter(Mathf.FloorToInt((range.y - range.x)/2f));
            UpdatePacketDelay(Mathf.FloorToInt(range.x + PacketJitterSlider.value));
        }

        void UpdatePacketDelayRangeMinValue(int value)
        {
            PacketDelayRangeMinValue.SetValueWithoutNotify(value);
        }
        
        void UpdatePacketDelayRangeMaxValue(int value)
        {
            PacketDelayRangeMaxValue.SetValueWithoutNotify(value);
        }

        void UpdatePacketDelay(int value)
        {
            PacketDelaySlider.SetValueWithoutNotify(value);

            if (m_NetworkSimulator.ConnectionPreset != null)
            {
                m_NetworkSimulator.ConnectionPreset.PacketDelayMs = value;
            }

            UpdateLiveIfPlaying();
        }

        void UpdatePacketJitter(int value)
        {
            PacketJitterSlider.SetValueWithoutNotify(value);

            if (m_NetworkSimulator.ConnectionPreset != null)
            {
                m_NetworkSimulator.ConnectionPreset.PacketJitterMs = value;
            }

            UpdateLiveIfPlaying();
        }

        void UpdatePacketLossInterval(int value)
        {
            PacketLossIntervalSlider.SetValueWithoutNotify(value);

            if (m_NetworkSimulator.ConnectionPreset != null)
            {
                m_NetworkSimulator.ConnectionPreset.PacketLossInterval = value;
            }

            UpdateLiveIfPlaying();
        }

        void UpdatePacketLossPercent(int value)
        {
            PacketLossPercentSlider.SetValueWithoutNotify(value);

            if (m_NetworkSimulator.ConnectionPreset != null)
            {
                m_NetworkSimulator.ConnectionPreset.PacketLossPercent = value;
            }

            UpdateLiveIfPlaying();
        }

        void UpdateLiveIfPlaying()
        {
            if (Application.isPlaying)
            {
                m_NetworkSimulator.UpdateLiveParameters();
            }
        }

        void SetSimulatorConfiguration(INetworkSimulatorPreset preset)
        {
            if (preset is Object configurationObject)
            {
                m_ConfigurationObject.objectReferenceValue = configurationObject;
                m_ConfigurationReference.managedReferenceValue = null;
            }
            else
            {
                m_ConfigurationReference.managedReferenceValue = preset;
                m_ConfigurationObject.objectReferenceValue = null;
            }

            m_SerializedObject.ApplyModifiedProperties();
        }

        void UpdateHelpText()
        {
            if (HasCustomValue)
            {
                HelpBox.text = CustomTooltipText;
                HelpBox.messageType = HelpBoxMessageType.Warning;
            }
            else
            {
                HelpBox.text = BuiltInTooltipText;
                HelpBox.messageType = HelpBoxMessageType.Info;
            }
        }
    }
}
