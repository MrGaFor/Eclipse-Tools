// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;

// [InitializeOnLoad]
// public static class OdinInspectorAutoRefreshFix
// {
//     static double _lastTime;
//     static bool _pending;

//     static OdinInspectorAutoRefreshFix()
//     {
//         Debug.Log("Odin Inspector Auto Refresh Fix Initialized");
//         ObjectFactory.componentWasAdded += OnComponentAdded;
//         EditorApplication.update += OnEditorUpdate;
//     }

//     static void OnComponentAdded(Component component)
//     {
//         Debug.Log($"Component added: {component.GetType().Name}");
//         _pending = true;
//         _lastTime = EditorApplication.timeSinceStartup;
//     }

//     static void OnEditorUpdate()
//     {
//         if (!_pending) return;

//         if (EditorApplication.timeSinceStartup - _lastTime < 0.5f)
//             return;

//         _pending = false;

//         UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
//         Debug.Log("Odin Inspector Auto Refresh Fix: Repainting all views");
//     }
// }
// #endif