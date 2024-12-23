#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsReporting
{

#if UNITY_MP_TOOLS_DEV
    [AddComponentMenu("Multiplayer Tools/" + "Test Data Generator", 1000)]
#else
    [AddComponentMenu("")]
#endif
    [Obsolete("This test class has been marked obsolete because it was not intended to be a public API", false)]
    public class TestDataGeneratorComponent : MonoBehaviour
    {
    }
}

#endif