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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void PreInit()
        {
            _instance = null;
        }

        /// <summary>
        /// Registers a service of type T. If there are any waiters for this type, they will be invoked with the service instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service) where T : SceneService
        {
            if (Instance._services.ContainsKey(typeof(T))) return;
            ForceRegister(service);
        }

        /// <summary>
        /// Forcefully registers a service of type T, replacing any existing service of the same type. If there are any waiters for this type, they will be invoked with the new service instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void ForceRegister<T>(T service) where T : SceneService
        {
            Instance._services[typeof(T)] = service;
            service.OnCreate();
            if (Instance._waiters.TryGetValue(typeof(T), out var list))
            {
                foreach (var cb in list) cb(service);
                Instance._waiters.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Unregisters the service of type T, if it exists. This will call OnDispose on the service before removing it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Unregister<T>() where T : SceneService
        {
            if (Instance._services.TryGetValue(typeof(T), out var service))
            {
                ((T)service).OnDispose();
                Instance._services.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Checks if a service of type T is registered in the locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Has<T>() where T : SceneService
        {
            return Instance._services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Tries to get a service of type T from the locator. Returns true if the service is found, false otherwise. The out parameter 'service' will contain the service instance if found, or default(T) if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool TryGet<T>(out T service) where T : SceneService
        {
            if (Instance._services.TryGetValue(typeof(T), out var s))
            {
                service = (T)s;
                return true;
            }
            service = default;
            return false;
        }

        /// <summary>
        /// Asynchronously tries to get a service of type T from the locator. If the service is already registered, it returns immediately with success=true and the service instance. If the service is not registered, it waits until either the service is registered (in which case it returns with success=true and the service instance) or the specified timeout elapses (in which case it returns with success=false and default(T)). The timeout is specified in seconds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<(bool success, T service)> TryGetAsync<T>(float timeoutSeconds) where T : SceneService
        {
            if (Instance._services.TryGetValue(typeof(T), out var existing))
                return (true, (T)existing);
            var tcs = new UniTaskCompletionSource<(bool, T)>();
            void callback(object s) => tcs.TrySetResult((true, (T)s));
            if (!Instance._waiters.TryGetValue(typeof(T), out var list))
                Instance._waiters[typeof(T)] = list = new List<Action<object>>();
            list.Add(callback);
            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var (firstCompleted, result) = await UniTask.WhenAny(tcs.Task, delayTask);
            if (firstCompleted)
                return result;
            list.Remove(callback);
            return (false, default);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                foreach (var service in Instance._services)
                    if (service.Value is SceneService ss)
                        ss.OnDispose();
        }
    }
}