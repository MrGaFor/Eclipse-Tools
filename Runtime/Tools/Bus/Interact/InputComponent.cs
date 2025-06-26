using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public class InputComponent : MonoBehaviour
    {
        [SerializeField] protected string _key;

        public void OnInvoke() => BusSystem.Invoke(_key);
    }
    [HideMonoScript]
    public class InputComponent<T> : MonoBehaviour
    {
        [SerializeField] protected string _key;

        public void OnInvoke(T value) => BusSystem.Invoke<T>(_key, value);
        public void OnSet(T value) => BusSystem.Set<T>(_key, value);
    }
}
