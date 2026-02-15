using Cysharp.Threading.Tasks;

namespace EC.Services
{
#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
    public abstract class GameService
    {
        public virtual void OnCreate()
        {

        }
        public virtual async UniTask OnCreateAsync()
        {

        }
        public virtual void OnDispose()
        {

        }
        public virtual async UniTask OnDisposeAsync()
        {

        }
    }
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
}
