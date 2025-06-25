using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputExit : InputComponent
    {
        [SerializeField] private UnityEvent _event;

        public override EventVariant EventVariant => EventVariant.Exit;
        public override void InvokeEvent(EventVariant eventType)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke();
        }
    }
}
