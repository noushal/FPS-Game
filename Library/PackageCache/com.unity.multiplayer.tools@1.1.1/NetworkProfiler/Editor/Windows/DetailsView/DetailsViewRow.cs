using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    ///  Details view row entry that can be shared between the list and tree views
    internal class DetailsViewRow : VisualElement
    {
        const int LeftColumnLength = 200;
        const int MiddleColumnLength = 150;
        const int RightColumnLength = 200;

        readonly Label m_NameLabel;
        readonly Label m_TypeLabel;
        readonly Label m_BytesSentLabel;
        readonly Label m_BytesReceivedLabel;

        // Color values from here: https://docs.unity3d.com/2020.2/Documentation/Manual/UIE-USS-UnityVariables.html
        // and yes, we're planning to switch to USS styling because this hardcoded RGB isn't great
        private static readonly Color k_LightThemeTextColor = new Color(9.0f / 256, 9f / 256, 9f / 256);
        private static readonly Color k_DarkThemeTextColor = new Color(210f / 256, 210f / 256, 210f / 256);

        public DetailsViewRow()
        {
            m_NameLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    width = LeftColumnLength,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    unityTextOverflowPosition = TextOverflowPosition.End,
                },
            };
            m_TypeLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = MiddleColumnLength,
                    maxWidth = MiddleColumnLength,
                    minWidth = MiddleColumnLength,
                },
            };
            m_BytesSentLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = MiddleColumnLength,
                    maxWidth = MiddleColumnLength,
                    minWidth = MiddleColumnLength,
                },
            };
            m_BytesReceivedLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleRight,
                    width = RightColumnLength,
                    maxWidth = RightColumnLength,
                    minWidth = RightColumnLength,
                },
            };

            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1f;
            style.flexShrink = 0f;
            style.flexBasis = 0f;

            Add(m_NameLabel);
            Add(m_TypeLabel);
            Add(m_BytesSentLabel);
            Add(m_BytesReceivedLabel);
        }

        public void BindItem(ITreeViewItem item)
        {
            var rowDataItem = item as TreeViewItem<IRowData>;
            if (rowDataItem == null)
            {
                return;
            }

            m_NameLabel.text = rowDataItem.data.Name;
            m_TypeLabel.text = rowDataItem.data.TypeDisplayName;
            m_BytesSentLabel.text = FormatBytes(rowDataItem.data.Bytes.Sent);
            m_BytesReceivedLabel.text = FormatBytes(rowDataItem.data.Bytes.Received);

            var textColorBase = EditorGUIUtility.isProSkin
                ? new StyleColor(k_DarkThemeTextColor)
                : new StyleColor(k_LightThemeTextColor);

            m_NameLabel.style.color = textColorBase;
            m_TypeLabel.style.color = textColorBase;

            var textColorBytes = rowDataItem.data.SentOverLocalConnection
                ? new StyleColor(Color.gray)
                : textColorBase;

            m_BytesSentLabel.style.color = textColorBytes;
            m_BytesReceivedLabel.style.color = textColorBytes;

            // The tooltip text depends on the theme because it describes the significance
            // of text colors that vary based on the theme
            var bytesTooltipText = EditorGUIUtility.isProSkin
                ? Tooltips.DetailsViewBytes_DarkTheme
                : Tooltips.DetailsViewBytes_LightTheme;

            m_BytesSentLabel.tooltip = bytesTooltipText;
            m_BytesReceivedLabel.tooltip = bytesTooltipText;

            userData = item;
        }

        static string FormatBytes(long bytes) => bytes == 0 ? "-" : EditorUtility.FormatBytes(bytes);
    }
}