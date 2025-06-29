using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.UI
{
    [HideMonoScript]
    public class BusSlider : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent<float> _onChange;

        private void OnEnable()
        {
            OnChangeValue(BusSystem.Get<float>(_key));
            BusSystem.Subscribe<float>(_key, OnChangeValue);
        }
        private void OnDisable()
        {
            BusSystem.Unsubscribe<float>(_key, OnChangeValue);
        }

        public void SetValue(float state)
        {
            BusSystem.Set<float>(_key, state);
        }

        private void OnChangeValue(float value)
        {
            _onChange?.Invoke(value);
        }

    }
}
