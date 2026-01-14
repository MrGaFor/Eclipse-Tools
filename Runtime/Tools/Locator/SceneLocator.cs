using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace EC.Services
{
    public class SceneLocator : MonoBehaviour
    {
        private static SceneLocator _instance;
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, List<Action<object>>> _waiters = new();

        private static SceneLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("SSLocator");
                    _instance = go.AddComponent<SceneLocator>();
                }
                return _instance;
            }
        }

        public static void Register<T>(T service)
        {
            var inst = Instance;
            var type = typeof(T);
            inst._services[type] = service;

            if (inst._waiters.TryGetValue(type, out var list))
            {
                foreach (var a in list) a(service);
                inst._waiters.Remove(type);
            }
        }
        public static void ForceRegister<T>(T service)
        {
            var inst = Instance;
            inst._services[typeof(T)] = service;
            if (inst._waiters.TryGetValue(typeof(T), out var list))
            {
                foreach (var cb in list) cb(service);
                inst._waiters.Remove(typeof(T));
            }
        }
        public static bool Has<T>()
        {
            return Instance._services.ContainsKey(typeof(T));
        }
        public static bool TryGet<T>(out T service)
        {
            var inst = Instance;
            if (inst._services.TryGetValue(typeof(T), out var s))
            {
                service = (T)s;
                return true;
            }
            service = default;
            return false;
        }

        public static async UniTask<(bool success, T service)> TryGetAsync<T>(float timeoutSeconds)
        {
            var inst = Instance;
            if (inst._services.TryGetValue(typeof(T), out var existing))
                return (true, (T)existing);

            var tcs = new UniTaskCompletionSource<(bool, T)>();
            void callback(object s) => tcs.TrySetResult((true, (T)s));

            var type = typeof(T);
            if (!inst._waiters.TryGetValue(type, out var list))
                inst._waiters[type] = list = new List<Action<object>>();

            list.Add(callback);

            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var (firstCompleted, result) = await UniTask.WhenAny(tcs.Task, delayTask);

            if (firstCompleted)
                return result;

            list.Remove(callback);
            return (false, default);
        }
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}