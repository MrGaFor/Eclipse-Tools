using Sirenix.OdinInspector;
using System.Threading.Tasks;
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
            QuitDelayed(delay);
        }
        private async Task QuitDelayed(float delay)
        {
            await Task.Delay(Mathf.RoundToInt(delay * 1000f));
            Application.Quit();
        }

    }
}
