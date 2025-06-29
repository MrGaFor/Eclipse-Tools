using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.UI
{
    [HideMonoScript]
    public class BusDropDown : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private EventPair[] _elements;

        [System.Serializable]
        private class EventPair
        {
            [FoldoutGroup("Events"), HorizontalGroup("Events/events")] public UnityEvent _isOn;
            [FoldoutGroup("Events"), HorizontalGroup("Events/events")] public UnityEvent _isOff;
        }

        private void OnEnable()
        {
            OnChangeValue(BusSystem.Get<int>(_key));
            BusSystem.Subscribe<int>(_key, OnChangeValue);
        }
        private void OnDisable()
        {
            BusSystem.Unsubscribe<int>(_key, OnChangeValue);
        }


        public void SetPrevValue()
        {
            SetValue(Mathf.RoundToInt(Mathf.PingPong(BusSystem.Get<int>(_key) - 1, _elements.Length  -1)));
        }
        public void SetNextValue()
        {
            SetValue(Mathf.RoundToInt(Mathf.PingPong(BusSystem.Get<int>(_key) - 1, _elements.Length + 1)));
        }
        public void SetValue(int state)
        {
            BusSystem.Set<int>(_key, state);
        }

        private void OnChangeValue(int value)
        {
            for (int i = 0; i < _elements.Length; i++)
                (value == i ? _elements[i]._isOn : _elements[i]._isOff)?.Invoke();
        }

    }
}
