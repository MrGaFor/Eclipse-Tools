#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class ColliderGizmosWindow : EditorWindow
{
    [MenuItem("Tools/Collider Gizmos/Settings")]
    private static void Open()
    {
        GetWindow<ColliderGizmosWindow>("Collider Gizmos");
    }

    private void OnGUI()
    {
        var s = ColliderGizmosSettingsProvider.Settings;

        EditorGUI.BeginChangeCheck();

        s.Enabled = EditorGUILayout.Toggle("Enabled", s.Enabled);

        EditorGUILayout.Space();

        s.DrawMode = (ColliderDrawMode)EditorGUILayout.EnumPopup("Draw Mode", s.DrawMode);
        s.Scope = (ColliderDrawScope)EditorGUILayout.EnumPopup("Scope", s.Scope);

        EditorGUILayout.Space();

        s.ColliderTypes = (ColliderTypeMask)EditorGUILayout.EnumFlagsField("Collider Types", s.ColliderTypes);
        s.IncludeInactive = EditorGUILayout.Toggle("Include Inactive", s.IncludeInactive);
        s.LayerMask = EditorGUILayout.LayerField("Layer Mask", s.LayerMask);

        EditorGUILayout.Space();

        s.BorderThikness = EditorGUILayout.FloatField("Border Thikness", s.BorderThikness);
        s.BorderColor = EditorGUILayout.ColorField("Border Color", s.BorderColor);
        s.FillColor = EditorGUILayout.ColorField("Fill Color", s.FillColor);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(s);
            SceneView.RepaintAll();
        }
    }
}
#endif
