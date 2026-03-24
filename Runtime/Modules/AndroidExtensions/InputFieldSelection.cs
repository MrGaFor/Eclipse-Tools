#if EC_ANDROID_EXTENSIONS
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_ANDROID
using UnityEngine.Events;
#endif

namespace AndroidExtensions
{
#if !UNITY_ANDROID
    [InfoBox("Works only on Android", InfoMessageType.Warning)]
#endif
    [ComponentIcon("Text Icon")]
    public class InputFieldSelection : MonoBehaviour
    {
#if UNITY_ANDROID
        [SerializeField] private TMPro.TMP_InputField _inputField;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onSelect;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onDeselect;

        private void OnEnable()
        {
            _inputField.onSelect.AddListener(OnSelect);
            _inputField.onDeselect.AddListener(OnDeselect);
            if (_inputField.isFocused)
                OnSelect("");
            else
                OnDeselect("");
        }

        public void OnDisable()
        {
            _inputField.onSelect.RemoveListener(OnSelect);
            _inputField.onDeselect.RemoveListener(OnDeselect);
            OnDeselect("");
        }

        private void OnSelect(string arg0)
        {
            _onSelect?.Invoke();
        }
        private void OnDeselect(string arg0)
        {
            _onDeselect?.Invoke();
        }
#endif
    }
}
#endif