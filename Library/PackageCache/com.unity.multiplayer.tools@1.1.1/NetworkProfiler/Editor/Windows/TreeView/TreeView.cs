using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class TreeView : VisualElement
    {
        const string k_ListViewName = "unity-tree-view__list-view";
        const string k_ItemName = "unity-tree-view__item";
        const string k_ItemToggleName = "unity-tree-view__item-toggle";
        const string k_ItemIndentsContainerName = "unity-tree-view__item-indents";
        const string k_ItemIndentName = "unity-tree-view__item-indent";
        const string k_ItemContentContainerName = "unity-tree-view__item-content";

        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription { name = "item-height", defaultValue = 30 };
            readonly UxmlBoolAttributeDescription m_ShowBorder = new UxmlBoolAttributeDescription { name = "show-border", defaultValue = false };
            readonly UxmlEnumAttributeDescription<SelectionType> m_SelectionType = new UxmlEnumAttributeDescription<SelectionType> { name = "selection-type", defaultValue = SelectionType.Single };
            readonly UxmlEnumAttributeDescription<AlternatingRowBackground> m_ShowAlternatingRowBackgrounds = new UxmlEnumAttributeDescription<AlternatingRowBackground> { name = "show-alternating-row-backgrounds", defaultValue = AlternatingRowBackground.None };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(visualElement, bag, creationContext);

                var treeView = (TreeView)visualElement;

                var itemHeight = 0;
                if (m_ItemHeight.TryGetValueFromBag(bag, creationContext, ref itemHeight))
                {
                    treeView.itemHeight = itemHeight;
                }

                treeView.showBorder = m_ShowBorder.GetValueFromBag(bag, creationContext);
                treeView.selectionType = m_SelectionType.GetValueFromBag(bag, creationContext);
                treeView.showAlternatingRowBackgrounds = m_ShowAlternatingRowBackgrounds.GetValueFromBag(bag, creationContext);
            }
        }

        Func<VisualElement> m_MakeItem;
        public Func<VisualElement> makeItem
        {
            get { return m_MakeItem; }
            set
            {
                if (m_MakeItem == value)
                {
                    return;
                }

                m_MakeItem = value;
                ListViewRefresh();
            }
        }

        public event Action<IEnumerable<ITreeViewItem>> onItemsChosen;
        public event Action<IReadOnlyList<ITreeViewItem>> onSelectionChange;
        public event Action<int, bool> onExpandedStateChanged;

        List<ITreeViewItem> m_SelectedItems;
        public ITreeViewItem selectedItem => m_SelectedItems.Count == 0 ? null : m_SelectedItems.First();

        public IEnumerable<ITreeViewItem> selectedItems
        {
            get
            {
                if (m_SelectedItems != null)
                {
                    return m_SelectedItems;
                }

                m_SelectedItems = new List<ITreeViewItem>();
                foreach (var treeItem in items)
                {
                    foreach (var itemId in m_ListView.selectedIndices)
                    {
                        if (treeItem.id == itemId)
                        {
                            m_SelectedItems.Add(treeItem);
                        }
                    }
                }

                return m_SelectedItems;
            }
        }

        Action<VisualElement, ITreeViewItem> m_BindItem;
        public Action<VisualElement, ITreeViewItem> bindItem
        {
            get { return m_BindItem; }
            set
            {
                m_BindItem = value;
                ListViewRefresh();
            }
        }

        public Action<VisualElement, ITreeViewItem> unbindItem { get; set; }

        IList<ITreeViewItem> m_RootItems;
        public IList<ITreeViewItem> rootItems
        {
            get { return m_RootItems; }
            set
            {
                m_RootItems = value;
                Refresh();
            }
        }

        public IEnumerable<ITreeViewItem> items => GetAllItems(m_RootItems);

        public float itemHeight
        {
            get { return m_ListView.GetItemHeight(); }
            set { m_ListView.SetItemHeight(value); }
        }

        public bool horizontalScrollingEnabled
        {
            get { return m_ListView.horizontalScrollingEnabled; }
            set { m_ListView.horizontalScrollingEnabled = value; }
        }

        public bool showBorder
        {
            get { return m_ListView.showBorder;}
            set { m_ListView.showBorder = value; }
        }

        public SelectionType selectionType
        {
            get { return m_ListView.selectionType; }
            set { m_ListView.selectionType = value; }
        }

        public AlternatingRowBackground showAlternatingRowBackgrounds
        {
            get { return m_ListView.showAlternatingRowBackgrounds; }
            set { m_ListView.showAlternatingRowBackgrounds = value; }
        }

        struct TreeViewItemWrapper
        {
            public int id => item.id;
            public int depth;

            public ITreeViewItem item;
        }

        [SerializeField]
        List<int> m_ExpandedItemIds;

        List<TreeViewItemWrapper> m_ItemWrappers;

        readonly ListView m_ListView;

        public TreeView()
        {
            m_SelectedItems = null;
            m_ExpandedItemIds = new List<int>();
            m_ItemWrappers = new List<TreeViewItemWrapper>();

            m_ListView = new ListView
            {
                name = k_ListViewName,
                itemsSource = m_ItemWrappers,
                viewDataKey = k_ListViewName
            };
            m_ListView.AddToClassList(k_ListViewName);

            hierarchy.Add(m_ListView);

            m_ListView.makeItem = MakeTreeItem;
            m_ListView.bindItem = BindTreeItem;
            m_ListView.unbindItem = UnbindTreeItem;
#if UNITY_2022_2_OR_NEWER
            m_ListView.itemsChosen += OnItemsChosen;
            m_ListView.selectionChanged += OnSelectionChange;
#else        
            m_ListView.onItemsChosen += OnItemsChosen;
            m_ListView.onSelectionChange += OnSelectionChange;
#endif
            
            var scrollView = m_ListView.Q<ScrollView>();
            scrollView.contentContainer.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        public TreeView(
            IList<ITreeViewItem> items,
            int itemHeight,
            Func<VisualElement> makeItem,
            Action<VisualElement, ITreeViewItem> bindItem)
            : this()
        {
            m_ListView.SetItemHeight(itemHeight);
            m_MakeItem = makeItem;
            m_BindItem = bindItem;
            m_RootItems = items;

            Refresh();
        }

        void Refresh()
        {
            RegenerateWrappers();
            ListViewRefresh();
        }
        
        static IEnumerable<ITreeViewItem> GetAllItems(IEnumerable<ITreeViewItem> rootItems)
        {
            if (rootItems == null)
            {
                yield break;
            }

            var iteratorStack = new Stack<IEnumerator<ITreeViewItem>>();
            var currentIterator = rootItems.GetEnumerator();

            while (true)
            {
                var hasNext = currentIterator.MoveNext();
                if (!hasNext)
                {
                    if (iteratorStack.Count > 0)
                    {
                        currentIterator = iteratorStack.Pop();
                        continue;
                    }

                    // We're at the end of the root items list.
                    break;
                }

                var currentItem = currentIterator.Current;
                yield return currentItem;

                if (currentItem.hasChildren)
                {
                    iteratorStack.Push(currentIterator);
                    currentIterator = currentItem.children.GetEnumerator();
                }
            }
        }

        void OnKeyDown(KeyDownEvent keyDownEvent)
        {
            var index = m_ListView.selectedIndex;

            var shouldStopPropagation = true;

            switch (keyDownEvent.keyCode)
            {
                case KeyCode.RightArrow:
                    if (!IsExpandedByIndex(index))
                    {
                        ExpandItemByIndex(index);
                    }
                    break;
                case KeyCode.LeftArrow:
                    if (IsExpandedByIndex(index))
                    {
                        CollapseItemByIndex(index);
                    }
                    else
                    {
                        if (m_ItemWrappers[index].item.parent != null)
                        {
                            SetSelection(m_ItemWrappers[index].item.parent.id);
                        }
                    }

                    break;
                default:
                    shouldStopPropagation = false;
                    break;
            }

            if (shouldStopPropagation)
            {
                keyDownEvent.StopPropagation();
            }
        }

        void SetSelection(int id)
        {
            SetSelection(new[] {id});
        }

        void SetSelection(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return;
            }

            var selectedIndexes = ids.Select(id => GetItemIndex(id, true)).ToList();
            ListViewRefresh();
            m_ListView.SetSelection(selectedIndexes);
        }

        public void AddToSelection(int id)
        {
            var index = GetItemIndex(id, true);
            ListViewRefresh();
            m_ListView.AddToSelection(index);
        }

        public void RemoveFromSelection(int id)
        {
            var index = GetItemIndex(id);
            m_ListView.RemoveFromSelection(index);
        }

        int GetItemIndex(int id, bool expand = false)
        {
            var item = FindItem(id);
            if (item == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, $"{nameof(TreeView)}: Item id not found.");
            }

            if (expand)
            {
                var regenerateWrappers = false;
                var itemParent = item.parent;
                while (itemParent != null)
                {
                    if (!m_ExpandedItemIds.Contains(itemParent.id))
                    {
                        m_ExpandedItemIds.Add(itemParent.id);
                        regenerateWrappers = true;
                        onExpandedStateChanged?.Invoke(itemParent.id, true);
                    }
                    itemParent = itemParent.parent;
                }

                if (regenerateWrappers)
                {
                    RegenerateWrappers();
                }
            }

            var index = 0;
            for (; index < m_ItemWrappers.Count; ++index)
            {
                if (m_ItemWrappers[index].id == id)
                {
                    break;
                }
            }

            return index;
        }

        public void CollapseItem(int id)
        {
            if (FindItem(id) == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, $"{nameof(TreeView)}: Item id not found.");
            }

            // Try to find it in the currently visible list.
            for (var i = 0; i < m_ItemWrappers.Count; ++i)
            {
                if (m_ItemWrappers[i].item.id == id)
                {
                    if (IsExpandedByIndex(i))
                    {
                        CollapseItemByIndex(i);
                        return;
                    }
                }
            }

            if (!m_ExpandedItemIds.Contains(id))
            {
                return;
            }

            m_ExpandedItemIds.Remove(id);
            onExpandedStateChanged?.Invoke(id, false);
            Refresh();
        }

        public void ExpandItem(int id)
        {
            if (FindItem(id) == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, $"{nameof(TreeView)}: Item id not found.");
            }

            // Try to find it in the currently visible list.
            for (var i = 0; i < m_ItemWrappers.Count; ++i)
            {
                if (m_ItemWrappers[i].item.id == id)
                {
                    if (!IsExpandedByIndex(i))
                    {
                        ExpandItemByIndex(i);
                        return;
                    }
                }
            }

            if (m_ExpandedItemIds.Contains(id))
            {
                return;
            }

            m_ExpandedItemIds.Add(id);
            onExpandedStateChanged?.Invoke(id, true);
            Refresh();
        }

        public ITreeViewItem FindItem(int id)
        {
            foreach (var item in items)
            {
                if (item.id == id)
                {
                    return item;
                }
            }

            return null;
        }

        void ListViewRefresh()
        {
            m_ListView.Rebuild();
        }

        void OnItemsChosen(IEnumerable<object> chosenItems)
        {
            if (onItemsChosen == null)
            {
                return;
            }

            var itemsList = new List<ITreeViewItem>();
            foreach (var item in chosenItems)
            {
                var wrapper = (TreeViewItemWrapper)item;
                itemsList.Add(wrapper.item);
            }

            onItemsChosen.Invoke(itemsList);
        }

        void OnSelectionChange(IEnumerable<object> selectedListItems)
        {
            if (m_SelectedItems == null)
            {
                m_SelectedItems = new List<ITreeViewItem>();
            }

            m_SelectedItems.Clear();
            foreach (var item in selectedListItems)
            {
                m_SelectedItems.Add(((TreeViewItemWrapper)item).item);
            }

            onSelectionChange?.Invoke(m_SelectedItems);
        }

        void OnItemMouseUp(MouseUpEvent mouseUpEvent)
        {
            if ((mouseUpEvent.modifiers & EventModifiers.Alt) == 0)
            {
                return;
            }

            var target = mouseUpEvent.currentTarget as VisualElement;
            var toggle = target.Q<Toggle>(k_ItemToggleName);
            var index = (int)toggle.userData;
            var item = m_ItemWrappers[index].item;
            var wasExpanded = IsExpandedByIndex(index);

            if (!item.hasChildren)
            {
                return;
            }

            var hashSet = new HashSet<int>(m_ExpandedItemIds);

            if (wasExpanded)
            {
                hashSet.Remove(item.id);
            }
            else
            {
                hashSet.Add(item.id);
            }

            foreach (var child in GetAllItems(item.children))
            {
                if (child.hasChildren)
                {
                    if (wasExpanded)
                    {
                        hashSet.Remove(child.id);
                    }
                    else
                    {
                        hashSet.Add(child.id);
                    }
                }
            }

            m_ExpandedItemIds = hashSet.ToList();

            Refresh();

            mouseUpEvent.StopPropagation();
        }

        VisualElement MakeTreeItem()
        {
            var itemContainer = new VisualElement
            {
                name = k_ItemName,
                style =
                {
                    flexDirection = FlexDirection.Row,
                    color = new Color(210, 210, 210), //#D2D2D2
                    fontSize = 12,
                },
            };
            itemContainer.AddToClassList(k_ItemName);
            itemContainer.RegisterCallback<MouseUpEvent>(OnItemMouseUp);

            var indents = new VisualElement
            {
                name = k_ItemIndentsContainerName,
                style =
                {
                    flexDirection = FlexDirection.Row,
                },
            };
            indents.AddToClassList(k_ItemIndentsContainerName);
            itemContainer.hierarchy.Add(indents);

            var toggle = new Toggle
            {
                name = k_ItemToggleName
            };
            toggle.AddToClassList(Foldout.toggleUssClassName);
            toggle.RegisterValueChangedCallback(ToggleExpandedState);
            itemContainer.hierarchy.Add(toggle);

            var userContentContainer = new VisualElement
            {
                name = k_ItemContentContainerName,
                style =
                {
                    flexGrow = 1,
                },
            };
            userContentContainer.AddToClassList(k_ItemContentContainerName);
            itemContainer.Add(userContentContainer);

            if (m_MakeItem != null)
            {
                userContentContainer.Add(m_MakeItem());
            }

            return itemContainer;
        }

        void UnbindTreeItem(VisualElement element, int index)
        {
            if (unbindItem == null)
            {
                return;
            }

            var item = m_ItemWrappers[index].item;
            var userContentContainer = element.Q(k_ItemContentContainerName).ElementAt(0);
            unbindItem(userContentContainer, item);
        }

        void BindTreeItem(VisualElement element, int index)
        {
            var item = m_ItemWrappers[index].item;

            var indents = element.Q(k_ItemIndentsContainerName);
            indents.Clear();
            for (var i = 0; i < m_ItemWrappers[index].depth; ++i)
            {
                var indentElement = new VisualElement();
                indentElement.AddToClassList(k_ItemIndentName);
                indents.Add(indentElement);
            }

            var toggle = element.Q<Toggle>(k_ItemToggleName);
            toggle.SetValueWithoutNotify(IsExpandedByIndex(index));
            toggle.userData = index;
            toggle.visible = item.hasChildren;

            if (m_BindItem == null)
            {
                return;
            }

            var userContentContainer = element.Q(k_ItemContentContainerName).ElementAt(0);
            m_BindItem(userContentContainer, item);
        }

        bool IsExpandedByIndex(int index)
        {
            return m_ExpandedItemIds.Contains(m_ItemWrappers[index].id);
        }

        void CollapseItemByIndex(int index)
        {
            if (!m_ItemWrappers[index].item.hasChildren)
            {
                return;
            }

            m_ExpandedItemIds.Remove(m_ItemWrappers[index].item.id);
            onExpandedStateChanged?.Invoke(m_ItemWrappers[index].item.id, false);

            var recursiveChildCount = 0;
            var currentIndex = index + 1;
            var currentDepth = m_ItemWrappers[index].depth;
            while (currentIndex < m_ItemWrappers.Count && m_ItemWrappers[currentIndex].depth > currentDepth)
            {
                recursiveChildCount++;
                currentIndex++;
            }

            m_ItemWrappers.RemoveRange(index + 1, recursiveChildCount);

            ListViewRefresh();
        }

        void ExpandItemByIndex(int index)
        {
            if (!m_ItemWrappers[index].item.hasChildren)
            {
                return;
            }

            var childWrappers = new List<TreeViewItemWrapper>();
            CreateWrappers(m_ItemWrappers[index].item.children, m_ItemWrappers[index].depth + 1, ref childWrappers);

            m_ItemWrappers.InsertRange(index + 1, childWrappers);

            m_ExpandedItemIds.Add(m_ItemWrappers[index].item.id);
            onExpandedStateChanged?.Invoke(m_ItemWrappers[index].item.id, true);

            ListViewRefresh();
        }

        void ToggleExpandedState(ChangeEvent<bool> evt)
        {
            var toggle = evt.target as Toggle;
            var index = (int)toggle.userData;
            var isExpanded = IsExpandedByIndex(index);

            Assert.AreNotEqual(isExpanded, evt.newValue);

            if (isExpanded)
            {
                CollapseItemByIndex(index);
            }
            else
            {
                ExpandItemByIndex(index);
            }
        }

        void CreateWrappers(IEnumerable<ITreeViewItem> treeViewItems, int depth, ref List<TreeViewItemWrapper> wrappers)
        {
            foreach (var item in treeViewItems)
            {
                var wrapper = new TreeViewItemWrapper
                {
                    depth = depth,
                    item = item,
                };

                wrappers.Add(wrapper);

                if (m_ExpandedItemIds.Contains(item.id) && item.hasChildren)
                {
                    CreateWrappers(item.children, depth + 1, ref wrappers);
                }
            }
        }

        void RegenerateWrappers()
        {
            m_ItemWrappers.Clear();

            if (m_RootItems == null)
            {
                return;
            }

            CreateWrappers(m_RootItems, 0, ref m_ItemWrappers);
        }
    }
}
