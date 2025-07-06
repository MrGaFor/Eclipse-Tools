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
        [SerializeField, HorizontalGroup("sett", 80), HideLabel, ShowIf("_callCount", CallCount.Limit)] private int _limitCount;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onDown;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/evs")] private UnityEvent _onUp;


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
        }
        public void OnUp()
        {
            _onUp?.Invoke();
        }
    }
}
