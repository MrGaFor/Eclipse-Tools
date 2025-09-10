using UnityEngine;
using UnityEngine.Events;

namespace GIB.Triggers
{
    public class LiteTrigger : MonoBehaviour
    {
        //The litest event around, y'all
        [SerializeField] private EventPro liteEvent;

        public void TriggerEvent() => liteEvent.Invoke();
    }
}
