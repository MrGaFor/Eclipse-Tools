using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Bus
{
    [HideMonoScript]
    public class OutputComponent : MonoBehaviour
    {
        [SerializeField] protected string _key;
        [SerializeField] protected UnityEvent _event;

        private void OnEnable() => BusSystem.Subscribe(_key, OnInvoke);
        private void OnDisable() => BusSystem.Unsubscribe(_key, OnInvoke);

        private void OnInvoke() => _event?.Invoke();
    }
    [HideMonoScript]
    public class OutputComponent<T> : MonoBehaviour
    {
        [SerializeField] protected string _key;
        [SerializeField] protected UnityEvent<T> _event;

        private void OnEnable() => BusSystem.Subscribe<T>(_key, OnInvoke);
        private void OnDisable() => BusSystem.Unsubscribe<T>(_key, OnInvoke);

        private void OnInvoke(T value) => _event?.Invoke(value);
    }
}
