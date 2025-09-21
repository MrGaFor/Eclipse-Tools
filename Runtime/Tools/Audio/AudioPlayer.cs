using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    public abstract class AudioModule
    {

    }
    [HideMonoScript]
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField, BoxGroup("Source", false), HideLabel] private AudioSourceSettings _sourceSettings;
        [SerializeField, BoxGroup("Volume", false), HideLabel] private AudioVolumeSettings _volumeSettings;
        [SerializeField, BoxGroup("Fade", false), HideLabel] private AudioFadeSettings _fadeSettings;
        [SerializeField, BoxGroup("Pitch", false), HideLabel] private AudioPitchSettings _pitchSettings;
        [SerializeField, BoxGroup("Loop", false), HideLabel] private AudioLoopSettings _loopSettings;
        [SerializeField, BoxGroup("3D", false), HideLabel] private Audio3DSettings _3dSettings;

        private Pool.ComponentPool<AudioSource> _pool;

        private void Awake()
        {
            if (_pool != null) _pool.RemoveAll();
            AudioSource source = new GameObject("Source").AddComponent<AudioSource>();
            source.transform.SetParent(transform);
            source.gameObject.SetActive(false);
            source.playOnAwake = false;
            _pool = new Pool.ComponentPool<AudioSource>(source, transform);
            _sourceSettings.OnCreate().Forget();
        }

        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<float>(_volumeSettings.Key, OnChangeVolume);
            OnChangeVolume(Bus.BusSystem.Get<float>(_volumeSettings.Key, 1f));
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<float>(_volumeSettings.Key, OnChangeVolume);
            Stop();
        }
        private void OnChangeVolume(float volume)
        {
            if (_pool == null) return;
            foreach (var source in _pool.GetAll())
                if (source.isActiveAndEnabled) 
                    _volumeSettings.Apply(source);
        }
        public void OnDestroy()
        {
            Stop();
        }

        private async UniTask SetParametersAndPlay(AudioSource source)
        {
            await _sourceSettings.Apply(source);
            _volumeSettings.Apply(source);
            _pitchSettings.Apply(source);
            _fadeSettings.Apply(source);
            _loopSettings.Apply(source, _fadeSettings);
            source.time = 0f;
            source.Play();
        }

        public void Play()
        {
            SetParametersAndPlay(_pool.GetOne()).Forget();
        }
        public void Stop()
        {
            if (_pool == null) return;
            foreach (AudioSource source in _pool.GetAll())
            {
                _loopSettings.Stop(source, _fadeSettings);
            }
        }

        #region EDITOR
#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            _sourceSettings.OnDrawGizmosSelected(transform.position);
            _3dSettings.OnDrawGizmosSelected(transform.position);
        }
#endif
        #endregion
    }
}
