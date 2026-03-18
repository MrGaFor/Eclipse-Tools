using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Services
{
    public class GameLocator
    {
        private static GameLocator _instance;

        private readonly Dictionary<Type, GameService> _services = new();
        private readonly Dictionary<Type, List<Action<GameService>>> _waiters = new();

        private static GameLocator Instance => _instance ??= new GameLocator();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void PreInit()
        {
            _instance = null;
        }

        #region Registration
        public static UniTask Register<T>(T service) where T : GameService
        {
            return RegisterInternal(service, false);
        }

        public static UniTask ForceRegister<T>(T service) where T : GameService
        {
            return RegisterInternal(service, true);
        }

        private static async UniTask RegisterInternal(GameService service, bool force)
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

        public static UniTask Unregister<T>() where T : GameService
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
            service = null;
            return false;
        }

        public static async UniTask<(bool success, T service)> TryGetAsync<T>(float timeoutSeconds = 1)
            where T : GameService
        {
            var type = typeof(T);
            if (Instance._services.TryGetValue(type, out var existing))
                return (true, (T)existing);
            var tcs = new UniTaskCompletionSource<(bool, T)>();
            void Callback(GameService s)
            {
                tcs.TrySetResult((true, (T)s));
            }
            if (!Instance._waiters.TryGetValue(type, out var list))
            {
                list = new List<Action<GameService>>();
                Instance._waiters[type] = list;
            }
            list.Add(Callback);
            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var (completedIndex, result) = await UniTask.WhenAny(tcs.Task, delayTask);
            if (completedIndex)
                return result;
            list.Remove(Callback);
            return (false, null);
        }
        #endregion
    }
}