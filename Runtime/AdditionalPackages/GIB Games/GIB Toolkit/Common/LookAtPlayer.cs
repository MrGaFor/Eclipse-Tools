using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Toolkit
{
    public class LookAtPlayer : MonoBehaviour
    {
        private Transform _player;

        private void Start()
        {
            GameObject _playerGo = GameObject.FindGameObjectWithTag("Player");
            if (_playerGo == null)
            {
                GIBUtils.Warn("LookAtPlayer unable to find object with Player tag, destroying.");
                Destroy(gameObject);
            }
            else
            {
                _player = _playerGo.transform;
            }
        }

        private void FixedUpdate()
        {
            transform.LookAt(_player);
        }
    }
}
