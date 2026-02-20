#if EC_THEMES
using Sirenix.OdinInspector;

namespace EC.Themes
{
    [System.Serializable]
    public class ThemeVariant
    {
        public string ThemeId;

        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DefaultExpandedState = true, DraggableItems = true, ShowFoldout = false)]
        public ThemeColorVariant[] Colors = new ThemeColorVariant[] { };
    }
}
#endif