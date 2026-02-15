using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Services
{
    public class GameLocator
    {
        private static GameLocator _instance;
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, List<Action<object>>> _waiters = new();

        private static GameLocator Instance => _instance ??= new GameLocator();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void PreInit()
        {
            _instance = null;
        }

        /// <summary>
        /// Registers a service of type T. If a service of that type is already registered, this method does nothing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service) where T : GameService
        {
            if (Instance._services.ContainsKey(typeof(T))) return;
            ForceRegister(service);
        }

        /// <summary>
        /// Registers a service of type T, replacing any existing service of that type. Use with caution, as this will call OnCreate on the new service and OnDispose on the old one if it exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void ForceRegister<T>(T service) where T : GameService
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
        public static void Unregister<T>() where T : GameService
        {
            if (Instance._services.TryGetValue(typeof(T), out var service))
            {
                ((T)service).OnDispose();
                Instance._services.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Checks if a service of type T is registered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Has<T>() where T : GameService
        {
            return Instance._services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Tries to get the service of type T. Returns true if the service exists, false otherwise. The out parameter will be set to the service if it exists, or default if it does not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool TryGet<T>(out T service) where T : GameService
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
        /// Tries to get the service of type T, waiting up to timeoutSeconds if it does not exist. Returns a tuple where the first item is true if the service was obtained, false if the timeout was reached, and the second item is the service if it was obtained or default if it was not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async UniTask<(bool success, T service)> TryGetAsync<T>(float timeoutSeconds) where T : GameService
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
    }
}
