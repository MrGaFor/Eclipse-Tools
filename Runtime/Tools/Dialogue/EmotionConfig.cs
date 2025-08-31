using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Dialogue
{
    [HideMonoScript, CreateAssetMenu(fileName = "EmotionConfig", menuName = "Config/Emotion")]
    public class EmotionConfig : ScriptableObject
    {
        private static EmotionConfig _instance;
        public static EmotionConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<EmotionConfig>("EmotionConfig");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<EmotionConfig>();
                        _instance.emotions = new List<string> { "Emotion_0" };
                        string path = "Assets/Resources/EmotionConfig.asset";
                        AssetDatabase.CreateAsset(_instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log("Created EmotionConfig in Resources 'Emotion_0'");
                    }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)] private List<string> emotions = new List<string>();
        public static string[] Emotions => Instance.emotions.ToArray();
    }
}