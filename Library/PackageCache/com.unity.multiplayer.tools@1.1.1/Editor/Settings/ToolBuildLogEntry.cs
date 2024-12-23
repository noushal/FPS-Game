using System;
using System.Linq;
using Unity.Multiplayer.Tools.Common;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.Multiplayer.Tools.Editor
{
    class ToolBuildLogEntry : IPostprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPostprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;

            var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            PlayerSettings.GetScriptingDefineSymbols(namedTarget, out var symbols);

            var isDevelopBuild = (report.summary.options & BuildOptions.Development) != 0;
            var isReleaseBuild = !isDevelopBuild;

            foreach (var tool in Enum.GetValues(typeof(Tool)))
            {
                var toolValue = (Tool)tool;

                var enabledInDevelop = !symbols.Contains(BuildSymbolStrings.GetBuildSymbolString(
                    toolValue, BuildSymbol.DisableInDevelop));
                var enabledInRelease =  symbols.Contains(BuildSymbolStrings.GetBuildSymbolString(
                    toolValue, BuildSymbol.EnableInRelease));
                var overrideEnabled  =  symbols.Contains(BuildSymbolStrings.GetBuildSymbolString(
                    toolValue, BuildSymbol.OverrideEnabled));

                // This logic needs to match the preprocessor logic for the inclusion of the tool
                // implementation in the tool implementation source files
                var toolImplementationEnabled = overrideEnabled ||
                    (isDevelopBuild && enabledInDevelop) ||
                    (isReleaseBuild && enabledInRelease);

                var buildType = isDevelopBuild ? "development" : "release";
                var enabled = toolImplementationEnabled ? "enabled" : "disabled";

                Debug.Log($"{StringUtil.AddSpacesToCamelCase(toolValue.ToString())} implementation {enabled} in {buildType} build targeting {target}");
            }
        }
    }
}
