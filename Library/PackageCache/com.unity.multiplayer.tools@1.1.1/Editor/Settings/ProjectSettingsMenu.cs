using Unity.Multiplayer.Tools.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.Editor
{
    internal class ProjectSettingsMenu : VisualElement
    {
        const string k_StylePath = StringConstants.k_PackageSettingsPath + "ProjectSettingsMenu.uss";
        internal ProjectSettingsMenu()
        {
            var settings = new ProjectSettings();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }
            AddToClassList(UssClassNames.k_Menu);

            {
                var title = new Label()
                {
                    text = StringConstants.k_SettingsMenuLabel,
                };
                title.AddToClassList(UssClassNames.k_Title);
                Add(title);
            }

            var contents = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };
            contents.AddToClassList(UssClassNames.k_SettingsContents);
            Add(contents);

            var netStatsMonitorSettings = new ToolSettingsMenu(settings, Tool.RuntimeNetStatsMonitor);
            contents.Add(netStatsMonitorSettings);

            var netSimSettings = new ToolSettingsMenu(settings, Tool.NetworkSimulator);
            contents.Add(netSimSettings);
        }
    }
}