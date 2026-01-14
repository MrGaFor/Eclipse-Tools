using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public sealed class BusSettings : ScriptableObject
    {
        [SerializeField, ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)]
        private List<KeyWithType> _keys = new();

        [Serializable]
        public sealed class KeyWithType
        {
            [HorizontalGroup, LabelWidth(70)] public string Key;
            [HorizontalGroup, LabelWidth(70), ReadOnly] public string Type;

            public KeyWithType(string key)
            {
                Key = key;
            }
        }

        public IReadOnlyList<KeyWithType> Keys => _keys;

        public string[] GetKeys()
        {
            string[] keys = new string[_keys.Count];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = _keys[i].Key;
            return keys;
        }

        public KeyWithType[] GetKeysWithType()
        {
            return _keys.ToArray();
        }

        public bool TryAddKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || FindIndex(key) >= 0)
                return false;
            _keys.Add(new KeyWithType(key));
            SaveSettings();
            return true;
        }

        public bool TryRemoveKey(string key)
        {
            int index = FindIndex(key);
            if (index < 0)
                return false;
            _keys.RemoveAt(index);
            SaveSettings();
            return true;
        }

        public bool TryRegisterType(string key, Type type)
        {
            if (type == null)
                return false;
            return TryRegisterType(key, type.Name);
        }

        public bool TryRegisterType(string key, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return false;
            int index = FindIndex(key);
            if (index < 0)
                return false;
            _keys[index].Type = typeName;
            SaveSettings();
            return true;
        }

        private int FindIndex(string key)
        {
            for (int i = 0; i < _keys.Count; i++)
                if (_keys[i].Key == key)
                    return i;
            return -1;
        }

        private void SaveSettings()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}
