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

        #region Registration
        public static async void Register<T>(T service) where T : GameService => await ForceRegisterAsync(service);
        public static async UniTask RegisterAsync<T>(T service) where T : GameService
        {
            if (Instance._services.ContainsKey(typeof(T))) return;
            await ForceRegisterAsync(service);
        }

        public static async void ForceRegister<T>(T service) where T : GameService => await ForceRegisterAsync(service);
        public static async UniTask ForceRegisterAsync<T>(T service) where T : GameService
        {
            Instance._services[typeof(T)] = service;
            service.OnCreate();
            await service.OnCreateAsync();
            if (Instance._waiters.TryGetValue(typeof(T), out var list))
            {
                foreach (var cb in list) cb(service);
                Instance._waiters.Remove(typeof(T));
            }
        }

        public static async void Unregister<T>() where T : GameService => await UnregisterAsync<T>();
        public static async UniTask UnregisterAsync<T>() where T : GameService
        {
            if (Instance._services.TryGetValue(typeof(T), out var service))
            {
                ((T)service).OnDispose();
                await ((T)service).OnDisposeAsync();
                Instance._services.Remove(typeof(T));
            }
        }
        #endregion

        #region Getting
        public static bool Has<T>() where T : GameService
        {
            return Instance._services.ContainsKey(typeof(T));
        }

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
        #endregion
    }
}
