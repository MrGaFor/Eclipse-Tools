using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.Localization
{
    [HideMonoScript]
    public abstract class LocalizationMonoType<T> : MonoBehaviour
    {
        [SerializeField, BoxGroup("sett", ShowLabel = false), HideLabel] private LocalizationElement<T> _data;
        [SerializeField, BoxGroup("sett"), FoldoutGroup("sett/Event")] private UnityEvent<T> _onLanguageChange;

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
            _onLanguageChange?.Invoke(_data.GetValue());
        }
    }
}
