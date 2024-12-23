#if UNITY_EDITOR

using System;

namespace Unity.Multiplayer.Tools.NetStatsReporting
{
    [Obsolete("This test class has been marked obsolete because it was not intended to be a public API", false)]
    public class TestDataDefinition
    {
        [Obsolete("This constructor has been marked obsolete because it was not intended to be part of the public API", false)]
        public TestDataDefinition(int seed) { }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateGameObjectName()
        {
            return "";
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateComponentName()
        {
            return "";
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateVariableName()
        {
            return "";
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateNamedMessageName()
        {
            return "";
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateRpcName()
        {
            return "";
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public long GenerateByteCount()
        {
            return 0;
        }

        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public string GenerateSceneName()
        {
            return "";
        }
    }
}

#endif
