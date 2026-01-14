using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Bus
{
    public static class BusSystem
    {
        private static readonly Dictionary<string, object> Variables = new();
        private static readonly Dictionary<string, List<object>> Subscribers = new();

        #region --- INIT ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Variables.Clear();
            Subscribers.Clear();
        }
        #endregion

        #region --- VALIDATION ---
        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Bus key is null or empty");
        }
        #endregion

        #region --- SUBSCRIBES ---
        public static void Subscribe(string key, Action action)
        {
            ValidateKey(key);
            AddSubscriber(key, action);
        }
        public static void Subscribe<T>(string key, Action<T> action)
        {
            ValidateKey(key);
            AddSubscriber(key, action);
        }

        private static void AddSubscriber(string key, object action)
        {
            if (!Subscribers.TryGetValue(key, out var list))
                Subscribers[key] = list = new List<object>();
            list.Add(action);
        }
        public static void Unsubscribe(string key, Action action)
        {
            ValidateKey(key);
            RemoveSubscriber(key, action);
        }
        public static void Unsubscribe<T>(string key, Action<T> action)
        {
            ValidateKey(key);
            RemoveSubscriber(key, action);
        }
        private static void RemoveSubscriber(string key, object action)
        {
            if (!Subscribers.TryGetValue(key, out var list)) return;
            list.Remove(action);
            if (list.Count == 0) Subscribers.Remove(key);
        }
        #endregion

        #region --- GETTERS ---
        public static bool HasKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            return Variables.ContainsKey(key);
        }
        public static T Get<T>(string key, T defValue = default)
        {
            ValidateKey(key);
            if (!Variables.TryGetValue(key, out var value)) return defValue;
            if (value is T t) return t;
            try
            {
                if (value is IConvertible)
                    return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception e)
            {
                Debug.LogError($"[Bus:Get] Key='{key}' Stored='{value.GetType().Name}' Requested='{typeof(T).Name}' Error='{e.Message}'");
            }
            throw new InvalidCastException($"[Bus:Get] Key='{key}' Stored='{value.GetType().Name}' Requested='{typeof(T).Name}'");
        }
        #endregion

        #region --- SETTERS ---
        public static void Set<T>(string key, T arg)
        {
            ValidateKey(key);
            Variables[key] = arg;
#if UNITY_EDITOR
            BusSettingsProvider.Settings.TryRegisterType(key, typeof(T));
#endif
            InvokeInternal(key, arg);
        }
        public static void Invoke(string key)
        {
            ValidateKey(key);
#if UNITY_EDITOR
            BusSettingsProvider.Settings.TryRegisterType(key, "Simple");
#endif
            if (!Subscribers.TryGetValue(key, out var list)) return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is Action a) a.Invoke();
                else Debug.LogError($"[Bus:Invoke] Key='{key}' SubscriberType='{list[i].GetType().Name}' Expected='Action'");
            }
        }

        public static void Invoke<T>(string key, T arg)
        {
            ValidateKey(key);
#if UNITY_EDITOR
            BusSettingsProvider.Settings.TryRegisterType(key, typeof(T));
#endif
            InvokeInternal(key, arg);
        }

        private static void InvokeInternal<T>(string key, T arg)
        {
            if (!Subscribers.TryGetValue(key, out var list)) return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is Action<T> a) a.Invoke(arg);
                else Debug.LogError($"[Bus:Invoke] Key='{key}' SubscriberType='{list[i].GetType().Name}' Expected='Action<{typeof(T).Name}>'");
            }
        }
        #endregion

        #region --- REMOVE ---
        public static void RemoveSubscribers(string key) => Subscribers.Remove(key);
        public static void RemoveVariable(string key) => Variables.Remove(key);
        #endregion

        #region --- EDITOR ONLY ---
#if UNITY_EDITOR
        public static IReadOnlyDictionary<string, object> GetAllVariables() => new Dictionary<string, object>(Variables);
#endif
        #endregion
    }

    public static class BusSettingsExecutor
    {
        public static readonly Dictionary<BusSettingsType, Action<IBusInSettings>> Execute = new()
        {
            { BusSettingsType.Int, data => Call<int>(data) },
            { BusSettingsType.Float, data => Call<float>(data) },
            { BusSettingsType.Bool, data => Call<bool>(data) },
            { BusSettingsType.String, data => Call<string>(data) },
            { BusSettingsType.Char, data => Call<char>(data) },
            { BusSettingsType.Vector2, data => Call<Vector2>(data) },
            { BusSettingsType.Vector2Int, data => Call<Vector2Int>(data) },
            { BusSettingsType.Vector3, data => Call<Vector3>(data) },
            { BusSettingsType.Vector3Int, data => Call<Vector3Int>(data) },
            { BusSettingsType.Quaternion, data => Call<Quaternion>(data) },
            { BusSettingsType.Color, data => Call<Color>(data) },
            { BusSettingsType.Gradient, data => Call<Gradient>(data) },
            { BusSettingsType.KeyCode, data => Call<KeyCode>(data) },
            { BusSettingsType.Transform, data => Call<Transform>(data) },
            { BusSettingsType.GameObject, data => Call<GameObject>(data) },
            { BusSettingsType.Camera, data => Call<Camera>(data) },
            { BusSettingsType.Object, data => Call<UnityEngine.Object>(data) },
        };

        public static void CallSafe<T>(IBusInSettings data)
        {
            if (data == null) return;
            var value = data.GetValue();
            if (value != null && value is not T)
            {
                Debug.LogError($"[BusExecutor] Key='{data.GetKey()}' Stored='{value.GetType().Name}' Expected='{typeof(T).Name}'");
                return;
            }

            if (data.GetEventType() == EventTypes.Set)
                BusSystem.Set<T>(data.GetKey(), value == null ? default : (T)value);
            else
                BusSystem.Invoke<T>(data.GetKey(), value == null ? default : (T)value);
        }
        private static void Call<T>(IBusInSettings data)
        {
            var value = data.GetValue();
            if (value == null)
            {
                if (data.GetEventType() == EventTypes.Set)
                    BusSystem.Set<T>(data.GetKey(), default);
                else
                    BusSystem.Invoke<T>(data.GetKey(), default);
                return;
            }
            if (value is not T)
            {
                Debug.LogError($"[BusExecutor] Key='{data.GetKey()}' Stored='{value.GetType().Name}' Expected='{typeof(T).Name}'");
                return;
            }
            if (data.GetEventType() == EventTypes.Set)
                BusSystem.Set(data.GetKey(), (T)value);
            else
                BusSystem.Invoke(data.GetKey(), (T)value);
        }
    }
}
