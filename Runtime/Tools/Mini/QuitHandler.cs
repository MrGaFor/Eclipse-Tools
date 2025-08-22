using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Mini
{
    [HideMonoScript]
    public class QuitHandler : MonoBehaviour
    {
        public void OnQuit()
        {
            Application.Quit();
        }

        public void OnQuitDelay(float delay)
        {
            QuitDelayed(delay).Forget();
        }
        private async UniTask QuitDelayed(float delay)
        {
            await UniTask.Delay(Mathf.RoundToInt(delay * 1000f));
            Application.Quit();
        }

    }
}
