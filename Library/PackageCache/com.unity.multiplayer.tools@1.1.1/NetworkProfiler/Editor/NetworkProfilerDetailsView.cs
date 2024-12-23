using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Tools.NetStats;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class NetworkProfilerDetailsView : VisualElement
    {
        const bool k_ShowTabDropdown = false;
        const int k_TabDropdownWidth = 120;

        readonly IReadOnlyDictionary<string, TreeViewTabElement> m_Tabs;
        readonly VisualElement m_CustomizableToolbarContainer;
        readonly ToolbarMenu m_TabsDropdown;

        MetricCollection m_MetricsCollection;
        TreeViewTabElement m_CurrentTabElement;
        string m_CurrentTab;

        public NetworkProfilerDetailsView()
        {
            m_Tabs = new[]
            {
                TreeViewNetwork.DisplayType.Activity,
                TreeViewNetwork.DisplayType.Messages,
            }
            .ToDictionary(displayType => displayType.ToString(), displayType =>  new TreeViewTabElement(displayType));

            var toolbar = new Toolbar();
            Add(toolbar);
            
            m_TabsDropdown = new ToolbarMenu();
            m_TabsDropdown.style.width = k_TabDropdownWidth;
            toolbar.Add(m_TabsDropdown);

            if (!k_ShowTabDropdown)
            {
                m_TabsDropdown.style.display = DisplayStyle.None;
            }

            m_CustomizableToolbarContainer = new VisualElement();
            m_CustomizableToolbarContainer.style.flexGrow = 1;
            m_CustomizableToolbarContainer.style.flexDirection = FlexDirection.Row;
            toolbar.Add(m_CustomizableToolbarContainer);

            foreach (var tab in m_Tabs)
            {
                m_TabsDropdown.menu.AppendAction(tab.Key, SelectTabFromDropdown, IsTabSelected);
                Add(tab.Value);
                tab.Value.Hide();
            }

            m_CurrentTab = TabNames.Activity;

            ShowTab(m_CurrentTab);

            style.flexGrow = 1;
        }

        public void ShowTab(string tabName)
        {
            if (!m_Tabs.TryGetValue(tabName, out var tab))
            {
                Debug.LogError($"Could not find tab with name {tabName}");
                return;
            }

            m_CurrentTabElement?.Hide();
            m_CurrentTabElement = tab;
            m_CurrentTabElement.UpdateMetrics(m_MetricsCollection);
            m_CurrentTabElement.Show();

            m_CustomizableToolbarContainer.Clear();
            m_CurrentTabElement.CustomizeToolbar(m_CustomizableToolbarContainer);

            m_TabsDropdown.text = tabName;
            m_CurrentTab = tabName;
        }

        public void PopulateView(MetricCollection metricCollection)
        {
            m_MetricsCollection = metricCollection;

            ShowTab(m_CurrentTab);
        }

        DropdownMenuAction.Status IsTabSelected(DropdownMenuAction dropdownMenuAction)
        {
            return dropdownMenuAction.name == m_CurrentTab
                ? DropdownMenuAction.Status.Checked
                : DropdownMenuAction.Status.Normal;
        }

        void SelectTabFromDropdown(DropdownMenuAction dropdownMenuAction)
        {
            ShowTab(dropdownMenuAction.name);
        }
    }
}