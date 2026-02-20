#if EC_THEMES
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace EC.Themes
{
    public static class ThemeSettingsProvider
    {
        private const string AssetName = "ECThemes";
        private const string FolderPath = "Assets/Resources";

        private static ThemeSettings _settings;

        public static ThemeSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = LoadSettings();
                return _settings;
            }
        }

        private static ThemeSettings LoadSettings()
        {
#if UNITY_EDITOR
            var assetPath = Path.Combine(FolderPath, AssetName + ".asset");
            var settings = AssetDatabase.LoadAssetAtPath<ThemeSettings>(assetPath);

            if (settings != null)
                return settings;

            return CreateAsset(assetPath);
#else
            return Resources.Load<ThemeSettings>(AssetName);
#endif
        }

#if UNITY_EDITOR
        private static ThemeSettings CreateAsset(string assetPath)
        {
            if (!AssetDatabase.IsValidFolder(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                AssetDatabase.Refresh();
            }

            var settings = ScriptableObject.CreateInstance<ThemeSettings>();
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();

            return settings;
        }
#endif
    }
}
#endif