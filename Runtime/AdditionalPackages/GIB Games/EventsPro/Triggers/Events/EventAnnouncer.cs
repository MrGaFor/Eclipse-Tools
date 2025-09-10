using Sirenix.OdinInspector;
using UnityEngine;

namespace GIB.EventTriggers
{
    /// <summary>
    /// Sends a specified event to the <see cref="EventController"/>.
    /// <remarks>Preferred method to use is <see cref="GameUtilities.Announce(string)"/>.</remarks>
    /// </summary>
    public class EventAnnouncer : MonoBehaviour
    {
        [SerializeField] private string eventName;

        /// <summary>
        /// Announce the event specified by the eventName property.
        /// </summary>
        [Button]
        public void AnnounceEvent()
        {
            EventController.Announce(eventName);
        }

        /// <summary>
        /// Announce a specified event.
        /// </summary>
        /// <param name="name">The name of the event to announce.</param>
        public void AnnounceEvent(string name)
        {
            EventController.Announce(name);
        }
    }
}
