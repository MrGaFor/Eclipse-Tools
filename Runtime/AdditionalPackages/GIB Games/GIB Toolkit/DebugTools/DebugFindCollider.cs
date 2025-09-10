using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Toolkit
{
    /// <summary>
    /// Used for determining if an object is colliding. For debug purposes only.
    /// </summary>
    public class DebugFindCollider : MonoBehaviour
    {
        [InfoBox("If you're using this script you must have spent hours" +
            " trying to figure out what the heck is colliding with the thing." +
            " If that's the case I am so sorry.", InfoMessageType.Warning)]
        [SerializeField] private bool DetectCollisions;
        [SerializeField] private bool DetectTriggerEnter;
        [SerializeField] private bool DetectTriggerExit;
        private void OnCollisionEnter(Collision collision)
        {
            if (DetectCollisions)
                Debug.Log($"collided with {collision.collider.gameObject.name}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (DetectTriggerEnter)
                Debug.Log($"OnTriggerEnter with {other.gameObject.name}");
        }

        private void OnTriggerExit(Collider other)
        {
            if (DetectTriggerExit)
                Debug.Log($"OnTriggerExit with {other.gameObject.name}");
        }
    }
}