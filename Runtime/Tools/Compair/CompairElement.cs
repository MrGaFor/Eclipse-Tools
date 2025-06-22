using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine;

namespace EC.Compair
{
    [System.Serializable]
    public class CompairElementGeneric<T>
    {
        [SerializeField, LabelWidth(70)] private EventTypes _type;

        [SerializeField, ShowIf("@_type == EventTypes.Event")] private UnityEvent<T> _event;
        [SerializeField, ShowIf("@_type == EventTypes.Bus"), HideLabel] private Bus.BusSettingsGenericCustom<T> _bus;
        [SerializeField, ShowIf("@_type == EventTypes.Effect"), HideLabel] private Effects.IEffectorComponent[] _effect;

        public void Invoke(T value)
        {
            switch (_type)
            {
                case EventTypes.Event:
                    _event?.Invoke(value);
                    break;
                case EventTypes.Bus:
                    _bus?.Invoke(value);
                    break;
                case EventTypes.Effect:
                    foreach (var effect in _effect)
                        effect?.PlaySmooth();
                    break;
            }
        }
    }

    [System.Serializable]
    public class CompairElementDefault
    {
        [SerializeField, LabelWidth(70)] private EventTypes _type;

        [SerializeField, ShowIf("_type", EventTypes.Event)] private UnityEvent _event;
        [SerializeField, ShowIf("_type", EventTypes.Bus), HideLabel] private Bus.BusSettingsInDefault _bus;
        [SerializeField, ShowIf("_type", EventTypes.Effect), HideLabel] private Effects.IEffectorComponent[] _effect;

        public void Invoke()
        {
            switch (_type)
            {
                case EventTypes.Event:
                    _event?.Invoke();
                    break;
                case EventTypes.Bus:
                    _bus?.Invoke();
                    break;
                case EventTypes.Effect:
                    foreach (var effect in _effect)
                        effect?.PlaySmooth();
                    break;
            }
        }
    }
}
