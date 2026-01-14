using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Bus
{
    [HideMonoScript]
    public class OutputComponent : MonoBehaviour
    {
        [SerializeField, LabelText("Old Key"), HorizontalGroup(), ShowIf("HasOldKey")] protected string _key; private bool HasOldKey => !string.IsNullOrEmpty(_key);
#if UNITY_EDITOR
        [SerializeField, HorizontalGroup(25), Button("➰"), ShowIf("HasOldKey"), Tooltip("Transfer to new Key")] private void BtnTransferKey() { _busKey.TransferKey(_key); _key = ""; }
#endif
        [SerializeField, HorizontalGroup("input"), HideLabel] protected BusKey _busKey;
        [SerializeField, HorizontalGroup("input", 90), HideLabel] protected AutoCall _autoCallType = AutoCall.None;

        [SerializeField] protected UnityEvent _event;

        private string Key => (_key != "") ? _key : _busKey.Key;

        private void Awake() { if (_autoCallType == AutoCall.Awake) OnInvoke(); }
        private void Start() { if (_autoCallType == AutoCall.Start) OnInvoke(); }
        private void OnEnable() { BusSystem.Subscribe(Key, OnInvoke); if (_autoCallType == AutoCall.OnEnable) OnInvoke(); }
        private void OnDisable() { BusSystem.Unsubscribe(Key, OnInvoke); if (_autoCallType == AutoCall.OnDisable) OnInvoke(); }
        private void OnDestroy() { if (_autoCallType == AutoCall.OnDestroy) OnInvoke(); }
        public void OnInvoke() => _event?.Invoke();
    }
    [HideMonoScript]
    public class OutputComponent<T> : MonoBehaviour
    {
        [SerializeField, LabelText("Old Key"), HorizontalGroup(), ShowIf("HasOldKey")] protected string _key; private bool HasOldKey => !string.IsNullOrEmpty(_key);
#if UNITY_EDITOR
        [SerializeField, HorizontalGroup(25), Button("➰"), ShowIf("HasOldKey"), Tooltip("Transfer to new Key")] private void BtnTransferKey() { _busKey.TransferKey(_key); _key = ""; }
#endif
        [SerializeField, HorizontalGroup("input"), HideLabel] protected BusKey _busKey;
        [SerializeField, HorizontalGroup("input", 90), HideLabel] protected AutoCall _autoCallType = AutoCall.None;

        [SerializeField] protected UnityEvent<T> _event;

        private string Key => (_key != "") ? _key : _busKey.Key;

        private void Awake() { if (_autoCallType == AutoCall.Awake) OnInvoke(); }
        private void Start() { if (_autoCallType == AutoCall.Start) OnInvoke(); }
        private void OnEnable() { BusSystem.Subscribe<T>(Key, OnInvoke); if (_autoCallType == AutoCall.OnEnable) OnInvoke(); }
        private void OnDisable() { BusSystem.Unsubscribe<T>(Key, OnInvoke); if (_autoCallType == AutoCall.OnDisable) OnInvoke(); }
        private void OnDestroy() { if (_autoCallType == AutoCall.OnDestroy) OnInvoke(); }
        public void OnInvoke(T value) => _event?.Invoke(value);
        public void OnInvoke() { if (BusSystem.HasKey(Key)) OnInvoke(BusSystem.Get<T>(Key)); }
    }
}
