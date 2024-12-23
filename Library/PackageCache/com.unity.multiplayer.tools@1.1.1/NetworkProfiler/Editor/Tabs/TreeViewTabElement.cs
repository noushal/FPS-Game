using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeViewTabElement : VisualElement
    {
        readonly TreeViewNetwork.DisplayType m_DisplayType;

        readonly ColumnBarNetwork m_ColumnBarNetwork;
        readonly SearchBar m_SearchBar;

        TreeModel m_TreeModel;
        ListViewContainer m_FilteredResultsArea;
        bool m_ShowFiltered;
        bool m_ShowStandard;
        TreeViewNetwork m_TreeView;
        VisualElement m_TreeViewArea;

        public TreeViewTabElement(TreeViewNetwork.DisplayType displayType)
        {
            m_DisplayType = displayType;

            style.flexGrow = 1;

            m_ColumnBarNetwork = new ColumnBarNetwork(
                HandleOnNameClickedEvent,
                HandleOnTypeClickedEvent,
                HandleOnBytesSentClickEvent,
                HandleOnBytesReceivedClickEvent);

            m_SearchBar = new SearchBar(
                HandleOnSearchResultsChanged,
                HandleOnSearchStringCleared);
        }

        public void UpdateMetrics(MetricCollection metricCollection)
        {
            m_TreeModel = ConstructTreeModel(metricCollection, m_DisplayType);
        }

        static TreeModel ConstructTreeModel(MetricCollection metricCollection, TreeViewNetwork.DisplayType displayType)
        {
            return displayType switch
            {
                TreeViewNetwork.DisplayType.Messages => TreeModelUtility.CreateMessagesTreeStructure(metricCollection),
                TreeViewNetwork.DisplayType.Activity => TreeModelUtility.CreateActivityTreeStructure(metricCollection),
                _ => throw new ArgumentOutOfRangeException(nameof(displayType), displayType, null)
            };
        }

        public void Show()
        {
            SetupUIElements();
            ShowStandardTreeView();
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public void Hide()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public void CustomizeToolbar(VisualElement container)
        {
            // Call set entries after adding the callback, so that we can
            // immediately get the callback with the filtered results
            m_SearchBar.SetEntries(m_TreeModel);
            container.Add(m_SearchBar);
        }

        void SetupUIElements()
        {
            m_TreeView = new TreeViewNetwork(m_TreeModel);
            m_TreeView.Show();

            m_TreeViewArea = new VisualElement
            {
                name = "TreeView Area"
            };
            m_TreeViewArea.style.flexGrow = 1;
            m_TreeViewArea.Add(m_TreeView);

            m_FilteredResultsArea = new ListViewContainer();
        }

        void HandleOnNameClickedEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.NameSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.NameSort(isAscending);
            }
        }

        void HandleOnTypeClickedEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.TypeSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.TypeSort(isAscending);
            }
        }

        void HandleOnBytesReceivedClickEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.BytesReceivedSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.BytesReceivedSort(isAscending);
            }
        }

        void HandleOnBytesSentClickEvent(bool isAscending)
        {
            if (m_ShowFiltered)
            {
                m_FilteredResultsArea.BytesSentSort(isAscending);
                Add(m_FilteredResultsArea);
            }

            if (m_ShowStandard)
            {
                m_TreeView.BytesSentSort(isAscending);
            }
        }

        void HandleOnSearchStringCleared()
        {
            ShowStandardTreeView();
        }

        void HandleOnSearchResultsChanged(IReadOnlyCollection<IRowData> results)
        {
            ShowFilteredResults(results);
        }

        void ShowStandardTreeView()
        {
            Clear();

            m_ShowStandard = true;
            m_ShowFiltered = false;
            Add(m_ColumnBarNetwork);
            Add(m_TreeViewArea);
            
            m_TreeView.RefreshSelected();
        }

        void ShowFilteredResults(IEnumerable<IRowData> results)
        {
            Clear();

            m_ShowFiltered = true;
            m_ShowStandard = false;
            Add(m_ColumnBarNetwork);
            m_FilteredResultsArea.CacheResults(results);
            m_FilteredResultsArea.ShowResults();
            Add(m_FilteredResultsArea);
            
            m_TreeView.RefreshSelected();
        }
    }
}
