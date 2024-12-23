using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal class TreeViewItem<T> : ITreeViewItem
    {
        public int id { get; }

        TreeViewItem<T> m_Parent;
        public ITreeViewItem parent => m_Parent;

        List<ITreeViewItem> m_Children;
        public IEnumerable<ITreeViewItem> children => m_Children;

        public bool hasChildren => m_Children != null && m_Children.Count > 0;

        public T data { get; }

        public TreeViewItem(int id, T data, List<TreeViewItem<T>> children = null)
        {
            this.id = id;
            this.data = data;

            if (children != null)
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }
        }

        public void AddChild(ITreeViewItem child)
        {
            var treeChild = child as TreeViewItem<T>;
            if (treeChild == null)
            {
                return;
            }
            
            m_Children ??= new List<ITreeViewItem>();
            m_Children.Add(treeChild);
            treeChild.m_Parent = this;
        }
    }
}
