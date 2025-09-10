using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GIB
{
    /// <summary>
    /// Static Utilities to help with development
    /// </summary>
    public static class GIBUtils
    {
        public static void Log(string msg, Object go) => Debug.Log($"[<color=yellow>{go.name}</color>] " + msg, go);
        public static void Log(string msg, string go) => Debug.Log($"[<color=yellow>{go}</color>] " + msg);
        public static void Log(string msg) => Debug.Log($"[<color=yellow>GIB Utils</color>] " + msg);
        public static void Warn(string msg, Object go) => Debug.LogWarning($"[<color=orange>{go.name}</color>] " + msg, go);
        public static void Warn(string msg, string go)=> Debug.LogWarning($"[<color=orange>{go}</color>] " + msg);
        public static void Warn(string msg)=>Debug.LogWarning($"[<color=orange>GIB Utils</color>] " + msg);
        public static void Error(string msg, Object go)=>Debug.LogError($"[<color=red>{go.name}</color>] " + msg, go);
        public static void Error(string msg, string go) =>Debug.LogError($"[<color=red>{go}</color>] " + msg);
        public static void Error(string msg) => Debug.LogError($"[<color=red>GIB Utils</color>] " + msg);
    }
}
