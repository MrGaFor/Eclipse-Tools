using Cysharp.Threading.Tasks;

namespace EC.Services
{
    public abstract class GameService<T> where T : class, new()
    {
        public static T Instance { get; private set; }

        protected GameService()
        {
            Instance = this as T;
            GameLocator.Register<T>(Instance);
        }

        public static bool TryGet(out T service)
        {
            service = Instance ?? (GameLocator.TryGet<T>(out var s) ? s : null);
            return service != null;
        }

        public static UniTask<(bool success, T service)> TryGetAsync(float timeoutSeconds)
        {
            if (Instance != null)
                return UniTask.FromResult((true, Instance));

            return GameLocator.TryGetAsync<T>(timeoutSeconds);
        }
    }
}
