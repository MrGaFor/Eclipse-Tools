#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SphereColliderDraw
{
    private static Mesh SphereMesh;
    public static void Draw(SphereCollider c, ColliderGizmosSettings s)
    {
        if (SphereMesh == null)
            SphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        var t = c.transform;
        Vector3 scale = t.lossyScale;
        float radius = c.radius * Mathf.Max(scale.x, scale.y, scale.z);
        Matrix4x4 matrix = Matrix4x4.TRS(t.TransformPoint(c.center), t.rotation, Vector3.one * (radius - 0.01f));

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

            void DrawWire()
            {
                Drawing.Draw.WireSphere(Vector3.zero, 1.01f, s.BorderColor);
                Drawing.Draw.SphereOutline(Vector3.zero, 1.01f, s.BorderColor);
            }
            void DrawFill()
            {
                if (SceneView.lastActiveSceneView == null) return;
                Drawing.Draw.SolidMesh(SphereMesh, s.FillColor);
            }
        }
    }
}
#endif
