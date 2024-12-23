#if UNITY_2021_2_OR_NEWER
using System;
using Unity.Profiling.Editor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    [Serializable]
    [ProfilerModuleMetadata(ModuleNames.GameObjects)]
    class NetcodeObjectsProfilerModule : ProfilerModule
    {
        public NetcodeObjectsProfilerModule()
            : base(ProfilerModuleDefinitions.ObjectsProfilerModule.CountersAsDescriptors())
        {
        }

        public override ProfilerModuleViewController CreateDetailsViewController()
        {
            return new NetworkDetailsViewController(ProfilerWindow, TabNames.Activity);
        }
    }
}
#endif
