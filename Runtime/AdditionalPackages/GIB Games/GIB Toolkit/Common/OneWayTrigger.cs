using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GIB.Toolkit
{
    public class OneWayTrigger : MonoBehaviour
    {
        BoxCollider m_collider;
        [Range(-1f, 1f)] public float variance;

        public EventPro _OnTriggerEnter;

        private void OnValidate()
        {
            if (m_collider != null)
                FixColliderZ();
        }

        private void Reset()
        {
            VerifyBoxCollider();
            m_collider.isTrigger = true;
        }

        private void VerifyBoxCollider()
        {
            if (m_collider == null)
            {
                // Add a BoxCollider component with size (1, 1, 0)
                m_collider = gameObject.AddComponent<BoxCollider>();
                m_collider.size = new Vector3(1f, 1f, 0f);
            }
            else
                m_collider = GetComponent<BoxCollider>();
        }

        private void FixColliderZ()
        {
            if (m_collider == null)
                VerifyBoxCollider();

            Vector3 size = m_collider.size;
            size.z = 0f;
            m_collider.size = size;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Get the average contact point between the two colliders
            Vector3 contactPoint = Vector3.zero;
            contactPoint = other.ClosestPoint(transform.position);

            // Calculate the direction from the trigger collider to the average contact point
            Vector3 incomingDirection = contactPoint - transform.position;

            // Check if the incoming direction is the same as the allowed direction
            if (Vector3.Dot(incomingDirection, transform.forward) > variance)
            {
                // The incoming collider is entering from the allowed direction, so trigger the desired behavior
                Debug.Log("Collision from valid direction");
                _OnTriggerEnter.Invoke();
            }
            else
            {
                // The incoming collider is not entering from the allowed direction, so ignore the collision
                Debug.Log("Collision from invalid direction");
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            BoxCollider box = transform.GetComponent<BoxCollider>();
            if (box == null) return;

            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix; // following Gizmos will be drawn in this transform's local space.
            Vector3 boxOffset = Vector3.forward * 0.1f;
            Gizmos.DrawWireCube(box.center, box.size);

            Vector3 forwardFace = transform.forward * box.size.z;
            Vector3 planeCenter = box.center + forwardFace;

            Gizmos.color = new Color(.2f, 1f, .2f, .2f);

            float arcOffset = 180 * variance * 0.5f;
            UnityEditor.Handles.color = Color.green;

#if UNITY_2020_2_OR_NEWER
            UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, 90 - arcOffset, .5f, 3f);
            UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, -90 + arcOffset, .5f, 3f);
#else
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, 90 - arcOffset, .5f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, -90 + arcOffset, .5f);
#endif
            //Gizmos.DrawWireCube(box.center+boxOffset, box.size*.8f);
        }

#endif
    }
}