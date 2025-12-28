using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Popup
{
    [HideMonoScript]
    public abstract class PopupBase : MonoBehaviour
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
    }
}
