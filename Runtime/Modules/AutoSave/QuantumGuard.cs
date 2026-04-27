#if UNITY_EDITOR && EC_AUTOSAVE
using System;
using System.Reflection;
using UnityEditor;

[InitializeOnLoad]
public static class QuantumGuard
{
    private static Type _runnerType;
    private static PropertyInfo _defaultProp;
    private static bool _initialized;

    public static bool IsRunning
    {
        get
        {
            if (!EnsureInit())
                return false;

            return _defaultProp.GetValue(null) != null;
        }
    }

    private static bool EnsureInit()
    {
        if (_initialized)
            return _runnerType != null;

        _initialized = true;

        _runnerType = FindType("Quantum.QuantumRunner");
        if (_runnerType == null)
            return false;

        _defaultProp = _runnerType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        return _defaultProp != null;
    }

    private static Type FindType(string fullName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = asm.GetType(fullName);
            if (type != null)
                return type;
        }
        return null;
    }
}
#endif