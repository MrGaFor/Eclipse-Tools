using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Bus
{
    [HideMonoScript]
    public class OutputComponent : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] protected string _key;
        [SerializeField, HorizontalGroup("input", 90), HideLabel] protected AutoCall _autoCallType = AutoCall.None;
        [SerializeField] protected UnityEvent _event;

        private void Awake() { if (_autoCallType == AutoCall.Awake) OnInvoke(); }
        private void Start() { if (_autoCallType == AutoCall.Start) OnInvoke(); }
        private void OnEnable() { BusSystem.Subscribe(_key, OnInvoke); if (_autoCallType == AutoCall.OnEnable) OnInvoke(); }
        private void OnDisable() { BusSystem.Unsubscribe(_key, OnInvoke); if (_autoCallType == AutoCall.OnDisable) OnInvoke(); }
        private void OnDestroy() { if (_autoCallType == AutoCall.OnDestroy) OnInvoke(); }
        private void OnInvoke() => _event?.Invoke();
    }
    [HideMonoScript]
    public class OutputComponent<T> : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] protected string _key;
        [SerializeField, HorizontalGroup("input", 90), HideLabel] protected AutoCall _autoCallType = AutoCall.None;
        [SerializeField] protected UnityEvent<T> _event;

        private void Awake() { if (_autoCallType == AutoCall.Awake && BusSystem.HasKey(_key)) OnInvoke(BusSystem.Get<T>(_key)); }
        private void Start() { if (_autoCallType == AutoCall.Start && BusSystem.HasKey(_key)) OnInvoke(BusSystem.Get<T>(_key)); }
        private void OnEnable() { BusSystem.Subscribe<T>(_key, OnInvoke); if (_autoCallType == AutoCall.OnEnable && BusSystem.HasKey(_key)) OnInvoke(BusSystem.Get<T>(_key)); }
        private void OnDisable() { BusSystem.Unsubscribe<T>(_key, OnInvoke); if (_autoCallType == AutoCall.OnDisable && BusSystem.HasKey(_key)) OnInvoke(BusSystem.Get<T>(_key)); }
        private void OnDestroy() { if (_autoCallType == AutoCall.OnDestroy && BusSystem.HasKey(_key)) OnInvoke(BusSystem.Get<T>(_key)); }
        private void OnInvoke(T value) => _event?.Invoke(value);
    }
}
