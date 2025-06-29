using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.UI
{
    [HideMonoScript]
    public class BusToggle : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/events")] private UnityEvent _isOn;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/events")] private UnityEvent _isOff;

        private void OnEnable()
        {
            OnChangeState(BusSystem.Get<bool>(_key));
            BusSystem.Subscribe<bool>(_key, OnChangeState);
        }
        private void OnDisable()
        {
            BusSystem.Unsubscribe<bool>(_key, OnChangeState);
        }

        public void ChangeState()
        {
            SetState(!BusSystem.Get<bool>(_key));
        }
        public void SetState(bool state)
        {
            BusSystem.Set<bool>(_key, state);
        }

        private void OnChangeState(bool state)
        {
            (state ? _isOn : _isOff)?.Invoke();
        }
    }
}
