using UnityEngine;

public enum ColliderDrawMode
{
    Wire,
    Fill,
    WireAndFill
}

public enum ColliderDrawScope
{
    All,
    Selected
}

[System.Flags]
public enum ColliderTypeMask
{
    Box = 1 << 0,
    Sphere = 1 << 1,
    Capsule = 1 << 2,
    Mesh = 1 << 3
}

public sealed class ColliderGizmosSettings : ScriptableObject
{
    public bool Enabled = true;

    public ColliderDrawMode DrawMode = ColliderDrawMode.WireAndFill;
    public ColliderDrawScope Scope = ColliderDrawScope.All;

    public ColliderTypeMask ColliderTypes = ColliderTypeMask.Box | ColliderTypeMask.Sphere | ColliderTypeMask.Capsule;

    public bool IncludeInactive = false;

    public LayerMask LayerMask = ~0;

    public float BorderThikness = 1.5f;
    public Color BorderColor = new Color(0f, 1f, 0f, 1f);
    public Color FillColor = new Color(0f, 1f, 0f, 0.15f);
}
