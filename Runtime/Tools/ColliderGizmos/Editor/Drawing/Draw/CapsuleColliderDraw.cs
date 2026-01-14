#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CapsuleColliderDraw
{
    private static Mesh SphereMesh;
    private static Mesh CylinderMesh;
    public static void Draw(CapsuleCollider c, ColliderGizmosSettings s)
    {
        if (SphereMesh == null)
            SphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        if (CylinderMesh == null)
            CylinderMesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");

        var t = c.transform;
        Vector3 scale = t.lossyScale;

        Vector3 dir = c.direction switch
        {
            0 => Vector3.right,
            1 => Vector3.up,
            2 => Vector3.forward,
            _ => Vector3.up
        };

        float axisScale = c.direction switch
        {
            0 => scale.x,
            1 => scale.y,
            2 => scale.z,
            _ => 1f
        };

        float radiusScale = c.direction switch
        {
            0 => Mathf.Max(scale.y, scale.z),
            1 => Mathf.Max(scale.x, scale.z),
            2 => Mathf.Max(scale.x, scale.y),
            _ => 1f
        };

        float radius = c.radius * radiusScale;
        float height = Mathf.Max(c.height * axisScale, radius * 2f);
        float halfCylinder = Mathf.Max(0f, (height - radius * 2f) * 0.5f);

        Matrix4x4 matrix = Matrix4x4.TRS(t.TransformPoint(c.center), t.rotation, Vector3.one);

        using (Drawing.Draw.WithLineWidth(s.BorderThikness, true))
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
                Vector3 a = dir * halfCylinder;
                Vector3 b = dir * -halfCylinder;

                matrix = Matrix4x4.TRS(t.TransformPoint(c.center), t.rotation, Vector3.one);
                using (Drawing.Draw.WithMatrix(matrix))
                {
                    Drawing.Draw.WireCapsule(a, b, radius * 1.01f, s.BorderColor);
                    Drawing.Draw.SphereOutline(a, radius * 1.01f, s.BorderColor);
                    Drawing.Draw.SphereOutline(b, radius * 1.01f, s.BorderColor);
                }
            }

            void DrawFill()
            {
                if (SceneView.lastActiveSceneView == null) return;

                Vector3 a = dir * halfCylinder;
                Vector3 b = -a;

                Matrix4x4 mA = Matrix4x4.TRS(a, Quaternion.identity, Vector3.one * radius);
                using (Drawing.Draw.WithMatrix(matrix * mA))
                    Drawing.Draw.SolidMesh(SphereMesh, s.FillColor);
                Matrix4x4 mB = Matrix4x4.TRS(b, Quaternion.identity, Vector3.one * radius);
                using (Drawing.Draw.WithMatrix(matrix * mB))
                    Drawing.Draw.SolidMesh(SphereMesh, s.FillColor);
                matrix = Matrix4x4.TRS(t.TransformPoint(c.center), t.rotation, dir * (height * 0.5f - radius) + (Vector3.one - dir) * radius * 0.99f);
                using (Drawing.Draw.WithMatrix(matrix))
                    Drawing.Draw.SolidMesh(CylinderMesh, s.FillColor);
            }
        }
    }
}
#endif
