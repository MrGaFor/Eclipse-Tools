using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputUp : InputTouchComponent
    {
        [SerializeField] private UnityEvent _event;

        public override EventVariant EventVariant => EventVariant.Up;
        public override void InvokeEvent(EventVariant eventType)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke();
        }
    }
}
