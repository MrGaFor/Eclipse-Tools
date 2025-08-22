using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Addressables
{
    [HideMonoScript]
    public class AddressablesDataMonoGeneric<T> : MonoBehaviour where T : UnityEngine.Object
    {
        [SerializeField, HideLabel] private AddressablesDataGeneric<T> _data;
        [SerializeField] private bool _autoLoad = true;

        public LoadState LoadState => _data.LoadState;

        public async UniTask Load()
        {
            await _data.Load();
        }
        public void Unload()
        {
            _data.Unload();
        }

        public async UniTask<T> GetObject()
        {
            return await _data.GetObject();
        }


        private void Awake()
        {
            if (_autoLoad)
                Load().Forget();
        }
        private void OnDestroy()
        {
            _data.Unload();
        }
    }
}
