#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class ColliderGizmosDrawer
{
    public static void Draw()
    {
        var s = ColliderGizmosSettingsProvider.Settings;
        if (!s.Enabled)
            return;

        Handles.zTest = CompareFunction.LessEqual;

        if ((s.ColliderTypes & ColliderTypeMask.Box) != 0)
            DrawBoxes(s);

        if ((s.ColliderTypes & ColliderTypeMask.Sphere) != 0)
            DrawSpheres(s);

        if ((s.ColliderTypes & ColliderTypeMask.Capsule) != 0)
            DrawCapsules(s);

        if ((s.ColliderTypes & ColliderTypeMask.Mesh) != 0)
            DrawMeshes(s);
    }

    private static void DrawBoxes(ColliderGizmosSettings s)
    {
        foreach (var c in ColliderCache.Get<BoxCollider>(s))
            BoxColliderDraw.Draw(c, s);
    }

    private static void DrawSpheres(ColliderGizmosSettings s)
    {
        foreach (var c in ColliderCache.Get<SphereCollider>(s))
            SphereColliderDraw.Draw(c, s);
    }

    private static void DrawCapsules(ColliderGizmosSettings s)
    {
        foreach (var c in ColliderCache.Get<CapsuleCollider>(s))
            CapsuleColliderDraw.Draw(c, s);
    }

    private static void DrawMeshes(ColliderGizmosSettings s)
    {
        foreach (var c in ColliderCache.Get<MeshCollider>(s))
            MeshColliderDraw.Draw(c, s);
    }
}
#endif
