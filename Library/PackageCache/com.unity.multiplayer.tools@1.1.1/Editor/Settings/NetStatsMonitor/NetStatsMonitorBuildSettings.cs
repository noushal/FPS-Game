using System;

namespace Unity.Multiplayer.Tools.Editor
{

    /// <summary>
    /// Methods to control whether the Runtime Net Stats Monitor is included in the build.
    /// When making automated builds of your project, you can use this to dynamically
    /// control whether the monitor is included in release or development builds.
    /// </summary>
    public static class NetStatsMonitorBuildSettings
    {
        // NOTE: These four public, parameterless methods are needed because our CI can only call
        // methods that are public and parameterless (not even optional parameters are allowed).

        /// <summary>
        /// Enables the RNSM in development builds for all build targets
        /// </summary>
        public static void EnableInDevelopForAllBuildTargets()
        {
            BuildSettings.SetSymbolForAllBuildTargets(Tool.RuntimeNetStatsMonitor, BuildSymbol.DisableInDevelop, false);
        }

        /// <summary>
        /// Disables the RNSM in development builds for all build targets
        /// </summary>
        public static void DisableInDevelopForAllBuildTargets()
        {
            BuildSettings.SetSymbolForAllBuildTargets(Tool.RuntimeNetStatsMonitor, BuildSymbol.DisableInDevelop, true);
        }

        /// <summary>
        /// Enables the RNSM in release builds for all build targets
        /// </summary>
        public static void EnableInReleaseForAllBuildTargets()
        {
            BuildSettings.SetSymbolForAllBuildTargets(Tool.RuntimeNetStatsMonitor, BuildSymbol.EnableInRelease, true);
        }

        /// <summary>
        /// Disables the RNSM in release builds for all build targets
        /// </summary>
        public static void DisableInReleaseForAllBuildTargets()
        {
            BuildSettings.SetSymbolForAllBuildTargets(Tool.RuntimeNetStatsMonitor, BuildSymbol.EnableInRelease, false);
        }
    }
}
