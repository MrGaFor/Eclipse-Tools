#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BoxColliderDraw
{
    public static void Draw(BoxCollider c, ColliderGizmosSettings s)
    {
        using (Drawing.Draw.WithLineWidth(s.BorderThikness, true))
        using (Drawing.Draw.InLocalSpace(c.transform))
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
                Drawing.Draw.WireBox(c.center, c.size, s.BorderColor);
            }
            void DrawFill()
            {
                Drawing.Draw.SolidBox(c.center, c.size, s.FillColor);
            }
        }
    }
}
#endif
