using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class Audio3DSettings
    {

        public void ApplyTo(AudioSource source)
        {
        }

        #region EDITOR
#if UNITY_EDITOR
        public void OnDrawGizmos(Vector3 position)
        {
        }
#endif
        #endregion
    }
}
