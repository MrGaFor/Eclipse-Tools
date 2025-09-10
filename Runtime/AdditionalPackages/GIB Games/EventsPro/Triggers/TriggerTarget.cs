using UnityEngine;

namespace GIB.Triggers
{
    /// <summary>
    /// A target script used for selective detection for <see cref="GIBTrigger"/>.
    /// </summary>
    public class TriggerTarget : MonoBehaviour
    {
        /// <summary>
        /// Object's target ID. Does not need to be unique.
        /// </summary>
        public string TargetId;
    }
}
