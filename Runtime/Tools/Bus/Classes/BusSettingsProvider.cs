using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Bus
{
    public static class BusSettingsProvider
    {
#if !UNITY_EDITOR
        public static BusSettings Settings => null;
#else
        private const string AssetName = "BusSettings.asset";
        private const string FolderPath = "Assets/Editor/Settings";

        private static BusSettings _settings;

        public static BusSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = LoadOrCreate();
                return _settings;
            }
        }

        private static BusSettings LoadOrCreate()
        {
            var assetPath = Path.Combine(FolderPath, AssetName);
            var settings = AssetDatabase.LoadAssetAtPath<BusSettings>(assetPath);
            if (settings != null)
                return settings;
            if (!AssetDatabase.IsValidFolder(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                AssetDatabase.Refresh();
            }
            settings = ScriptableObject.CreateInstance<BusSettings>();
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
#endif
    }
}
