#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ColliderCache
{
    private static readonly Dictionary<System.Type, Collider[]> _allCache = new();
    private static readonly Dictionary<System.Type, Collider[]> _selectedCache = new();
    private static bool _dirty = true;

    public static void Invalidate()
    {
        _dirty = true;
    }

    public static IEnumerable<T> Get<T>(ColliderGizmosSettings s) where T : Collider
    {
        if (_dirty)
            Rebuild();

        var type = typeof(T);
        var source = s.Scope == ColliderDrawScope.Selected
            ? _selectedCache
            : _allCache;

        if (!source.TryGetValue(type, out var list))
            yield break;

        foreach (var c in list)
        {
            if (!PassFilters(c, s))
                continue;

            yield return (T)c;
        }
    }

    private static void Rebuild()
    {
        _allCache.Clear();
        _selectedCache.Clear();

        CacheAll<BoxCollider>();
        CacheAll<SphereCollider>();
        CacheAll<CapsuleCollider>();
        CacheAll<MeshCollider>();

        CacheSelected<BoxCollider>();
        CacheSelected<SphereCollider>();
        CacheSelected<CapsuleCollider>();
        CacheSelected<MeshCollider>();

        _dirty = false;
    }

    private static void CacheAll<T>() where T : Collider
    {
        _allCache[typeof(T)] = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
    }

    private static void CacheSelected<T>() where T : Collider
    {
        var list = new List<T>();
        foreach (var go in Selection.gameObjects)
            list.AddRange(go.GetComponentsInChildren<T>(true));

        _selectedCache[typeof(T)] = list.ToArray();
    }

    private static bool PassFilters(Collider c, ColliderGizmosSettings s)
    {
        var go = c.gameObject;

        if (!s.IncludeInactive && !go.activeInHierarchy)
            return false;

        if (s.LayerMask == 0) // ничего не выбрано → всё разрешено
            return true;

        if ((s.LayerMask.value & (1 << go.layer)) == 0)
            return false;

        return true;
    }

}
#endif
