using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioSourceSettings : AudioModule
    {
        [SerializeField, ListDrawerSettings(DraggableItems = false)] private AudioClip[] _localClip;

        public async UniTask OnCreate()
        {

        }
        public async UniTask Apply(AudioSource source)
        {
            source.clip = _localClip[Random.Range(0, _localClip.Length)];
        }

#if UNITY_EDITOR
        public void OnDrawGizmosSelected(Vector3 position)
        {

        }
#endif
    }
}
