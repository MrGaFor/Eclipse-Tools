#if EC_THEMES
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Themes
{
    public class ThemeManager : Services.GameService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Services.GameLocator.Register(new ThemeManager());
        }

        private Dictionary<string, Dictionary<string, Color>> _themesGenerated;
        private string _activeTheme;

        public event Action OnChangeTheme;
        private const string SaveKey = "ActiveTheme";

        public override void OnCreate()
        {
            base.OnCreate();
            _themesGenerated = new Dictionary<string, Dictionary<string, Color>>();
            foreach (var theme in ThemeSettingsProvider.Settings.Themes)
            {
                Dictionary<string, Color> colors = new Dictionary<string, Color>();
                foreach (var color in theme.Colors)
                    colors.Add(color.ColorId, color.Color);
                _themesGenerated.Add(theme.ThemeId, colors);
            }
            Bus.BusSystem.Subscribe<string>(SaveKey, OnChangedTheme);
            Saver.SaverSystem.AddVariable<string>(SaveKey, ThemeSettingsProvider.Settings.DefaultTheme);
        }

        public void ChangeTheme(string themeId)
        {
            Bus.BusSystem.Set<string>(SaveKey, themeId);
        }

        private void OnChangedTheme(string themeId)
        {
            _activeTheme = themeId;
            OnChangeTheme?.Invoke();
        }

        public Color GetColor(string colorId, string themeId = "")
        {
            if (string.IsNullOrWhiteSpace(themeId))
                themeId = _activeTheme;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var theme in ThemeSettingsProvider.Settings.Themes)
                    if (theme.ThemeId == themeId)
                        foreach (var color in theme.Colors)
                            if (color.ColorId == colorId)
                                return color.Color;
                Debug.LogError($"Themes: ColorId <{colorId}> dont include in themes <{themeId}>");
                return Color.red;
            }
#endif
            return _themesGenerated[themeId][colorId];
        }
#if UNITY_EDITOR
        public static Color GetColorEditor(string colorId)
        {
            foreach (var theme in ThemeSettingsProvider.Settings.Themes)
                if (theme.ThemeId == ThemeSettingsProvider.Settings.DefaultTheme)
                    foreach (var color in theme.Colors)
                        if (color.ColorId == colorId)
                            return color.Color;
            Debug.LogError($"Themes: ColorId <{colorId}> dont include in themes <{ThemeSettingsProvider.Settings.DefaultTheme}>");
            return Color.red;
        }
#endif
    }
}
#endif