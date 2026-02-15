using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EC.Services
{
#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
    public abstract class SceneService : MonoBehaviour
    {
        [SerializeField] private bool _autoRegister = true;

        private void Awake()
        {
            if (_autoRegister)
                SceneLocator.Register(this);
        }

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
