using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.UI
{
    [HideMonoScript]
    public class BusInputField : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("params", 0.5f), LabelWidth(30)] private string _key;
        [SerializeField, HorizontalGroup("params"), LabelWidth(60)] private int _minChar = 5;
        [SerializeField, HorizontalGroup("params"), LabelWidth(60)] private int _maxChar = 15;
        [SerializeField, HorizontalGroup("params", 60), LabelWidth(40)] private bool _space = true;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent<string> _onChange;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/Validation")] private UnityEvent _onTrueValidation;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/Validation")] private UnityEvent _onFalseValidation;

        private bool _isValid = false;

        private void OnEnable()
        {
            OnChangeValue(BusSystem.Get<string>(_key, ""));
            BusSystem.Subscribe<string>(_key, OnChangeValue);
        }
        private void OnDisable()
        {
            BusSystem.Unsubscribe<string>(_key, OnChangeValue);
        }

        public void SetValue(string value)
        {
            BusSystem.Set<string>(_key, value);
        }
        public void AddChar(char value)
        {
            BusSystem.Set<string>(_key, BusSystem.Get<string>(_key) + value);
        }
        public void RemoveChar()
        {
            if (BusSystem.Get<string>(_key).Length == 0) return;
            BusSystem.Set<string>(_key, BusSystem.Get<string>(_key).Substring(0, BusSystem.Get<string>(_key).Length - 1));
        }
        private void OnChangeValue(string value)
        {
            if (!_space && value.Contains(" "))
            {
                BusSystem.Set<string>(_key, BusSystem.Get<string>(_key).Replace(" ", ""));
                return;
            }
            _onChange?.Invoke(value);
            bool isValid = value.Length >= _minChar && value.Length <= _maxChar;
            if (_isValid != isValid)
            {
                _isValid = isValid;
                (_isValid ? _onTrueValidation : _onFalseValidation)?.Invoke();
            }
        }

    }
}
