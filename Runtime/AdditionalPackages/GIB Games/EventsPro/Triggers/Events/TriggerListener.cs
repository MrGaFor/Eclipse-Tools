using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GIB.EventTriggers
{
    /// <summary>
    /// A light listener script that fires off a specific Unity Event.
    /// </summary>
    public class TriggerListener : MonoBehaviour
    {
        protected UnityAction onEvent;
        /// <summary>
        /// The name of the Event being called.
        /// </summary>
        [SerializeField] protected string eventName;
        /// <summary>
        /// The events being triggered when this event is called.
        /// </summary>
        [SerializeField] private EventPro triggeredEvents;

        #region Listener
        protected void OnEnable()
        {
            onEvent = new UnityAction(TriggerEvents);
            EventController.Subscribe(eventName, onEvent);
        }

        protected void OnDisable()
        {
            EventController.Unsubscribe(eventName, onEvent);
        }
        #endregion

        /// <summary>
        /// Trigger the events.
        /// </summary>
        public virtual void TriggerEvents()
        {
            triggeredEvents.Invoke();
        }
    }
}
