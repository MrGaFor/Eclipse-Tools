#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

public static class TypeIconCache
{
    static Dictionary<Type, Texture> icons = new();

    public static void SetIcon(Type type, Texture icon)
    {
        icons[type] = icon;
    }

    public static Texture GetIcon(Type type)
    {
        icons.TryGetValue(type, out var icon);
        return icon;
    }
}
#endif