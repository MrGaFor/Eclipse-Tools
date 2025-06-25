using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputZoom : InputComponent
    {
        [SerializeField] private UnityEvent<float> _event;

        public override EventVariant EventVariant => EventVariant.Zoom;
        public override void InvokeEvent(EventVariant eventType, float floatValue)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke(floatValue);
        }
    }
}
