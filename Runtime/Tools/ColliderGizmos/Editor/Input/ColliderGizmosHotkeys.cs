#if UNITY_EDITOR
using UnityEditor;

public static class ColliderGizmosHotkeys
{
    [MenuItem("Tools/Collider Gizmos/Toggle _#g")] // Shift+G
    private static void Toggle()
    {
        var s = ColliderGizmosSettingsProvider.Settings;
        s.Enabled = !s.Enabled;
        EditorUtility.SetDirty(s);
        SceneView.RepaintAll();
    }
}
#endif
