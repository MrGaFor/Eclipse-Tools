using System.Threading.Tasks;
using UnityEngine;

namespace EC.Scenes
{
    public static class SceneLoader
    {
        private static SceneLoaderUI _ui;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            _ui = Resources.Load<SceneLoaderUI>("[SceneLoaderUI]");
            if (_ui == null)
            {
                Debug.LogWarning("[SceneLoaderUI] prefab not found in Resources folder. Please create a <<[SceneLoaderUI]>> prefab and place it in the Resources folder.");
                return;
            }
            _ui = GameObject.Instantiate(_ui);
            GameObject.DontDestroyOnLoad(_ui.gameObject);
            _ui.PlayMoment(true);
            _ui.PlaySmooth(false);

            EC.Bus.BusSystem.Subscribe<int>("LoadSceneIndex", async (index) => await LoadScene(index));
            EC.Bus.BusSystem.Subscribe<string>("LoadSceneName", async (name) => await LoadScene(name));
            EC.Bus.BusSystem.Subscribe<string>("LoadSceneAddressablesName", async (name) => await LoadSceneAddressables(name));
        }
        private static async Task SetUI(bool isShowing)
        {
            if (_ui == null) return;
            _ui.PlaySmooth(isShowing);
            await Task.Delay((int)((isShowing ? _ui.ShowDuration : _ui.HideDuration) * 1000));
        }

        public static async Task LoadScene(int sceneIndex)
        {
            await SetUI(true);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            await SetUI(false);
        }
        public static async Task LoadScene(string sceneName)
        {
            await SetUI(true);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            await SetUI(false);
        }
        public static async Task LoadSceneAddressables(string address)
        {
            await SetUI(true);
            await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(address, UnityEngine.SceneManagement.LoadSceneMode.Single, true).Task;
            await SetUI(false);
        }
    }
}
