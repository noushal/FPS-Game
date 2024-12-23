using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Common
{
    internal static class ListUtil
    {
        public static void Resize<T>(this List<T> list, int size, T element = default)
        {
            var count = list.Count;
            var delta = size - count;

            if (delta < 0)
            {
                list.RemoveRange(size, count - size);
            }
            else if (delta > 0)
            {
                if (size > list.Capacity)
                {
                    list.Capacity = size;
                }
                for (int i = 0; i < delta; ++i)
                {
                    list.Add(element);
                }
            }
        }
        public static void Resize<T>(this List<T> list, int size, Func<T> generator)
        {
            var count = list.Count;
            var delta = size - count;

            if (delta < 0)
            {
                list.RemoveRange(size, count - size);
            }
            else if (delta > 0)
            {
                if (size > list.Capacity)
                {
                    list.Capacity = size;
                }
                for (int i = 0; i < delta; ++i)
                {
                    list.Add(generator());
                }
            }
        }
    }
}