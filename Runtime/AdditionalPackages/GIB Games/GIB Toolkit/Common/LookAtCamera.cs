using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Toolkit
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            transform.LookAt(_camera);
        }
    }
}
