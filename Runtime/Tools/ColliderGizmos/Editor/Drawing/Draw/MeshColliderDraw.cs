#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class MeshColliderDraw
{
    public static void Draw(MeshCollider c, ColliderGizmosSettings s)
    {
        Mesh mesh = c.sharedMesh;
        if (!mesh) return;
        if ((c.hideFlags & HideFlags.HideInHierarchy) != 0) return;

        var t = c.transform;
        Matrix4x4 matrix = Matrix4x4.TRS(
            t.position,
            t.rotation,
            t.lossyScale
        );

        using (Drawing.Draw.WithLineWidth(s.BorderThikness, true))
        using (Drawing.Draw.WithMatrix(matrix))
        {
            switch (s.DrawMode)
            {
                case ColliderDrawMode.Wire:
                    DrawWire();
                    break;

                case ColliderDrawMode.Fill:
                    DrawFill();
                    break;

                case ColliderDrawMode.WireAndFill:
                    DrawFill();
                    DrawWire();
                    break;
            }
        }

        void DrawWire()
        {
            Drawing.Draw.WireMesh(mesh, s.BorderColor);
        }

        void DrawFill()
        {
            if (SceneView.lastActiveSceneView == null) return;
            Drawing.Draw.SolidMesh(mesh, s.FillColor);
        }
    }
}
#endif
