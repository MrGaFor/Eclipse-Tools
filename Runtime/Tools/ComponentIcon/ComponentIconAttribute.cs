using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ComponentIconAttribute : Attribute
{
    public readonly string Icon;

    /// <param name="icon">Name internal icon Unity (example: "RectTransform Icon") or path to texture in project.</param>
    public ComponentIconAttribute(string icon)
    {
        Icon = icon; 
    }
}