using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EC.Scenes
{
    public static class SceneLoader
    {
        private static SceneLoaderUI _ui;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async UniTask Init()
        {
            EC.Bus.BusSystem.Subscribe<int>("LoadSceneIndex", async (index) => await LoadScene(index));
            EC.Bus.BusSystem.Subscribe<string>("LoadSceneName", async (name) => await LoadScene(name));
            EC.Bus.BusSystem.Subscribe<string>("LoadSceneAddressablesName", async (name) => await LoadSceneAddressables(name));

            _ui = Resources.Load<SceneLoaderUI>("[SceneLoaderUI]");
            if (_ui)
            {
                _ui = GameObject.Instantiate(_ui);
                GameObject.DontDestroyOnLoad(_ui.gameObject);
                _ui.PlayMoment(true);
                _ui.PlaySmooth(false).Forget();
            }
            else
            {
                //Debug.LogWarning("[SceneLoaderUI] prefab not found in Resources folder. Please create a <<[SceneLoaderUI]>> prefab and place it in the Resources folder.");
                return;
            }
            await UniTask.Yield();
        }
        private static async UniTask SetUI(bool isShowing)
        {
            if (_ui == null) return;
            await _ui.PlaySmooth(isShowing);
        }

        public static async UniTask LoadScene(int sceneIndex)
        {
            await SetUI(true);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            await SetUI(false);
        }
        public static async UniTask LoadScene(string sceneName)
        {
            await SetUI(true);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            await SetUI(false);
        }
        public static async UniTask LoadSceneAddressables(string address)
        {
            await SetUI(true);
            await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(address, UnityEngine.SceneManagement.LoadSceneMode.Single, true).Task;
            await SetUI(false);
        }
    }
}
