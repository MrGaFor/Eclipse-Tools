using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputDown : InputTouchComponent
    {
        [SerializeField] private UnityEvent _event;

        public override EventVariant EventVariant => EventVariant.Down;
        public override void InvokeEvent(EventVariant eventType)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke();
        }
    }
}
