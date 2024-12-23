using System.Linq;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.UI
{
    sealed class NetworkPresetDropdown : DropdownField
    {
        public new class UxmlFactory : UxmlFactory<NetworkPresetDropdown, UxmlTraits> { }
        public const string Custom = nameof(Custom);

        public NetworkPresetDropdown()
        {
            var presets = NetworkSimulatorPresets.Values.Select(x => x.Name).ToList();
            presets.Add(Custom);
            choices = presets;
        }

        int IndexOf(string choice)
        {
            return choices.IndexOf(choice);
        }

        internal void UpdatePresetDropdown([CanBeNull]string configurationName)
        {
            configurationName = string.IsNullOrEmpty(configurationName)
                ? Custom
                : configurationName;

            var newIndex = IndexOf(configurationName);
            index = newIndex == -1
                ? IndexOf(Custom)
                : newIndex;
        }
    }
}