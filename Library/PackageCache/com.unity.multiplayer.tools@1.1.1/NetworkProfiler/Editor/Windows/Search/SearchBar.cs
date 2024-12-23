using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class SearchBar : VisualElement
    {
        const string ToolbarSearchFieldName = "ToolbarSearchField";
        const string SearchAssetPath =
                "Packages/com.unity.multiplayer.tools/NetworkProfiler/Editor/Windows/Search/search.uxml";

        static SearchListFilter Filter => Filters.PartialMatchGameObjectFilter;

        readonly Action<IReadOnlyCollection<IRowData>> m_OnSearchResultsChanged;
        readonly Action m_OnSearchStringCleared;

        readonly ToolbarSearchField m_ToolbarSearchField;
        List<IRowData> m_Entries = new List<IRowData>();

        public SearchBar(
            Action<IReadOnlyCollection<IRowData>> onSearchResultsChanged,
            Action onSearchStringCleared)
        {
            m_OnSearchResultsChanged = onSearchResultsChanged;
            m_OnSearchStringCleared = onSearchStringCleared;

            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SearchAssetPath);
            var root = tree.CloneTree();

            m_ToolbarSearchField = root.Q<ToolbarSearchField>(ToolbarSearchFieldName);
            m_ToolbarSearchField.value = DetailsViewPersistentState.SearchBarString;
            m_ToolbarSearchField.RegisterValueChangedCallback(OnSearchFieldChanged);

            tooltip = Tooltips.SearchBar;

            Add(root);
        }

        internal void SetEntries(TreeModel treeModel)
        {
            m_Entries = TreeModelUtility.FlattenTree(treeModel);
            RefreshSearchResults();
        }

        void OnSearchFieldChanged(ChangeEvent<string> searchString)
        {
            DetailsViewPersistentState.SearchBarString = searchString.newValue;
            RefreshSearchResults();
        }

        void RefreshSearchResults()
        {
            var searchString = m_ToolbarSearchField.value;
            var isSearching = !string.IsNullOrWhiteSpace(searchString);
            if (isSearching)
            {
                var entries = Filter.Invoke(m_Entries, searchString);
                m_OnSearchResultsChanged.Invoke(entries);
                return;
            }
            m_OnSearchStringCleared.Invoke();
        }
    }
}