using System.Collections.Generic;
using Unity.Multiplayer.Tools.Editor;

namespace Unity.Multiplayer.Tools.Tests.Editor
{
    internal static class BuildTestsConstants
    {
        internal static readonly IReadOnlyList<BuildSymbol> k_AllBuildSymbols = new List<BuildSymbol>
        {
            BuildSymbol.DisableInDevelop,
            BuildSymbol.EnableInRelease,
        };

        internal static readonly IReadOnlyList<Tool> k_AllTools = new List<Tool>
        {
            Tool.RuntimeNetStatsMonitor,
            Tool.NetworkSimulator
        };
    }
}
