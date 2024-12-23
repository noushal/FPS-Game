using System.Runtime.CompilerServices;

// Publishers
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.MetricTestData")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsReporting")]

// Listeners
// should only include the NGO 1.0 Adapter.
// Individual tools should listen through the adapter, rather than using this assembly directly.
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Adapters.Ngo1")]

// Listeners (tests)
// should only include the NGO 1.0 Adapter.
// Individual tools should listen through the adapter, rather than using this assembly directly.
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation")]
