using System.Runtime.CompilerServices;

namespace Unity.Multiplayer.Tools.Common
{
    static class RuntimeUtils
    {
        public static void NoEffectWarning(this object source, [CallerMemberName] string caller = "")
        {
            NoEffectWarning<bool>(source, caller);
        }

        public static T NoEffectWarning<T>(this object source, [CallerMemberName] string caller = "")
        {
            var type = source.GetType().Name;
            UnityEngine.Debug.LogWarning($"\"{type}.{caller}\" has no effect as it has been disabled by scripting symbols.");
            return default;
        }
    }
}
