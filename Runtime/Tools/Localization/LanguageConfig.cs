using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Localization
{
    [HideMonoScript, CreateAssetMenu(fileName = "LanguageConfig", menuName = "Config/Language")]
    public class LanguageConfig : ScriptableObject
    {
        private static LanguageConfig _instance;
        public static LanguageConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<LanguageConfig>("LanguageConfig");
                    if (Application.isPlaying)
                        Debug.Log("Localization Loaded languages: " + _instance.languages.ToArray().ToString());
#if UNITY_EDITOR
                    if (_instance == null)
                {
                    _instance = CreateInstance<LanguageConfig>();
                    _instance.languages = new List<string> { "English" };
                    string path = "Assets/Resources/LanguageConfig.asset";
                    AssetDatabase.CreateAsset(_instance, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("Created LanguageConfig in Resources 'English'");
                }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)] private List<string> languages = new List<string>();
        public static string[] Languages => Instance.languages.ToArray();
    }
}