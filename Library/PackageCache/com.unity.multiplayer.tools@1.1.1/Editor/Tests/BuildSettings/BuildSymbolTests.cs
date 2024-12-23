using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.Editor;

namespace Unity.Multiplayer.Tools.Tests.Editor
{
    public class BuildSymbolTests
    {
        readonly IReadOnlyDictionary<
            Tool,
            IReadOnlyDictionary<BuildSymbol, string>> m_BuildSymbols = new Dictionary<Tool,
                IReadOnlyDictionary<BuildSymbol, string>>
        {
            //RNSM Build Symbols
            {
                Tool.RuntimeNetStatsMonitor,
                new Dictionary<BuildSymbol, string>
                {
                    {
                        BuildSymbol.DisableInDevelop,
                        "UNITY_MP_TOOLS_NET_STATS_MONITOR_DISABLED_IN_DEVELOP"
                    },
                    {
                        BuildSymbol.EnableInRelease,
                        "UNITY_MP_TOOLS_NET_STATS_MONITOR_ENABLED_IN_RELEASE"
                    },
                    {
                        BuildSymbol.OverrideEnabled,
                        "UNITY_MP_TOOLS_NET_STATS_MONITOR_IMPLEMENTATION_ENABLED"
                    }
                }
            },
            //Network Simulator Build Symbols
            {
                Tool.NetworkSimulator,
                new Dictionary<BuildSymbol, string>
                {
                    {
                        BuildSymbol.DisableInDevelop,
                        "UNITY_MP_TOOLS_NETSIM_DISABLED_IN_DEVELOP"
                    },
                    {
                        BuildSymbol.EnableInRelease,
                        "UNITY_MP_TOOLS_NETSIM_ENABLED_IN_RELEASE"
                    },
                    {
                        BuildSymbol.OverrideEnabled,
                        "UNITY_MP_TOOLS_NETSIM_IMPLEMENTATION_ENABLED"
                    }
                }
            }
        };

        [Test]
        public void When_GettingToolBuildSymbol_IsExpectedString()
        {
            foreach (var tool in BuildTestsConstants.k_AllTools)
            {
                foreach (var symbol in BuildTestsConstants.k_AllBuildSymbols)
                {
                    var expectedString = m_BuildSymbols[tool][symbol];
                    var actualString = BuildSymbolStrings.GetBuildSymbolString(tool, symbol);
                    Assert.AreEqual(expectedString, actualString);
                }
            }
        }
    }
}
