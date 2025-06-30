using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.UI
{
    [HideMonoScript]
    public class BusSlider : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("params", 0.5f), LabelWidth(30)] private string _key;
        [SerializeField, HorizontalGroup("params"), LabelWidth(30)] private float _min = 0f;
        [SerializeField, HorizontalGroup("params"), LabelWidth(30)] private float _max = 1f;
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

        public void SetValue(float value)
        {
            BusSystem.Set<float>(_key, Mathf.Clamp(value, _min, _max));
        }
        public void SetMoveValue(float delta)
        {
            BusSystem.Set<float>(_key, Mathf.Clamp(BusSystem.Get<float>(_key) + delta, _min, _max));
        }
        private void OnChangeValue(float value)
        {
            _onChange?.Invoke(Mathf.Clamp(value, _min, _max));
        }

    }
}
