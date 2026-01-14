using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public class InputComponent : MonoBehaviour
    {
        [SerializeField, LabelText("Old Key"), HorizontalGroup(), ShowIf("HasOldKey")] protected string _key; private bool HasOldKey => !string.IsNullOrEmpty(_key);
#if UNITY_EDITOR
        [SerializeField, HorizontalGroup(25), Button("➰"), ShowIf("HasOldKey"), Tooltip("Transfer to new Key")] private void BtnTransferKey() { _busKey.TransferKey(_key); _key = ""; }
#endif
        [SerializeField, HideLabel] protected BusKey _busKey;

        public string Key => string.IsNullOrEmpty(_key) ? _busKey.Key : _key;

        public void OnInvoke() => BusSystem.Invoke(Key);
    }
    [HideMonoScript]
    public class InputComponent<T> : MonoBehaviour
    {
        [SerializeField, LabelText("Old Key"), HorizontalGroup(), ShowIf("HasOldKey")] protected string _key; private bool HasOldKey => !string.IsNullOrEmpty(_key);
#if UNITY_EDITOR
        [SerializeField, HorizontalGroup(25), Button("➰"), ShowIf("HasOldKey"), Tooltip("Transfer to new Key")] private void BtnTransferKey() { _busKey.TransferKey(_key); _key = ""; }
#endif
        [SerializeField, HideLabel] protected BusKey _busKey;

        public string Key => string.IsNullOrEmpty(_key) ? _busKey.Key : _key;

        public void OnInvoke(T value) => BusSystem.Invoke<T>(Key, value);
        public void OnSet(T value) => BusSystem.Set<T>(Key, value);
    }
}
