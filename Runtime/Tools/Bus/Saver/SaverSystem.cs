using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Globalization.CultureInfo;

namespace EC.Saver
{
    public enum SaverType { Int, Float, Bool, String, Vector2, Vector3, Quaternion, Color, Gradient, KeyCode, }

    public static class SaverSystem
    {
        private static List<string> ActiveVariables = new();

        public static void AddVariable<T>(string key, T defaultValue = default)
        {
            if (ActiveVariables.Contains(key)) return;
            ActiveVariables.Add(key);
            Bus.BusSystem.Set<T>(key, FromString<T>(PlayerPrefs.GetString(key, ToString<T>(defaultValue))));
            Bus.BusSystem.Subscribe<T>(key, (value) => OnChangeVariable(key, value));
        }
        public static void RemoveVariable(string key)
        {
            if (!ActiveVariables.Contains(key)) return;
            ActiveVariables.Remove(key);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        private static void OnChangeVariable<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, ToString(value));
            PlayerPrefs.Save();
        }

        #region PARSE & TOSTRING
        private static string ToString<T>(T value) => value switch
        {
            int i => i.ToString(InvariantCulture),
            float f => f.ToString(InvariantCulture),
            bool b => b.ToString(),
            string s => s,
            Enum e => e.ToString(),
            Vector2 v2 => $"{v2.x.ToString(InvariantCulture)},{v2.y.ToString(InvariantCulture)}",
            Vector3 v3 => $"{v3.x.ToString(InvariantCulture)},{v3.y.ToString(InvariantCulture)},{v3.z.ToString(InvariantCulture)}",
            Quaternion q => $"{q.x.ToString(InvariantCulture)},{q.y.ToString(InvariantCulture)},{q.z.ToString(InvariantCulture)},{q.w.ToString(InvariantCulture)}",
            Color c => $"{c.r.ToString(InvariantCulture)},{c.g.ToString(InvariantCulture)},{c.b.ToString(InvariantCulture)},{c.a.ToString(InvariantCulture)}",
            Gradient g => JsonUtility.ToJson(g),
            _ => throw new NotSupportedException($"Unsupported type: {typeof(T)}")
        };
        private static T FromString<T>(string str)
        {
            Type t = typeof(T);
            return (t switch
            {
                Type _ when t == typeof(int) => int.Parse(str, InvariantCulture),
                Type _ when t == typeof(float) => float.Parse(str, InvariantCulture),
                Type _ when t == typeof(bool) => bool.Parse(str),
                Type _ when t == typeof(string) => str,
                Type _ when t == typeof(Vector2) => ParseVec2(str),
                Type _ when t == typeof(Vector3) => ParseVec3(str),
                Type _ when t == typeof(Quaternion) => ParseQuat(str),
                Type _ when t == typeof(Color) => ParseColor(str),
                Type _ when t == typeof(Gradient) => JsonUtility.FromJson<Gradient>(str),
                Type _ when t.IsEnum => Enum.Parse(t, str),
                _ => throw new NotSupportedException($"Unsupported type: {t}")
            }) is T result ? result : default;
        }

        private static Vector2 ParseVec2(string s) { var p = s.Split(','); return new(float.Parse(p[0], InvariantCulture), float.Parse(p[1], InvariantCulture)); }
        private static Vector3 ParseVec3(string s) { var p = s.Split(','); return new(float.Parse(p[0], InvariantCulture), float.Parse(p[1], InvariantCulture), float.Parse(p[2], InvariantCulture)); }
        private static Quaternion ParseQuat(string s) { var p = s.Split(','); return new(float.Parse(p[0], InvariantCulture), float.Parse(p[1], InvariantCulture), float.Parse(p[2], InvariantCulture), float.Parse(p[3], InvariantCulture)); }
        private static Color ParseColor(string s) { var p = s.Split(','); return new(float.Parse(p[0], InvariantCulture), float.Parse(p[1], InvariantCulture), float.Parse(p[2], InvariantCulture), float.Parse(p[3], InvariantCulture)); }
        #endregion
    }


}
