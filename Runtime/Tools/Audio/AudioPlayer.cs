using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EC.Audio
{
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

        [SerializeField, BoxGroup("3D", ShowLabel = false)] private bool _use3D = false;
        [SerializeField, BoxGroup("3D"), ShowIf("_use3D"), Range(0f, 5f)] private float _dopplerLevel = 1f;
        [SerializeField, BoxGroup("3D"), ShowIf("_use3D"), Range(0f, 360f)] private float _spread = 0f;
        [SerializeField, BoxGroup("3D"), ShowIf("_use3D"), Min(0)] private float _minDistance = 0f;
        [SerializeField, BoxGroup("3D"), ShowIf("_use3D")] private float _maxDistance = 30f;
        [SerializeField, BoxGroup("3D"), ShowIf("_use3D")] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField, BoxGroup("3D"), ShowIf("@_rolloffMode == AudioRolloffMode.Custom && _use3D")] private AnimationCurve _customRolloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private Pool.ComponentPool<AudioSource> _pool;
        private Dictionary<AudioSource, UnityEngine.Coroutine> _sourceDisableTimers = new();

        private async void Awake()
        {
            if (_pool != null) _pool.RemoveAll();
            AudioSource source = new GameObject("Source").AddComponent<AudioSource>();
            source.transform.SetParent(transform);
            source.gameObject.SetActive(false);
            source.playOnAwake = false;
            source.clip = _clipSource == ClipSource.Local ? _localClip : await _addressableClip.GetObject();
            source.loop = _loop;
            if (_use3D)
            {
                source.spatialBlend = 1f;
                source.dopplerLevel = _dopplerLevel;
                source.spread = _spread;
                source.minDistance = _minDistance;
                source.maxDistance = _maxDistance;
                source.rolloffMode = _rolloffMode;
                if (_rolloffMode == AudioRolloffMode.Custom) source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _customRolloffCurve);
            }

            _pool = new Pool.ComponentPool<AudioSource>(source, transform);
        }


        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<float>(_volumeKey, OnChangeVolume);
            ResumeAll();
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<float>(_volumeKey, OnChangeVolume);
            PauseAll();
        }
        public void OnDestroy()
        {
            StopAll();
        }
        private void OnChangeVolume(float newVolume)
        {
            if (_pool == null) return;
            foreach (var source in _pool.GetAll())
                if (source.isActiveAndEnabled)
                    source.volume = newVolume * _volumeMult;
        }


        public void Play()
        {
            if (_pool == null) return;
            AudioSource source = _pool.GetOne();
            SetParametersAndPlay(source);
            CheckLoop(source);
        }
        public void Play(AudioClip clip)
        {
            if (_pool == null) return;
            AudioSource source = _pool.GetOne();
            source.clip = clip;
            SetParametersAndPlay(source);
            CheckLoop(source);
        }
        public void Play(out AudioSource source)
        {
            if (_pool == null) { source = null; return; }
            source = _pool.GetOne();
            SetParametersAndPlay(source);
            CheckLoop(source);
        }
        public void Play(AudioClip clip, out AudioSource source)
        {
            if (_pool == null) { source = null; return; }
            source = _pool.GetOne();
            source.clip = clip;
            SetParametersAndPlay(source);
            CheckLoop(source);
        }

        public void Pause(AudioSource source)
        {
            if (!source.gameObject.activeSelf) return;
            if (!_loop && _sourceDisableTimers.TryGetValue(source, out UnityEngine.Coroutine timer))
                StopCoroutine(timer);
            source.Pause();
        }
        public void PauseAll()
        {
            if (_pool == null) return;
            foreach (AudioSource source in _pool.GetAll())
                Pause(source);
        }

        public void Resume(AudioSource source)
        {
            if (!source.gameObject.activeSelf) return;
            if (!_loop && _sourceDisableTimers.ContainsKey(source))
                _sourceDisableTimers[source] = StartCoroutine(EndTimerDisable(source));
            source.UnPause();
        }
        public void ResumeAll()
        {
            if (_pool == null) return;
            foreach (AudioSource source in _pool.GetAll())
                Resume(source);
        }

        public void Stop(AudioSource source)
        {
            if (!_loop && _sourceDisableTimers.TryGetValue(source, out UnityEngine.Coroutine timer))
            {
                StopCoroutine(timer);
                _sourceDisableTimers.Remove(source);
            }
            source.Stop();
            source.gameObject.SetActive(false);
        }
        public void StopAll()
        {
            if (_pool == null) return;
            foreach (AudioSource source in _pool.GetAll())
                Stop(source);
        }

        private void SetParametersAndPlay(AudioSource source)
        {
            source.volume = Bus.BusSystem.Get<float>(_volumeKey, 1f) * _volumeMult;
            source.pitch = _pitchRandom ? Random.Range(_pitchRange.x, _pitchRange.y) : _pitch;
            source.Play();
        }
        private void CheckLoop(AudioSource source)
        {
            if (!_loop) _sourceDisableTimers.Add(source, StartCoroutine(EndTimerDisable(source)));
        }
        private IEnumerator EndTimerDisable(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length - source.time);
            _sourceDisableTimers.Remove(source);
            source.gameObject.SetActive(false);
        }


        #region EDITOR
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_clipSource == ClipSource.Addressable) _localClip = null;
            if (_clipSource == ClipSource.Local) _addressableClip = null;
        }
        public void OnDrawGizmosSelected()
        {
            if (_use3D)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, _minDistance);
                Gizmos.DrawWireSphere(transform.position, _maxDistance);
            }
        }
#endif
        #endregion
    }
}
