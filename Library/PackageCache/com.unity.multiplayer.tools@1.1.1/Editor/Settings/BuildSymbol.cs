using System;

namespace Unity.Multiplayer.Tools.Editor
{
    enum BuildSymbol
    {
        None,

        /// This is phrased as a negative so that the default state (not defined) matches the
        /// desired default behaviour (inclusion in develop builds)
        DisableInDevelop,

        EnableInRelease,

        /// By adding this scripting define symbol users can override our build logic and
        /// forcibly enable the tool in both development and release. This option takes
        /// precedence over DisableInDevelop
        OverrideEnabled,
    }

    static class BuildSymbolStrings
    {
        public const string k_Prefix = "UNITY_MP_TOOLS_";

        static string GetToolSymbolSubString(Tool tool)
        {
            switch (tool)
            {
                case Tool.RuntimeNetStatsMonitor:
                    return "NET_STATS_MONITOR_";
                case Tool.NetworkSimulator:
                    return "NETSIM_";
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }

        static string GetBuildSymbolSubString(BuildSymbol symbol)
        {
            switch (symbol)
            {
                case BuildSymbol.None:
                    return "";
                case BuildSymbol.DisableInDevelop:
                    return "DISABLED_IN_DEVELOP";
                case BuildSymbol.EnableInRelease:
                    return "ENABLED_IN_RELEASE";
                case BuildSymbol.OverrideEnabled:
                    return "IMPLEMENTATION_ENABLED";
                default:
                    throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null);
            }
        }
        
        public static string GetBuildSymbolString(
            Tool tool,
            BuildSymbol symbol)
        {
            return k_Prefix + GetToolSymbolSubString(tool) + GetBuildSymbolSubString(symbol);
        }
    }
}
