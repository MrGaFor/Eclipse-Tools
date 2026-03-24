#if UNITY_EDITOR && EC_HIERARCHY
namespace CustomHierarchy
{
    public struct HierarchyObjectStatus
    {
        public bool IsSelected;
        public bool IsHovered;
        public bool IsDropDownHovered;
    }
}
#endif