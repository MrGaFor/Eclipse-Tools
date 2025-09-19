using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Localization
{
    [HideMonoScript]
    public class LocalizationEvent : MonoBehaviour
    {
        [SerializeField, BoxGroup("sett", ShowLabel = false), HideLabel] private LocalizationElement<UnityEvent> _data;

        public void OnEnable()
        {
            LocalizationSystem.SubscribeChange(OnLanguageChange);
            OnLanguageChange();
        }
        public void OnDisable()
        {
            LocalizationSystem.UnsubscribeChange(OnLanguageChange);
        }

        private void OnLanguageChange()
        {
            _data.GetValue()?.Invoke();
        }
    }
}
