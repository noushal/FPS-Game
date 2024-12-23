using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkSimulator.Editor")]

#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.Tests.Runtime.NetworkSimulator")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
