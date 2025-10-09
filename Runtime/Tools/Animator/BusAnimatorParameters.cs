using UnityEngine;
using Sirenix.OdinInspector;
using UnityAnimator = UnityEngine.Animator;

namespace EC.Animator
{
    [HideMonoScript]
    public class BusAnimatorParameters : MonoBehaviour
    {
        [SerializeField, BoxGroup(), DisableInPlayMode] private UnityAnimator _animator;
        [SerializeField, BoxGroup(), DisableInPlayMode, ListDrawerSettings(DraggableItems = false, ShowFoldout = false)] private AnimatorKey[] _keys;

        [System.Serializable]
        private class AnimatorKey
        {
            private enum KeyType { Trigger, Float, Bool }
            [SerializeField, HorizontalGroup(Width = 80f), HideLabel, DisableInPlayMode] private KeyType _type;
            [SerializeField, HorizontalGroup(), LabelWidth(40f), LabelText("Bus:"), OnValueChanged("OnChangeBusKey", InvokeOnInitialize = true)] private string _busKey;
            private void OnChangeBusKey()
            {
                if (_dublicate)
                    _animKey = _busKey;
            }
            [SerializeField, HorizontalGroup(), LabelWidth(40f), LabelText("Key:"), DisableIf("_dublicate")] private string _animKey;
            [SerializeField, HorizontalGroup(Width = 22f), HideLabel, OnValueChanged("OnChangeBusKey", InvokeOnInitialize = true)] private bool _dublicate = true;

            private UnityAnimator _animator;

            public void OnInit(UnityAnimator animator)
            {
                _animator = animator;
            }
            public void OnEnable()
            {
                switch (_type)
                {
                    case KeyType.Trigger: Bus.BusSystem.Subscribe(_busKey, OnTriggerSet); break;
                    case KeyType.Float: Bus.BusSystem.Subscribe<float>(_busKey, OnFloatSet); break;
                    case KeyType.Bool: Bus.BusSystem.Subscribe<bool>(_busKey, OnBoolSet); break;
                }
            }
            public void OnDisable()
            {
                switch (_type)
                {
                    case KeyType.Trigger: Bus.BusSystem.Unsubscribe(_busKey, OnTriggerSet); break;
                    case KeyType.Float: Bus.BusSystem.Unsubscribe<float>(_busKey, OnFloatSet); break;
                    case KeyType.Bool: Bus.BusSystem.Unsubscribe<bool>(_busKey, OnBoolSet); break;
                }
            }

            private void OnTriggerSet() => _animator.SetTrigger(_animKey);
            private void OnFloatSet(float value) => _animator.SetFloat(_animKey, value);
            private void OnBoolSet(bool value) => _animator.SetBool(_animKey, value);
        }

        public void Awake()
        {
            foreach (var key in _keys)
                key.OnInit(_animator);
        }
        private void OnEnable()
        {
            foreach (var key in _keys)
                key.OnEnable();
        }
        private void OnDisable()
        {
            foreach (var key in _keys)
                key.OnDisable();
        }
    }
}
