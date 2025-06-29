using UnityEngine;
using EC.Bus;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

namespace EC.UI
{
    [HideMonoScript]
    public class BusKeyCode : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private KeyCode[] _allowKeyCodes;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/events")] private UnityEvent _onEnterEditing;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/events")] private UnityEvent<string> _onChange;
        [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/events")] private UnityEvent _onExitEditing;

        private void OnEnable()
        {
            OnChangeKey(BusSystem.Get<KeyCode>(_key));
            BusSystem.Subscribe<KeyCode>(_key, OnChangeKey);
        }
        private void OnDisable()
        {
            BusSystem.Unsubscribe<KeyCode>(_key, OnChangeKey);
        }

        public void EnterEditingKey()
        {
            if (_waiting == null)
            {
                _waiting = StartCoroutine(WaitForKeyDown());
                _onEnterEditing?.Invoke();
            }
        }
        public void ExitEditingKey()
        {
            if (_waiting != null)
            {
                StopCoroutine(_waiting);
                _waiting = null;
                _onExitEditing?.Invoke();
                OnChangeKey(BusSystem.Get<KeyCode>(_key));
            }
        }
        public void SetKey(KeyCode key)
        {
            BusSystem.Set<KeyCode>(_key, key);
            ExitEditingKey();
        }

        private void OnChangeKey(KeyCode key)
        {
            _onChange?.Invoke(key.ToString());
        }


        private UnityEngine.Coroutine _waiting;
        private IEnumerator WaitForKeyDown()
        {
            while (true)
            {
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ExitEditingKey();
                        yield break;
                    }
                    foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKeyDown(key) && (_allowKeyCodes.Length == 0 || _allowKeyCodes.Contains(key)))
                        {
                            SetKey(key);
                            yield break;
                        }
                    }
                }
                yield return null;
            }
        }

    }
}
