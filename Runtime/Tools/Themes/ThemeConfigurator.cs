#if EC_THEMES
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EC.Themes
{
    public class ThemeConfigurator : OdinEditorWindow
    {
        [MenuItem("Tools/EC/Modules/Themes")]
        private static void OpenWindow()
        {
            GetWindow<ThemeConfigurator>().Show();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Reset();
        }

        [Title("EC Themes Configurator")]


        [SerializeField, Title("Color Registry"), OnStateUpdate("UpdateOnlyIds"), ListDrawerSettings(DraggableItems = true, DefaultExpandedState = true, ShowFoldout = false)]
        private string[] colorIds;

        [SerializeField, Title("Default Theme"), ValueDropdown("ThemeIds")]
        private string defaultTheme;

        [ButtonGroup("ressave"), Button("Reset")]
        private void Reset()
        {
            colorIds = (string[])ThemeSettingsProvider.Settings.ColorIds.Clone();
            defaultTheme = ThemeSettingsProvider.Settings.DefaultTheme;
            themes = CopyThemes(ThemeSettingsProvider.Settings.Themes);
        }

        [ButtonGroup("ressave"), Button("Save")]
        private void Save()
        {
            ThemeSettingsProvider.Settings.ColorIds = (string[])colorIds.Clone();
            ThemeSettingsProvider.Settings.DefaultTheme = defaultTheme;
            ThemeSettingsProvider.Settings.Themes = CopyThemes(themes);
            EditorUtility.SetDirty(ThemeSettingsProvider.Settings);
            AssetDatabase.SaveAssets();
        }

        [Button("Update Colors")]
        private void UpdateColors()
        {
            UpdateOnlyColors();
            UpdateOnlyIds();
            UpdateDefaultTheme();
        }
        
        [SerializeField, Title("Themes"), OnStateUpdate("UpdateDefaultTheme"), ListDrawerSettings(AddCopiesLastElement = true, DefaultExpandedState = true, DraggableItems = false, ShowFoldout = false)]
        private ThemeVariant[] themes;

        #region Utils
        private void UpdateDefaultTheme()
        {
            if ((string.IsNullOrEmpty(defaultTheme) || !ThemeIds.Contains(defaultTheme)) && themes.Length > 0)
                defaultTheme = themes[0].ThemeId;
        }
        private void UpdateOnlyColors()
        {
            for (int themeNo = 0; themeNo < themes.Length; themeNo++)
                if (themes[themeNo].Colors.Length != colorIds.Length)
                {
                    ThemeColorVariant[] newColors = new ThemeColorVariant[colorIds.Length];
                    for (int colorNo = 0; colorNo < Mathf.Min(themes[themeNo].Colors.Length, colorIds.Length); colorNo++)
                        newColors[colorNo] = themes[themeNo].Colors[colorNo];
                    if (colorIds.Length > themes[themeNo].Colors.Length)
                        for (int colorNo = themes[themeNo].Colors.Length; colorNo < colorIds.Length; colorNo++)
                            newColors[colorNo] = new ThemeColorVariant();
                    themes[themeNo].Colors = newColors;
                }
        }
        private void UpdateOnlyIds()
        {
            for (int themeNo = 0; themeNo < themes.Length; themeNo++)
                for (int colorNo = 0; colorNo < Mathf.Min(themes[themeNo].Colors.Length, colorIds.Length); colorNo++)
                    if (themes[themeNo].Colors[colorNo].ColorId != colorIds[colorNo])
                        themes[themeNo].Colors[colorNo].ColorId = colorIds[colorNo];
        }
        private ThemeVariant[] CopyThemes(ThemeVariant[] themes)
        {
            ThemeVariant[] newVariant = new ThemeVariant[themes.Length];
            for (int i = 0; i < themes.Length; i++)
            {
                newVariant[i] = new ThemeVariant();
                newVariant[i].ThemeId = themes[i].ThemeId;
                newVariant[i].Colors = new ThemeColorVariant[themes[i].Colors.Length];
                for (int j = 0; j < themes[i].Colors.Length; j++)
                {
                    newVariant[i].Colors[j] = new ThemeColorVariant();
                    newVariant[i].Colors[j].ColorId = themes[i].Colors[j].ColorId;
                    newVariant[i].Colors[j].Color = themes[i].Colors[j].Color;
                }
            }
            return newVariant;
        }
        private string[] ThemeIds => ThemeSettingsProvider.Settings.ThemeIds;
        #endregion

    }
}
#endif
#endif