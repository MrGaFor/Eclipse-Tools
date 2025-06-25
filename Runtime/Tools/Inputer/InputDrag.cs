using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputDrag : InputComponent
    {
        [SerializeField] private UnityEvent<Vector2> _event;

        public override EventVariant EventVariant => EventVariant.Drag;
        public override void InvokeEvent(EventVariant eventType, Vector2 vectorValue)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke(vectorValue);
        }
    }
}
