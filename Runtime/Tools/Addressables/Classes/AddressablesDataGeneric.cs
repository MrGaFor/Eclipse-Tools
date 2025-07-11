using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EC.Addressables
{
    public enum LoadState
    {
        Unloaded,
        Loading,
        Loaded,
        Failed
    }

    [System.Serializable]
    public class AddressablesDataGeneric<T> where T : UnityEngine.Object
    {
        public AddressablesDataGeneric(string key)
        {
            _key = key;
        }

#if UNITY_EDITOR
        private string[] GetIds() => Editor.AddressablesIdCache.GetAddressesOfType<T>();
        [ValueDropdown("GetIds")]
#endif
        [SerializeField, DisableInPlayMode, HorizontalGroup("Key"), LabelWidth(100)] private string _key;
        [HideInEditorMode, ReadOnly, SerializeField, HorizontalGroup("Key"), LabelWidth(100)] private LoadState _loadState = LoadState.Unloaded;

        public LoadState LoadState => _loadState;

        private T _object;
        private AsyncOperationHandle<T>? _loadHandle;


        public async Task Load()
        {
            if (string.IsNullOrEmpty(_key)) { Debug.LogError("Addressables key is NULL or Empty"); return; }
            if (LoadState == LoadState.Loaded) return;
            if (LoadState == LoadState.Loading) { await _loadHandle.Value.Task; return; }

            _loadState = LoadState.Loading;
            _loadHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(_key);

            try
            {
                await _loadHandle.Value.Task;

                if (!_loadHandle.Value.IsValid() || _loadHandle.Value.Status != AsyncOperationStatus.Succeeded)
                {
                    _loadState = LoadState.Failed;
                    _object = null;
                    _loadHandle = null;
                    return;
                }

                _object = _loadHandle.Value.Result;
                _loadState = LoadState.Loaded;
            }
            catch (Exception)
            {
                _loadState = LoadState.Failed;
                _object = null;
                _loadHandle = null;
            }
        }
        public void Unload()
        {
            if (LoadState == LoadState.Unloaded) return;
            if (_loadHandle.HasValue && _loadHandle.Value.IsValid())
            {
                UnityEngine.AddressableAssets.Addressables.Release(_loadHandle.Value);
                _loadHandle = null;
            }
            _object = null;
            _loadState = LoadState.Unloaded;
        }

        public async Task<T> GetObject()
        {
            await Load();
            if (_loadState == LoadState.Loaded) return _object;
            return null;
        }

    }
}
