using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Triggers
{
    public abstract class PlayerTrigger : MonoBehaviour
    {
        [Tooltip("Tags of the objects expected to invoke this OnTriggerEnter.")] public List<string> TriggerTags = new List<string>(){ "Player" };
        public void OnTriggerEnter(Collider other)
        {
            if (TriggerTags.Contains(other.tag))
                OnPlayerTriggerEnter();
        }

        public abstract void Trigger();

        public virtual void OnPlayerTriggerEnter() { }
    }
}
