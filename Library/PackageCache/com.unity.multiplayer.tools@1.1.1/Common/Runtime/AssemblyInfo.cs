using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Adapters")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Common.Tests")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Initialization")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.MetricTestData")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.MetricTypes")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStats")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Component")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Configuration")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Implementation")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Runtime")]

#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Tests.Runtime.NetworkSimulator")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
