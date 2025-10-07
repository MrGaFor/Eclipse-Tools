using System.Collections.Generic;
using System;

namespace EC.Bus
{
    public static class BusSystem
    {
        private static Dictionary<string, object> Variables = new Dictionary<string, object>();
        private static Dictionary<string, List<object>> Subscribers = new Dictionary<string, List<object>>();

        #region --- SUBSCRIBE ---
        public static void Subscribe(string key, Action action) { AddSubscriber(key, action); }
        public static void Subscribe<T>(string key, Action<T> action) { AddSubscriber(key, action); }

        private static void AddSubscriber(string key, object action)
        {
            if (Subscribers.ContainsKey(key)) Subscribers[key].Add(action);
            else Subscribers.Add(key, new List<object>() { action });
        }
        #endregion

        #region --- UNSUBSCRIBE ---
        public static void Unsubscribe(string key, Action action) { RemoveSubscriber(key, action); }
        public static void Unsubscribe<T>(string key, Action<T> action) { RemoveSubscriber(key, action); }

        private static void RemoveSubscriber(string key, object action)
        {
            if (Subscribers.ContainsKey(key))
            {
                Subscribers[key].Remove(action);
                if (Subscribers[key].Count == 0)
                    Subscribers.Remove(key);
            }
        }
        #endregion

        #region --- GET ---
        public static T Get<T>(string key, T defValue = default)
            => Variables.TryGetValue(key, out var value)
            ? (T)value : defValue;
        #endregion

        #region --- SET ---
        public static void Set<T>(string key, T arg)
        {
            Variables[key] = arg;
            BusKeysData.RegisterKeyType(key, typeof(T));
            Invoke(key, arg);
        }
        #endregion

        #region --- INVOKE ---
        public static void Invoke(string key)
        {
            BusKeysData.RegisterKeyType(key, "Simple");
            if (Subscribers.ContainsKey(key))
                if (Subscribers[key].Count > 0)
                    for (int i = Subscribers[key].Count - 1; i >= 0; i--)
                        (Subscribers[key][i] as Action)?.Invoke();
        }
        public static void Invoke<T>(string key, T arg)
        {
            BusKeysData.RegisterKeyType(key, typeof(T));
            if (Subscribers.ContainsKey(key))
                if (Subscribers[key].Count > 0)
                    for (int i = Subscribers[key].Count - 1; i >= 0; i--)
                        (Subscribers[key][i] as Action<T>)?.Invoke(arg);
        }
        #endregion

        #region --- HASKEY ---
        public static bool HasKey(string key)
        {
            return Variables.ContainsKey(key);
        }
        #endregion

        #region --- EVENT ---
        public static void CallEvent(IBusInSettings settings)
        {
            Invoke(settings.GetKey());
        }
        public static void CallEvent<T>(IBusInSettings settings)
        {
            if (settings.GetEventType() == EventTypes.Set)
                Set<T>(settings.GetKey(), (T)settings.GetValue());
            else if (settings.GetEventType() == EventTypes.Invoke)
                Invoke<T>(settings.GetKey(), (T)settings.GetValue());
        }
        #endregion

        #region --- REMOVE-SUBSCRIBERS ---
        public static void RemoveSubscribers(string key)
        {
            if (Subscribers.ContainsKey(key))
                Subscribers.Remove(key);
        }
        #endregion

        #region --- REMOVE-VARIABLE ---
        public static void RemoveVariable(string key)
        {
            if (Variables.ContainsKey(key))
                Variables.Remove(key);
        }
        #endregion

        #region --- EDITOR ---
#if UNITY_EDITOR
        public static Dictionary<string, object> GetAllVariables()
        {
            return new Dictionary<string, object>(Variables);
        }
#endif
        #endregion
    }
}