using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioSourceSettings : AudioModule
    {
        private enum ClipSource { Local, Addressable }
        [SerializeField, HorizontalGroup("Clip", MinWidth = 200), LabelWidth(100)] private ClipSource _source;
        [SerializeField, HorizontalGroup("Clip", 80), ShowIf("_source", ClipSource.Addressable), LabelWidth(55), LabelText("OnStart")] private bool _startLoad;
        [SerializeField, ShowIf("_source", ClipSource.Local), ListDrawerSettings(DraggableItems = false)] private AudioClip[] _localClip;
        [SerializeField, ShowIf("_source", ClipSource.Addressable), ListDrawerSettings(DraggableItems = false)] private Addressables.AddressablesDataGeneric<AudioClip>[] _addressableClip;

        public async UniTask OnCreate()
        {
            if (_source == ClipSource.Addressable && _startLoad) foreach (var clip in _addressableClip) await clip.Load();
        }
        public async UniTask Apply(AudioSource source)
        {
            source.clip = _source == ClipSource.Local ? _localClip[Random.Range(0, _localClip.Length)] : await _addressableClip[Random.Range(0, _addressableClip.Length)].GetObject();
        }

#if UNITY_EDITOR
        public void OnDrawGizmosSelected(Vector3 position)
        {
            if (Application.isPlaying) return;
            if (_source == ClipSource.Addressable && _localClip != null) _localClip = null;
            if (_source == ClipSource.Local && _addressableClip != null) _addressableClip = null;
        }
#endif
    }
}
