#if EC_THEMES
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Themes
{
    public class ThemeInput : MonoBehaviour
    {
        [SerializeField, ValueDropdown("ThemeIds")] private string _themeId;

        private string[] ThemeIds => ThemeSettingsProvider.Settings.ThemeIds;

        private ThemeManager Manager => Services.GameLocator.TryGet<ThemeManager>(out var manager) ? manager : null;

        public void ActiveTheme() => ActiveThemeCustom(_themeId);
        public void ActiveThemeCustom(string themeId)
        {
            Manager.ChangeTheme(themeId);
        }


#if UNITY_EDITOR
        public void OnValidate()
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(_themeId))
                _themeId = ThemeSettingsProvider.Settings.Themes[0].ThemeId;
        }
#endif

    }
}
#endif