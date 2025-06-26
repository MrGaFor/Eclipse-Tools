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

        private void OnEnable()
        {
            InputKeyController.Subscribe(_key, OnDown, true);
            InputKeyController.Subscribe(_key, OnUp, false);
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
