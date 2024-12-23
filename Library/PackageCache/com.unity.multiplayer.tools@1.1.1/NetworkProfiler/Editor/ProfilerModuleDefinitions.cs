using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    internal static class ProfilerModuleDefinitions
    {
        internal static readonly ProfilerModuleDefinition ObjectsProfilerModule = new ProfilerModuleDefinition(
            ModuleNames.GameObjects,
            new []
            {
                ProfilerCounters.rpc.Bytes.Sent, 
                ProfilerCounters.rpc.Bytes.Received,
                ProfilerCounters.networkVariableDelta.Bytes.Sent,
                ProfilerCounters.networkVariableDelta.Bytes.Received,
                ProfilerCounters.objectSpawned.Bytes.Sent,
                ProfilerCounters.objectSpawned.Bytes.Received,
                ProfilerCounters.objectDestroyed.Bytes.Sent,
                ProfilerCounters.objectDestroyed.Bytes.Received,
                ProfilerCounters.ownershipChange.Bytes.Sent,
                ProfilerCounters.ownershipChange.Bytes.Received,
            });
        
        internal static readonly ProfilerModuleDefinition MessagesProfilerModule = new ProfilerModuleDefinition(
            ModuleNames.Message,
            new []
            {
                ProfilerCounters.totalBytes.Sent,
                ProfilerCounters.totalBytes.Received,
                ProfilerCounters.namedMessage.Bytes.Sent,
                ProfilerCounters.namedMessage.Bytes.Received,
                ProfilerCounters.unnamedMessage.Bytes.Sent,
                ProfilerCounters.unnamedMessage.Bytes.Received,
                ProfilerCounters.sceneEvent.Bytes.Sent,
                ProfilerCounters.sceneEvent.Bytes.Received,
            });

        static ProfilerModuleDefinitions()
        {
            Modules = new[]
            {
                MessagesProfilerModule,
                ObjectsProfilerModule
            };
        }
        
        public static IReadOnlyList<ProfilerModuleDefinition> Modules { get; }

        static ProfilerCounters ProfilerCounters => ProfilerCounters.Instance;
    }

    internal struct ProfilerModuleDefinition
    {
        public ProfilerModuleDefinition(string name, IReadOnlyList<string> counters)
        {
            Name = name;
            Counters = counters;
        }

        public string Name { get; }

        public IReadOnlyList<string> Counters { get; }
    }
}