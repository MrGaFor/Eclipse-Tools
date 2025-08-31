using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Dialogue
{
    [HideMonoScript, CreateAssetMenu(fileName = "TagConfig", menuName = "Config/Tag")]
    public class TagConfig : ScriptableObject
    {
        private static TagConfig _instance;
        public static TagConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TagConfig>("TagConfig");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<TagConfig>();
                        _instance.tags = new List<string> { "Tag_0" };
                        string path = "Assets/Resources/TagConfig.asset";
                        AssetDatabase.CreateAsset(_instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log("Created TagConfig in Resources 'Tag_0'");
                    }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)] private List<string> tags = new List<string>();
        public static string[] Tags => Instance.tags.ToArray();
    }
}