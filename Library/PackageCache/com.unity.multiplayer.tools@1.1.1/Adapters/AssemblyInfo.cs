using System.Runtime.CompilerServices;

// Adapters
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Adapters.Ngo1")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Adapters.Utp2")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Adapters.Ngo1WithUtp2")]

// Tools
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkProfiler.Runtime")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Implementation")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Runtime")]

// Test assemblies
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation")]

#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Tests.Runtime.NetworkSimulator")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
