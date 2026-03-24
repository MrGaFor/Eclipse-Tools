#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Управляет назначением иконок компонентам и кэшированием для обычных классов.
/// </summary>
[InitializeOnLoad]
public static class ComponentIconSystem
{
    private static readonly Dictionary<Type, MonoScript> scriptCache = new();
    private static readonly Dictionary<string, Texture2D> iconCache = new();
    private static readonly Dictionary<Type, Texture> typeIcons = new();

    static ComponentIconSystem()
    {
        // Назначаем иконки после загрузки редактора и после reload скриптов
        EditorApplication.delayCall += ApplyIcons;
        AssemblyReloadEvents.afterAssemblyReload += ApplyIcons;
    }

    /// <summary>
    /// Получить иконку для типа. Работает и для обычных классов.
    /// </summary>
    public static Texture GetTypeIcon(Type type)
    {
        typeIcons.TryGetValue(type, out var icon);
        if (icon != null) return icon;

        // fallback: стандартная иконка Unity
        return EditorGUIUtility.ObjectContent(null, type).image;
    }

    /// <summary>
    /// Основной проход по типам с атрибутом ComponentIcon.
    /// Назначает иконку для MonoBehaviour / ScriptableObject.
    /// Для обычных классов только кэширует иконку.
    /// </summary>
    private static void ApplyIcons()
    {
        var types = TypeCache.GetTypesWithAttribute<ComponentIconAttribute>();

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ComponentIconAttribute>();
            if (attr == null) continue;

            var icon = GetIcon(attr.Icon);
            if (icon == null) continue;

            typeIcons[type] = icon;

            // Если тип наследует UnityEngine.Object — назначаем MonoScript
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var script = GetScript(type);
                if (script != null)
                    EditorGUIUtility.SetIconForObject(script, icon);
            }
        }
    }

    /// <summary>
    /// Кешируем MonoScript, безопасно для Unity.
    /// </summary>
    private static MonoScript GetScript(Type type)
    {
        if (scriptCache.TryGetValue(type, out var script))
            return script;

        foreach (var s in MonoImporter.GetAllRuntimeMonoScripts())
        {
            var t = s.GetClass();
            if (t == null) continue;       // абстрактные / интерфейсы
            if (!t.IsClass) continue;

            if (t == type)
            {
                scriptCache[type] = s;
                return s;
            }
        }

        return null;
    }

    /// <summary>
    /// Загрузка текстуры по имени (встроенная или AssetDatabase)
    /// </summary>
    private static Texture2D GetIcon(string name)
    {
        if (iconCache.TryGetValue(name, out var icon))
            return icon;

        icon = EditorGUIUtility.IconContent(name)?.image as Texture2D;

        if (icon == null)
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(name);

        iconCache[name] = icon;
        return icon;
    }
}
#endif