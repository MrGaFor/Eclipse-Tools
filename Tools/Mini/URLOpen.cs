using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Mini
{
    [HideMonoScript]
    public class URLOpen : MonoBehaviour
    {
        [SerializeField] private string _url;

        public void Open() => Open(_url);
        public void Open(string url) => Application.OpenURL(url);
    }
}