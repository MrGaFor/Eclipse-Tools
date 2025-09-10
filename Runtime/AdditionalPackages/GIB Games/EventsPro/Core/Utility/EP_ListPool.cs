using System.Collections.Generic;

namespace UnityEngine.UI.Utility
{
    static class EP_ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly EP_ObjectPool<List<T>> s_ListPool = new EP_ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}