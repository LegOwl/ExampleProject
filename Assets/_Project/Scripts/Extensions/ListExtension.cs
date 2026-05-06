using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtension
    {
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new InvalidOperationException("List is null or empty");

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static void AddOnce<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }
    }
}