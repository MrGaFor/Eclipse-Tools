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
        private enum ClipSource { Local, Addressable }
        [SerializeField, BoxGroup("Settings", ShowLabel = false), HorizontalGroup("Settings/Clip", MinWidth = 200), LabelWidth(100)] private ClipSource _clipSource;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/Clip"), ShowIf("_clipSource", ClipSource.Local), HideLabel] private AudioClip _localClip;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/Clip"), ShowIf("_clipSource", ClipSource.Addressable), HideLabel] private Addressables.AddressablesDataGeneric<AudioClip> _addressableClip;

        [SerializeField, HorizontalGroup("Settings/Volume", MinWidth = 200), ValueDropdown("VolumeKeys", AppendNextDrawer = true), LabelWidth(100), LabelText("Volume")] private string _volumeKey = "Sound";
        private string[] VolumeKeys => new[] { _volumeKey, "Sound", "Music" };
        [SerializeField, HorizontalGroup("Settings/Volume", MinWidth = 200), Range(0f, 1f), LabelWidth(55), LabelText("Multiple")] private float _volumeMult = 1f;

        [SerializeField, BoxGroup("Settings")] private bool _loop = false;

        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/Pitch", MinWidth = 200), HideIf("_pitchRandom"), LabelWidth(100), Range(-3, 3)] private float _pitch = 1f;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/Pitch", MinWidth = 200), ShowIf("_pitchRandom"), LabelWidth(100), MinMaxSlider(-3, 3, ShowFields = true)] private Vector2 _pitchRange = Vector2.one;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/Pitch", 125), LabelText("Use Range"), LabelWidth(100)] private bool _pitchRandom = false;




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
