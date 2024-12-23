using System.Collections.Generic;
using Unity.Multiplayer.Tools.Common;
using UnityEditor;

namespace Unity.Multiplayer.Tools.Editor
{
    static class ProjectSettingsMenuProvider
    {
        const string k_UIPath = "Project/" + StringConstants.k_SettingsMenuLabel;

        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingsMenuProvider()
        {
            var provider = new SettingsProvider(
                path: k_UIPath,
                scopes: SettingsScope.Project)
            {
                label = StringConstants.k_SettingsMenuLabel,

                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) =>
                {
                    var menu = new ProjectSettingsMenu();
                    rootElement.Add(menu);
                },
                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[]
                {
                    "Multiplayer",
                    "Tools",
                    "Netcode",
                    "Netcode for GameObjects",
                    "NGO",
                    "Network",
                    "Stats",
                    "Monitor",
                    "Net Stats Monitor",
                    "Network Stats Monitor",
                })
            };
            return provider;
        }
    }
}