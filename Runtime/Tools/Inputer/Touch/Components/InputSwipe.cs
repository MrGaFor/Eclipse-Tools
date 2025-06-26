using UnityEngine;
using UnityEngine.Events;

namespace EC.Inputer
{
    public class InputSwipe : InputTouchComponent
    {
        [SerializeField] private UnityEvent<Vector2> _event;

        public override EventVariant EventVariant => EventVariant.Swipe;
        public override void InvokeEvent(EventVariant eventType, Vector2 vectorValue)
        {
            if (EventVariant != eventType) return;
            _event?.Invoke(vectorValue);
        }
    }
}
