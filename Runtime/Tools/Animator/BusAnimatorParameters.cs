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
            [SerializeField, HorizontalGroup(), LabelWidth(60f)] private string _key;

            private UnityAnimator _animator;

            public void OnInit(UnityAnimator animator)
            {
                _animator = animator;
            }
            public void OnEnable()
            {
                switch (_type)
                {
                    case KeyType.Trigger: Bus.BusSystem.Subscribe(_key, OnTriggerSet); break;
                    case KeyType.Float: Bus.BusSystem.Subscribe<float>(_key, OnFloatSet); break;
                    case KeyType.Bool: Bus.BusSystem.Subscribe<bool>(_key, OnBoolSet); break;
                }
            }
            public void OnDisable()
            {
                switch (_type)
                {
                    case KeyType.Trigger: Bus.BusSystem.Unsubscribe(_key, OnTriggerSet); break;
                    case KeyType.Float: Bus.BusSystem.Unsubscribe<float>(_key, OnFloatSet); break;
                    case KeyType.Bool: Bus.BusSystem.Unsubscribe<bool>(_key, OnBoolSet); break;
                }
            }

            private void OnTriggerSet() => _animator.SetTrigger(_key);
            private void OnFloatSet(float value) => _animator.SetFloat(_key, value);
            private void OnBoolSet(bool value) => _animator.SetBool(_key, value);
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
