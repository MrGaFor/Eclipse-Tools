using Sirenix.OdinInspector;
using UnityEngine;

namespace GIB.Toolkit
{
    /// <summary>
    /// Freezes and unfreezes a <a href="https://docs.unity3d.com/ScriptReference/Rigidbody.html">Rigidbody</a>.
    /// </summary>
    public class RigidbodyFreeze : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private bool freezeOnStart;

        private void Start()
        {
            if (freezeOnStart)
                Freeze();
        }

        /// <summary>
        /// Unfreezes Rigidbody by removing all constraints.
        /// </summary>
        [Button]
        public void Unfreeze()
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
        }

        /// <summary>
        /// Freezes Rigidbody by applying all constraints.
        /// </summary>
        [Button]
        public void Freeze()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
