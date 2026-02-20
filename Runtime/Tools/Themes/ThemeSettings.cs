#if EC_THEMES
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace EC.Themes
{
    [CreateAssetMenu(menuName = "EC/Themes/Theme Settings")]
    public class ThemeSettings : ScriptableObject
    {
        [InfoBox("Use Tools/EC/Modules/Themes")]

        [Title("Color Registry"), ReadOnly, ListDrawerSettings(DefaultExpandedState = true, DraggableItems = false, ShowFoldout = false, HideAddButton = true, HideRemoveButton = true)]
        public string[] ColorIds;

        [Title("Default Theme"), ReadOnly]
        public string DefaultTheme;

        [Title("Themes"), ReadOnly, ListDrawerSettings(DefaultExpandedState = true, DraggableItems = false, ShowFoldout = false, HideAddButton = true, HideRemoveButton = true)]
        public ThemeVariant[] Themes;

        public string[] ThemeIds
        {
            get
            {
                string[] names = new string[Themes.Length];
                for (int i = 0; i < Themes.Length; i++)
                    names[i] = Themes[i].ThemeId;
                return names;
            }
        }
    }
}
#endif