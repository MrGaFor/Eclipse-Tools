#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EC.Bus
{
    public class BusDebuggerWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("Tools/Bus Debugger")]
        public static void ShowWindow()
        {
            GetWindow<BusDebuggerWindow>("Bus Debugger");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Bus Variables", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Only works in play mode");
                return;
            }

            Dictionary<string, object> vars = BusSystem.GetAllVariables();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var pair in vars)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(pair.Key, GUILayout.Width(150));
                EditorGUILayout.LabelField(pair.Value?.GetType().Name ?? "null", GUILayout.Width(100));
                EditorGUILayout.LabelField(pair.Value?.ToString() ?? "null", GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif