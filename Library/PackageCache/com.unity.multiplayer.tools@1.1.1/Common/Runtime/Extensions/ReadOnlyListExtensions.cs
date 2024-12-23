using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Common
{
    internal static class ReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T elementToFind )
            where T : IEquatable<T>
        {
            int i = 0;
            foreach(T element in list)
            {
                if (element.Equals(elementToFind))
                {
                    return i;
                }

                i++;
            }
            return -1;
        }
    }
}