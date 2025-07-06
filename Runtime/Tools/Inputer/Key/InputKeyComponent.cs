using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    [HideMonoScript]
    public class InputKeyComponent : MonoBehaviour
    {
        private enum CallCount { Any, Limit }
        [SerializeField, HorizontalGroup("sett"), LabelWidth(60)] private KeyCode _key;
        [SerializeField, HorizontalGroup("sett", 140), LabelWidth(60), LabelText("Count")] private CallCount _callCount;
        [SerializeField, HorizontalGroup("sett", 80), HideLabel, ShowIf("_callCount", CallCount.Limit), Min(1)] private int _limitCount = 1;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onDown;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onUp;

        private int _leftCount;

        private void OnEnable()
        {
            if (_callCount == CallCount.Limit) _leftCount = _limitCount;
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
            if (_callCount == CallCount.Limit && _leftCount == 0) return;
            _onDown?.Invoke();
        }
        public void OnUp()
        {
            if (_callCount == CallCount.Limit && _leftCount == 0) return;
            if (_callCount == CallCount.Limit) _leftCount--;
            _onUp?.Invoke();
        }
    }
}
