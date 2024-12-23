namespace Unity.Multiplayer.Tools.Editor
{
    /// Although this could be a scriptable object in future that stores its own state,
    /// for now all the state that is needed is stored in the existing project settings
    /// in the script compilation define symbols
    class ProjectSettings
    {
        public bool NetStatsMonitorEnabledInDevelop
        {
            // Value is negated here because the define symbol itself is phrased in the negative.
            // The define symbol is phrased in the negative so that the absence of a define symbol
            // provides the correct default: that the RNSM is enabled in develop.
            get => !BuildSettings.GetSymbolInAllBuildTargets(
                Tool.RuntimeNetStatsMonitor,
                BuildSymbol.DisableInDevelop);
            set => BuildSettings.SetSymbolForAllBuildTargets(
                Tool.RuntimeNetStatsMonitor,
                BuildSymbol.DisableInDevelop,
                !value);
        }

        public bool NetStatsMonitorEnabledInRelease
        {
            get => BuildSettings.GetSymbolInAllBuildTargets(
                Tool.RuntimeNetStatsMonitor,
                BuildSymbol.EnableInRelease);
            set => BuildSettings.SetSymbolForAllBuildTargets(
                Tool.RuntimeNetStatsMonitor,
                BuildSymbol.EnableInRelease,
                value);
        }

        public bool NetworkSimulatorEnabledInDevelop
        {
            get => !BuildSettings.GetSymbolInAllBuildTargets(
                Tool.NetworkSimulator,
                BuildSymbol.DisableInDevelop);
            set => BuildSettings.SetSymbolForAllBuildTargets(
                Tool.NetworkSimulator,
                BuildSymbol.DisableInDevelop,
                !value);
        }

        public bool NetworkSimulatorEnabledInRelease
        {
            get => BuildSettings.GetSymbolInAllBuildTargets(
                Tool.NetworkSimulator,
                BuildSymbol.EnableInRelease);
            set => BuildSettings.SetSymbolForAllBuildTargets(
                Tool.NetworkSimulator,
                BuildSymbol.EnableInRelease,
                value);
        }
    }
}
