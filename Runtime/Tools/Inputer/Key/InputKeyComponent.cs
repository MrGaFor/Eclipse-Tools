using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    [HideMonoScript]
    public class InputKeyComponent : MonoBehaviour
    {
        [SerializeField] private KeyCode _key;
        [SerializeField, HorizontalGroup("evs")] private UnityEvent _onDown;
        [SerializeField, HorizontalGroup("evs")] private UnityEvent _onUp;
        [SerializeField, HorizontalGroup("evs")] private UnityEvent _onClick;

        private bool _isDown;

        private void OnEnable()
        {
            InputKeyController.Subscribe(_key, OnDown, true);
            InputKeyController.Subscribe(_key, OnUp, false);
        }
        private void OnDisable()
        {
            InputKeyController.Unsubscribe(_key, OnDown, true);
            InputKeyController.Unsubscribe(_key, OnUp, false);
        }

        public void OnDown()
        {
            _onDown?.Invoke();
            _isDown = true;
        }
        public void OnUp()
        {
            _onUp?.Invoke();
            if (_isDown)
            {
                _isDown = false;
                OnClick();
            }
        }
        public void OnClick() => _onClick?.Invoke();
    }
}
