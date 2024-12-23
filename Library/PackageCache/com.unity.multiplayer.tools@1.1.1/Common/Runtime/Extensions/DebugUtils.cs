using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Unity.Multiplayer.Tools.Common
{
    static class DebugUtil
    {
        /// <summary>
        /// Use #define UNITY_MP_TOOLS_DEBUG_TRACE at the call site in order to enable logging.
        /// </summary>
        /// <param name="message"></param>
        [Conditional("UNITY_MP_TOOLS_DEBUG_TRACE")]
        public static void Trace(string message)
        {
            Debug.Log(message);
        }  
    }
}
