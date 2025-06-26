using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputEndDrag : InputTouchComponent
    {
        [SerializeField] private UnityEvent _event;

        public override EventVariant EventVariant => EventVariant.EndDrag;
        public override void InvokeEvent(EventVariant eventType)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke();
        }
    }
}
