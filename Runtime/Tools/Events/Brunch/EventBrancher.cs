using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.Brancher
{
    [System.Serializable]
    public class EventBrancher<T>
    {
        [SerializeField, BoxGroup(), ListDrawerSettings(ShowFoldout = false, ShowPaging = false, DraggableItems = false)] private BranchVariant<T>[] _variants;

        public void Branch(T value)
        {
            foreach (var variant in _variants)
                variant.CheckValue(value);
        }
    }

    [HideMonoScript]
    public class MonoEventBrancher<T> : MonoBehaviour
    {
        [SerializeField, BoxGroup(), ListDrawerSettings(ShowFoldout = false, ShowPaging = false, DraggableItems = false)] private BranchVariant<T>[] _variants;

        public void Branch(T value)
        {
            foreach (var variant in _variants)
                variant.CheckValue(value);
        }
    }

    [System.Serializable]
    public struct BranchVariant<T>
    {
        [SerializeField, LabelWidth(70)] private T _value;
        [FoldoutGroup("Events"), HorizontalGroup("Events/evs"), SerializeField] private UnityEvent _isTrue;
        [FoldoutGroup("Events"), HorizontalGroup("Events/evs"), SerializeField] private UnityEvent _isFalse;

        public void CheckValue(T value) => (Equals(value, _value) ? _isTrue : _isFalse)?.Invoke();
    }
}
