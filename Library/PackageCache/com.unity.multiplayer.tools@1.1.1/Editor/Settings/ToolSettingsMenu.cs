using System;
using Unity.Multiplayer.Tools.Common;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.Editor
{
    class ToolSettingsMenu : VisualElement
    {
        internal ToolSettingsMenu(ProjectSettings settings, Tool tool)
        {
            var foldout = new Foldout
            {
                text = StringUtil.AddSpacesToCamelCase(tool.ToString())
            };
            foldout.Q<Label>()?.AddToClassList(UssClassNames.k_SettingsSubMenuTitle);
            foldout.contentContainer.AddToClassList(UssClassNames.k_SettingsSubMenuContents);
            Add(foldout);
            {
                var includeInDevelop = new Toggle
                {
                    label = "Include in Develop Builds",
                    value = GetToolEnabledInDevelop(settings, tool),
                };
                includeInDevelop.AddToClassList(UssClassNames.k_Setting);
                includeInDevelop.RegisterValueChangedCallback(evt =>
                {
                    SetToolEnabledInDevelop(settings, tool, evt.newValue);
                });
                foldout.Add(includeInDevelop);
            }
            {
                var includeInRelease = new Toggle
                {
                    label = "Include in Release Builds",
                    value = GetToolEnabledInRelease(settings, tool),
                };
                includeInRelease.AddToClassList(UssClassNames.k_Setting);
                includeInRelease.RegisterValueChangedCallback(evt =>
                {
                    SetToolEnabledInRelease(settings, tool, evt.newValue);
                });
                foldout.Add(includeInRelease);
            }
        }

        bool GetToolEnabledInDevelop(ProjectSettings settings, Tool tool)
        {
            switch (tool)
            {
                case Tool.RuntimeNetStatsMonitor:
                    return settings.NetStatsMonitorEnabledInDevelop;
                case Tool.NetworkSimulator:
                    return settings.NetworkSimulatorEnabledInDevelop;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }

        bool GetToolEnabledInRelease(ProjectSettings settings, Tool tool)
        {
            switch (tool)
            {
                case Tool.RuntimeNetStatsMonitor:
                    return settings.NetStatsMonitorEnabledInRelease;
                case Tool.NetworkSimulator:
                    return settings.NetworkSimulatorEnabledInRelease;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }

        void SetToolEnabledInDevelop(ProjectSettings settings, Tool tool, bool value)
        {
            switch (tool)
            {
                case Tool.RuntimeNetStatsMonitor:
                    settings.NetStatsMonitorEnabledInDevelop = value;
                    break;
                case Tool.NetworkSimulator:
                    settings.NetworkSimulatorEnabledInDevelop = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }

        void SetToolEnabledInRelease(ProjectSettings settings, Tool tool, bool value)
        {
            switch (tool)
            {
                case Tool.RuntimeNetStatsMonitor:
                    settings.NetStatsMonitorEnabledInRelease = value;
                    break;
                case Tool.NetworkSimulator:
                    settings.NetworkSimulatorEnabledInRelease = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }
    }
}
