#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EC.Editor
{
    public static class AddressablesIdCache
    {
        private static Dictionary<Type, string[]> _cachedAddresses = new();
        private static Dictionary<Type, int> _cachedCounts = new();

        public static string[] GetAddressesOfType<T>() where T : UnityEngine.Object
        {
            var type = typeof(T);

            if (_cachedAddresses.TryGetValue(type, out var cachedList) &&
                _cachedCounts.TryGetValue(type, out var cachedCount))
            {
                int currentCount = CountEntriesOfType<T>();
                if (currentCount == cachedCount)
                    return cachedList;
            }

            return ForceRefresh<T>();
        }

        public static string[] ForceRefresh<T>() where T : UnityEngine.Object
        {
            var type = typeof(T);
            var newList = new List<string>();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var allEntries = new List<AddressableAssetEntry>();

            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                group.GatherAllAssets(allEntries, true, true, false);
            }

            foreach (var entry in allEntries)
            {
                var path = AssetDatabase.GUIDToAssetPath(entry.guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    newList.Add(entry.address);
            }

            _cachedAddresses[type] = newList.ToArray();
            _cachedCounts[type] = newList.Count;

            return newList.ToArray();
        }

        private static int CountEntriesOfType<T>() where T : UnityEngine.Object
        {
            int count = 0;
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var allEntries = new List<AddressableAssetEntry>();

            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                group.GatherAllAssets(allEntries, true, true, false);
            }

            foreach (var entry in allEntries)
            {
                var path = AssetDatabase.GUIDToAssetPath(entry.guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    count++;
            }

            return count;
        }
    }
}
#endif
