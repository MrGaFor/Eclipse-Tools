using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Dialogue
{
    [HideMonoScript, CreateAssetMenu(fileName = "ActorConfig", menuName = "Config/Actor")]
    public class ActorConfig : ScriptableObject
    {
        private static ActorConfig _instance;
        public static ActorConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ActorConfig>("ActorConfig");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<ActorConfig>();
                        _instance.actors = new List<EC.Localization.LocalizationElement<string>> { new Localization.LocalizationElement<string>() };
                        _instance.actors[^1].SetValue("Actor_0", EC.Localization.LanguageConfig.Languages[0]);
                        string path = "Assets/Resources/ActorConfig.asset";
                        AssetDatabase.CreateAsset(_instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log("Created ActorConfig in Resources 'Actor_0'");
                    }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false, AddCopiesLastElement = true)] private List<EC.Localization.LocalizationElement<string>> actors = new List<EC.Localization.LocalizationElement<string>>();
        public static EC.Localization.LocalizationElement<string>[] Actors => Instance.actors.ToArray();
    }
}