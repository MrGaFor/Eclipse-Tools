using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace EC.Saver
{
    public enum SaverType { Int, Float, Bool, String, Vector2, Vector3, Quaternion, Color, Gradient, KeyCode, }

    public static class SaverSystem
    {
        private static Dictionary<string, object> ActiveVariables = new();

        public static void AddVariable<T>(string key, T defaultValue = default)
        {
            if (ActiveVariables.ContainsKey(key)) return;
            ActiveVariables.Add(key, defaultValue);
            Bus.BusSystem.Set<T>(key, FromString<T>(PlayerPrefs.GetString(key, ToString<T>(defaultValue))));
            Bus.BusSystem.Subscribe<T>(key, (value) => OnChangeVariable(key, value));
        }
        public static void RemoveVariable(string key)
        {
            if (!ActiveVariables.ContainsKey(key)) return;
            ActiveVariables.Remove(key);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        public static void ClearAllVariables()
        {
            foreach (var key in ActiveVariables)
                UnsubscribeByKey(key);
            ActiveVariables.Clear();
        }
        private static void UnsubscribeByKey<T>(KeyValuePair<string, T> key)
        {
            Bus.BusSystem.Unsubscribe(key.Key, (Action<T>)((value) => OnChangeVariable(key.Key, value)));
        }
        public static void ClearAndDeleteAllVariables()
        {
            foreach (var key in ActiveVariables)
            {
                UnsubscribeByKey(key);
                PlayerPrefs.DeleteKey(key.Key);
            }
            PlayerPrefs.Save();
            ActiveVariables.Clear();
        }
        private static void OnChangeVariable<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, ToString(value));
            PlayerPrefs.Save();
        }

        #region PARSE & TOSTRING
        private static string ToString<T>(T value) => value switch
        {
            int i => i.ToString(CultureInfo.InvariantCulture),
            float f => f.ToString(CultureInfo.InvariantCulture),
            bool b => b.ToString(),
            string s => s,
            Enum e => e.ToString(),
            Vector2 v2 => $"{v2.x.ToString(CultureInfo.InvariantCulture)},{v2.y.ToString(CultureInfo.InvariantCulture)}",
            Vector3 v3 => $"{v3.x.ToString(CultureInfo.InvariantCulture)},{v3.y.ToString(CultureInfo.InvariantCulture)},{v3.z.ToString(CultureInfo.InvariantCulture)}",
            Quaternion q => $"{q.x.ToString(CultureInfo.InvariantCulture)},{q.y.ToString(CultureInfo.InvariantCulture)},{q.z.ToString(CultureInfo.InvariantCulture)},{q.w.ToString(CultureInfo.InvariantCulture)}",
            Color c => $"{c.r.ToString(CultureInfo.InvariantCulture)},{c.g.ToString(CultureInfo.InvariantCulture)},{c.b.ToString(CultureInfo.InvariantCulture)},{c.a.ToString(CultureInfo.InvariantCulture)}",
            Gradient g => JsonUtility.ToJson(g),
            _ => typeof(T).IsSerializable ? JsonUtility.ToJson(value) : throw new NotSupportedException($"Unsupported type: {typeof(T)}")
        };
        private static T FromString<T>(string str)
        {
            Type t = typeof(T);
            return (t switch
            {
                Type _ when t == typeof(int) => int.Parse(str, CultureInfo.InvariantCulture),
                Type _ when t == typeof(float) => float.Parse(str, CultureInfo.InvariantCulture),
                Type _ when t == typeof(bool) => bool.Parse(str),
                Type _ when t == typeof(string) => str,
                Type _ when t == typeof(Vector2) => ParseVec2(str),
                Type _ when t == typeof(Vector3) => ParseVec3(str),
                Type _ when t == typeof(Quaternion) => ParseQuat(str),
                Type _ when t == typeof(Color) => ParseColor(str),
                Type _ when t == typeof(Gradient) => JsonUtility.FromJson<Gradient>(str),
                Type _ when t.IsEnum => Enum.Parse(t, str),
                Type _ when t.IsSerializable => JsonUtility.FromJson<T>(str),
                _ => throw new NotSupportedException($"Unsupported type: {t}")
            }) is T result ? result : default;
        }

        private static Vector2 ParseVec2(string s) { var p = s.Split(','); return new(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture)); }
        private static Vector3 ParseVec3(string s) { var p = s.Split(','); return new(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture)); }
        private static Quaternion ParseQuat(string s) { var p = s.Split(','); return new(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture), float.Parse(p[3], CultureInfo.InvariantCulture)); }
        private static Color ParseColor(string s) { var p = s.Split(','); return new(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture), float.Parse(p[3], CultureInfo.InvariantCulture)); }
        #endregion
    }


}
