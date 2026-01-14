#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ColliderGizmosSettingsProvider
{
    private const string AssetName = "ColliderGizmosSettings.asset";
    private const string FolderPath = "Assets/Editor/Settings";

    private static ColliderGizmosSettings _settings;

    public static ColliderGizmosSettings Settings
    {
        get
        {
            if (_settings == null)
                _settings = LoadOrCreate();
            return _settings;
        }
    }

    private static ColliderGizmosSettings LoadOrCreate()
    {
        var assetPath = Path.Combine(FolderPath, AssetName);
        var settings = AssetDatabase.LoadAssetAtPath<ColliderGizmosSettings>(assetPath);
        if (settings != null)
            return settings;
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
            AssetDatabase.Refresh();
        }
        settings = ScriptableObject.CreateInstance<ColliderGizmosSettings>();
        AssetDatabase.CreateAsset(settings, assetPath);
        AssetDatabase.SaveAssets();
        return settings;
    }
}
#endif
