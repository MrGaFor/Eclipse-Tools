using UnityEngine;

namespace EC.Compair
{
    [System.Serializable]
    public class CompairSettingsGeneric<T>
    {
        [SerializeField] private CompairElementGeneric<T>[] _events;

        public void Invoke(T value)
        {
            foreach (var e in _events)
                e.Invoke(value);
        }
    }
    [System.Serializable]
    public class CompairSettingsDefault
    {
        [SerializeField] private CompairElementDefault[] _events;

        public void Invoke()
        {
            foreach (var e in _events)
                e.Invoke();
        }
    }
}
