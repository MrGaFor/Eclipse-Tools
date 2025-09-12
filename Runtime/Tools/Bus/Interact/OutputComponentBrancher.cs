using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Bus
{
    [HideMonoScript]
    public class OutputComponentBrancher<T> : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] protected string _key;
        [SerializeField, HorizontalGroup("input", 90), HideLabel] protected AutoCall _autoCallType = AutoCall.None;
        [SerializeField, BoxGroup(), ListDrawerSettings(ShowFoldout = false, ShowPaging = false, DraggableItems = false)] private OutputBranchVariant<T>[] _variants;

        private void Awake() { if (_autoCallType == AutoCall.Awake && BusSystem.HasKey(_key)) Branch(BusSystem.Get<T>(_key)); }
        private void Start() { if (_autoCallType == AutoCall.Start && BusSystem.HasKey(_key)) Branch(BusSystem.Get<T>(_key)); }
        private void OnEnable() { BusSystem.Subscribe<T>(_key, Branch); if (_autoCallType == AutoCall.OnEnable && BusSystem.HasKey(_key)) Branch(BusSystem.Get<T>(_key)); }
        private void OnDisable() { BusSystem.Unsubscribe<T>(_key, Branch); if (_autoCallType == AutoCall.OnDisable && BusSystem.HasKey(_key)) Branch(BusSystem.Get<T>(_key)); }
        private void OnDestroy() { if (_autoCallType == AutoCall.OnDestroy && BusSystem.HasKey(_key)) Branch(BusSystem.Get<T>(_key)); }
        private void Branch(T value)
        {
            foreach (var variant in _variants)
                variant.CheckValue(value);
        }
    }

    [System.Serializable]
    public struct OutputBranchVariant<T>
    {
        [SerializeField, LabelWidth(70)] private T _value;
        [FoldoutGroup("Events"), HorizontalGroup("Events/evs"), SerializeField] private UnityEvent _isTrue;
        [FoldoutGroup("Events"), HorizontalGroup("Events/evs"), SerializeField] private UnityEvent _isFalse;

        public void CheckValue(T value) => (Equals(value, _value) ? _isTrue : _isFalse)?.Invoke();
    }
}
