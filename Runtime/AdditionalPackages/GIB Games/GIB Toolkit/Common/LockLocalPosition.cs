
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Toolkit
{
    /// <summary>
    /// Locks a position in local space.
    /// </summary>
    public class LockLocalPosition : MonoBehaviour
    {
        public bool LockX;
        public bool LockY;
        public bool LockZ;

        [SerializeField] private bool locked;

        [SerializeField] private Vector3 startPos;

        private void Start()
        {
            startPos = transform.localPosition;
        }

        private void FixedUpdate()
        {
            if (locked)
                transform.localPosition = startPos;
        }



    }
}