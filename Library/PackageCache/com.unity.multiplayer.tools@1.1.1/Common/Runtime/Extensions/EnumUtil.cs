using System;

namespace Unity.Multiplayer.Tools.Common
{
    internal static class EnumUtil {
        public static T[] GetValues<T>() {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}