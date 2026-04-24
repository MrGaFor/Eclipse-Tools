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

        _runnerType = Type.GetType("Quantum.QuantumRunner, Quantum.Engine");
        if (_runnerType == null)
            return false;

        _defaultProp = _runnerType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        return _defaultProp != null;
    }
}
#endif