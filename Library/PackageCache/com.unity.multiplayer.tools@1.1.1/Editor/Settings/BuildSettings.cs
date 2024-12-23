using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Unity.Multiplayer.Tools.Editor
{
    static class BuildSettings
    {
        internal static readonly NamedBuildTarget[] k_AllBuildTargets;

        static BuildSettings()
        {
            k_AllBuildTargets = GetAllBuildTargetsViaReflection().ToArray();
        }

        static IEnumerable<NamedBuildTarget> GetAllBuildTargetsViaReflection()
        {
            var buildTargetGroupType = typeof(BuildTargetGroup);
            var buildTargetGroupNames = Enum.GetNames(buildTargetGroupType);
            var buildTargetGroupValues = (BuildTargetGroup[])Enum.GetValues(buildTargetGroupType);
            var buildTargetGroupCount = buildTargetGroupNames.Length;
            List<NamedBuildTarget> allBuildTargets = new List<NamedBuildTarget>();
            for (var i = 0; i < buildTargetGroupCount; ++i)
            {
                var name = buildTargetGroupNames[i];
                var value = buildTargetGroupValues[i];
                if (value == BuildTargetGroup.Unknown)
                {
                    continue;
                }
                var fieldInfo = buildTargetGroupType.GetField(name);
                var attributes = fieldInfo.CustomAttributes;
                var isDeprecated = attributes.Any(attr =>
                    attr.AttributeType == typeof(ObsoleteAttribute));
                if (isDeprecated)
                {
                    continue;
                }
                NamedBuildTarget namedBuildTarget;
                try
                {
                    namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(value);
                }
                catch (ArgumentException)
                {
                    // NamedBuildTarget.FromBuildTarget(BuildTargetGroup) may throw
                    // an ArgumentException. We'll just ignore any BuildTargetGroups
                    // that do not have a corresponding NamedBuildTarget.
                    continue;
                }
                yield return namedBuildTarget;
            }
        }

        /// Adds the symbol to the build target
        static void AddSymbolToBuildTarget(
            NamedBuildTarget buildTarget,
            Tool tool,
            BuildSymbol symbol)
        {
            var symbolString = BuildSymbolStrings.GetBuildSymbolString(tool, symbol);
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);
            if (!defines.Contains(symbolString))
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    buildTarget,
                    defines.Append(symbolString).ToArray());
            }
        }

        /// Removes the symbol from the build target
        static void RemoveSymbolFromBuildTarget(
            NamedBuildTarget buildTarget,
            Tool tool,
            BuildSymbol symbol)
        {
            var symbolString = BuildSymbolStrings.GetBuildSymbolString(tool, symbol);
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] symbols);
            if (symbols.Contains(symbolString))
            {
                var symbolsExcludingRnsmEnable = symbols
                    .Where(scriptingSymbol => scriptingSymbol != symbolString)
                    .ToArray();
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbolsExcludingRnsmEnable);
            }
        }

        /// Enables or disables the symbol in the build target
        static void SetSymbolForBuildTarget(
            NamedBuildTarget buildTarget,
            Tool tool,
            BuildSymbol symbol,
            bool enabled)
        {
            if (enabled)
            {
                AddSymbolToBuildTarget(buildTarget, tool, symbol);
            }
            else
            {
                RemoveSymbolFromBuildTarget(buildTarget, tool, symbol);
            }
        }

        /// Returns true if the symbol is defined in the build targets
        static bool GetSymbolInBuildTarget(
            NamedBuildTarget buildTarget,
            Tool tool,
            BuildSymbol symbol)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);
            return defines.Contains(BuildSymbolStrings.GetBuildSymbolString(tool, symbol));
        }

        /// Adds the symbol to all build targets
        internal static void AddSymbolToAllBuildTargets(Tool tool, BuildSymbol symbol)
        {
            foreach (var buildTarget in k_AllBuildTargets)
            {
                AddSymbolToBuildTarget(buildTarget, tool, symbol);
            }
        }

        /// Removes the symbol from all build targets
        internal static void RemoveSymbolFromAllBuildTargets(Tool tool, BuildSymbol symbol)
        {
            foreach (var buildTarget in k_AllBuildTargets)
            {
                RemoveSymbolFromBuildTarget(buildTarget, tool, symbol);
            }
        }

        /// Enables or disables the symbol in all build targets
        internal static void SetSymbolForAllBuildTargets(
            Tool tool,
            BuildSymbol symbol,
            bool enabled)
        {
            foreach (var buildTarget in k_AllBuildTargets)
            {
                SetSymbolForBuildTarget(buildTarget, tool, symbol, enabled);
            }
        }

        /// Returns true if the symbol is defined in all build targets
        internal static bool GetSymbolInAllBuildTargets(Tool tool, BuildSymbol symbol)
        {
            return k_AllBuildTargets.All(target => GetSymbolInBuildTarget(target, tool, symbol));
        }

        /// Returns true if the symbol is defined in any build targets
        internal static bool GetEnabledForAnyBuildTargets(Tool tool, BuildSymbol symbol)
        {
            return k_AllBuildTargets.Any(target => GetSymbolInBuildTarget(target, tool, symbol));
        }
    }
}
