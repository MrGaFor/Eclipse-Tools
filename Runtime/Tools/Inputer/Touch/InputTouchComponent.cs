using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Inputer
{
    [HideMonoScript, RequireComponent(typeof(InputTouchController)), AddComponentMenu("")]
    public class InputTouchComponent : MonoBehaviour
    {
        public virtual EventVariant EventVariant { get; }

        public virtual void InvokeEvent(EventVariant eventType) { }
        public virtual void InvokeEvent(EventVariant eventType, Vector2 vectorValue) { }
        public virtual void InvokeEvent(EventVariant eventType, float floatValue) { }
    }
}
