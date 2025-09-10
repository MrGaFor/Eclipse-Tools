using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GIB.EventTriggers
{
    /// <summary>
    /// A thread-safe singleton manager that keeps track of, and executes, specified events.
    /// </summary>
    public class EventController : MonoBehaviour
    {
        #region Singleton
        private static EventController _instance;
        private static object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of the EventController. If multiple instances exist, it keeps only one
        /// and deletes the others. If none exists, it creates one.
        /// </summary>
        public static EventController Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var instances = FindObjectsOfType<EventController>();

                        if (instances.Length > 1)
                        {
                            Debug.LogError($"Multiple instances of {typeof(EventController)}. Deleting extras.");

                            // Destroy all but keep one
                            for (int i = 1; i < instances.Length; i++)
                            {
                                Destroy(instances[i].gameObject);
                            }
                            _instance = instances[0];
                            return _instance;
                        }

                        if (instances.Length == 1)
                        {
                            return instances[0];
                        }

                        if (instances.Length == 0)
                        {
                            Debug.LogWarning($"An object attempted to get the instance of {typeof(EventController)} but it was not found." +
                                " A new instance has been created.");

                            GameObject newSingleton = new GameObject();
                            _instance = newSingleton.AddComponent<EventController>();
                            newSingleton.name = $"{typeof(EventController)} Singleton";
                        }
                    }
                    return _instance;
                }
            }
        }
    #endregion

        private static Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
        private static readonly object lockObject = new object(); // Lock object

#if DEBUG
        /// <summary>
        /// List of subscriptions.
        /// ReadOnly attribute ensures this field is view-only in the inspector.
        /// </summary>
        [ReadOnly]
        public List<string> subscriptionList;
#endif

        #region Public Static Methods

        /// <summary>
        /// Subscribes a listener to a specific event.
        /// </summary>
        /// <param name="eventName">Name of the event to subscribe to.</param>
        /// <param name="listener">Action to execute when the event is announced.</param>
        /// <param name="listenerName">Optional name for the listener, defaults to "anonymous".</param>
        public static void Subscribe(string eventName, UnityAction listener, string listenerName = "anonymous")
        {
            lock (lockObject)
            {
                if (!eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
                {
                    thisEvent = new UnityEvent();
                    eventDictionary.Add(eventName, thisEvent);
                }
                thisEvent.AddListener(listener);
            }
#if DEBUG
            if (Instance)
                Instance.AddToSubscriptionList(eventName, listenerName);
#endif
        }

        /// <summary>
        /// Unsubscribes a listener from a specific event.
        /// </summary>
        /// <param name="eventName">Name of the event to unsubscribe from.</param>
        /// <param name="listener">The action that was previously subscribed.</param>
        /// <param name="listenerName">Optional name for the listener, defaults to "anonymous".</param>

        public static void Unsubscribe(string eventName, UnityAction listener, string listenerName = "anonymous")
        {
            lock (lockObject)
            {
                if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
                {
                    thisEvent.RemoveListener(listener);
                }
            }
        }

        /// <summary>
        /// Announces an event, causing all subscribed listeners for that event to be invoked.
        /// </summary>
        /// <param name="eventName">Name of the event to announce.</param>

        public static void Announce(string eventName)
        {
            lock (lockObject)
            {
                if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
                {
                    thisEvent.Invoke();
                }
            }
#if DEBUG
            GIBUtils.Log($"Invoking EventController event {eventName}.");
#endif
        }

        #endregion

        #region private methods

#if DEBUG
        private void AddToSubscriptionList(string objName, string evName)
        {
            if (!subscriptionList.Contains(objName + " > " + evName))
            {
                subscriptionList.Add(objName + " > " + evName);
                subscriptionList.Sort();
            }
        }

        private void RemoveFromSubscriptionList(string objName, string evName)
        {
            if (subscriptionList.Contains(objName + " > " + evName))
            {
                subscriptionList.Remove(objName + " > " + evName);
            }
        }

        private void OutputSubscriptionList()
        {
            string subscribers = "";
            foreach (string s in subscriptionList)
            {
                subscribers += s + "\n";
            }
            GIBUtils.Log(subscribers);
        }
#endif

        #endregion
    }
}
