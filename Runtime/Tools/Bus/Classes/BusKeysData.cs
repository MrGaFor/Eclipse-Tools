using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public class BusKeysData : ScriptableObject
    {
        private static BusKeysData _instance;
        public static BusKeysData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<BusKeysData>("BusKeys");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<BusKeysData>();
                        _instance._keys = new KeyWithType[] { };
                        string path = "Assets/Resources/BusKeys.asset";
                        UnityEditor.AssetDatabase.CreateAsset(_instance, path);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                        Debug.Log("Created BusKeys in Resources");
                    }
#endif
                }
                return _instance;
            }
        }

        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)] private KeyWithType[] _keys;
        [System.Serializable]
        public class KeyWithType
        {
            [HorizontalGroup(), LabelWidth(70)] public string Key;
            [HorizontalGroup(), LabelWidth(70), ReadOnly] public string Type;

            public KeyWithType(string key)
            {
                Key = key;
            }
        }

        public static string[] GetKeys()
        {
            string[] keys = new string[Instance._keys.Length];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = Instance._keys[i].Key;
            return keys;
        }
        public static KeyWithType[] GetKeysWithType()
        {
            return Instance._keys;
        }
        public static void RegisterKeyType(string key, string type)
        {
            if (!GetKeys().Contains(key)) return;
            foreach (var k in Instance._keys)
                if (k.Key == key)
                    k.Type = type;
        }
        public static void RegisterKeyType(string key, System.Type type)
        {
            if (!GetKeys().Contains(key)) return;
            foreach (var k in Instance._keys)
                if (k.Key == key)
                    k.Type = type.Name;
        }
        public static void TryAddKey(string key)
        {
            if (GetKeys().Contains(key))
            {
                Debug.Log($"Key {'"'}{key}{'"'} already added.");
                return;
            }
            List<KeyWithType> keys = Instance._keys.ToList();
            keys.Add(new KeyWithType(key));
            Instance._keys = keys.ToArray();
        }
        public static void TryRemoveKey(string key)
        {
            if (!GetKeys().Contains(key))
            {
                Debug.Log($"Key {'"'}{key}{'"'} dont have.");
                return;
            }
            List<KeyWithType> keys = Instance._keys.ToList();
            if (keys.Count > 0)
                for (int i = 0; i < keys.Count; i++)
                    if (keys[i].Key == key)
                    {
                        keys.RemoveAt(i);
                        break;
                    }
            Instance._keys = keys.ToArray();
        }
    }
}
