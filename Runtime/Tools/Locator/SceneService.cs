using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EC.Services
{
    public abstract class SceneService<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this as T;
            SceneLocator.Register<T>(Instance);
        }

        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(Instance, this))
                Instance = null;
        }

        public static bool TryGet(out T service)
        {
            service = Instance ?? (SceneLocator.TryGet<T>(out var s) ? s : null);
            return service != null;
        }

        public static UniTask<(bool success, T service)> TryGetAsync(float timeoutSeconds)
        {
            if (Instance != null)
                return UniTask.FromResult((true, Instance));

            return SceneLocator.TryGetAsync<T>(timeoutSeconds);
        }
    }
}
