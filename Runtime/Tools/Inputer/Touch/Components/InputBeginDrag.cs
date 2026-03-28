using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputBeginDrag : InputTouchComponent
    {
        [SerializeField] private UnityEvent _event;
        public UnityEvent Event => _event;

        public override EventVariant EventVariant => EventVariant.BeginDrag;
        public override void InvokeEvent(EventVariant eventType)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke();
        }
    }
}
