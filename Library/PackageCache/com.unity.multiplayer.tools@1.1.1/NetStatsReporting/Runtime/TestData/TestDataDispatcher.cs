#if UNITY_EDITOR

using System;

namespace Unity.Multiplayer.Tools.NetStatsReporting
{
    [Obsolete("This test class has been marked obsolete because it was not intended to be a public API", false)]
    public class TestDataDispatcher
    {
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public TestDataDispatcher(int seed = 0) { }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void DispatchClientFrame() { }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void DispatchServerFrame(uint nbClients = 2) { }
    }
}

#endif