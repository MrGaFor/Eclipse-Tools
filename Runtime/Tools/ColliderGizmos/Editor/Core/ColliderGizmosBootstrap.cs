#if UNITY_EDITOR
using EC;
using UnityEditor;

[InitializeOnLoad]
public static class ColliderGizmosBootstrap
{
    static ColliderGizmosBootstrap()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        Undo.undoRedoPerformed += SceneView.RepaintAll;
    }

    private static void OnSceneGUI(SceneView view)
    {
        ColliderGizmosDrawer.Draw();
    }

    private static void OnHierarchyChanged()
    {
        ColliderCache.Invalidate();
        SceneView.RepaintAll();
    }
}
#endif
