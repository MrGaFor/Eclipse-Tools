using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace EC.Services
{
    public class SceneLocator : MonoBehaviour
    {
        private static SceneLocator _instance;
        private readonly Dictionary<Type, SceneService> _services = new();
        private readonly Dictionary<Type, List<Action<SceneService>>> _waiters = new();

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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void PreInit()
        {
            _instance = null;
        }
        private void OnDestroy()
        {
            if (_instance != this) return;
            foreach (var service in _services.Values)
            {
                service.OnDispose();
                service.OnDisposeAsync().Forget();
            }
        }

        #region Registration
        public static UniTask Register<T>(T service) where T : SceneService
        {
            return RegisterInternal(service, false);
        }

        public static UniTask ForceRegister<T>(T service) where T : SceneService
        {
            return RegisterInternal(service, true);
        }
        private static async UniTask RegisterInternal(SceneService service, bool force)
        {
            var type = service.GetType();
            if (!force && Instance._services.ContainsKey(type))
                return;
            Instance._services[type] = service;
            service.OnCreate();
            await service.OnCreateAsync();
            if (Instance._waiters.TryGetValue(type, out var list))
            {
                foreach (var cb in list)
                    cb(service);

                Instance._waiters.Remove(type);
            }
        }

        public static UniTask Unregister<T>() where T : SceneService
        {
            var type = typeof(T);

            if (Instance._services.TryGetValue(type, out var service))
            {
                Instance._services.Remove(type);
                service.OnDispose();
                return service.OnDisposeAsync();
            }

            return UniTask.CompletedTask;
        }
        #endregion

        #region Getting
        public static bool Has<T>() where T : SceneService
        {
            return Instance._services.ContainsKey(typeof(T));
        }

        public static bool TryGet<T>(out T service) where T : SceneService
        {
            if (Instance._services.TryGetValue(typeof(T), out var s))
            {
                service = (T)s;
                return true;
            }
            service = null;
            return false;
        }

        public static async UniTask<(bool success, T service)> TryGetAsync<T>(float timeoutSeconds = 1)
            where T : SceneService
        {
            var type = typeof(T);
            if (Instance._services.TryGetValue(type, out var existing))
                return (true, (T)existing);
            var tcs = new UniTaskCompletionSource<(bool, T)>();
            void Callback(SceneService s)
            {
                tcs.TrySetResult((true, (T)s));
            }
            if (!Instance._waiters.TryGetValue(type, out var list))
            {
                list = new List<Action<SceneService>>();
                Instance._waiters[type] = list;
            }
            list.Add(Callback);
            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var (firstCompleted, result) = await UniTask.WhenAny(tcs.Task, delayTask);
            if (firstCompleted)
                return result;
            list.Remove(Callback);
            return (false, null);
        }
        #endregion
    }
}