using System;
using System.Collections.Generic;

namespace LibEngine.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IList<T> collection, Action<T, int> action)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                action(collection[i], i);
            }
        }
    }
}