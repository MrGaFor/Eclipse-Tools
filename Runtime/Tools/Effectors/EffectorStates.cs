using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorStates : MonoBehaviour
    {
        #region Automatic
        private enum EnableType { Awake, OnEnable, Start }
        private enum EnableVariant { Moment, Smooth }
        [SerializeField, BoxGroup("Automatic", ShowLabel = false), HorizontalGroup("Automatic/enable", Width = 130), LabelText("Is Automatic"), LabelWidth(100)] private bool _isAutomatic;
        [SerializeField, BoxGroup("Automatic"), HorizontalGroup("Automatic/enable"), ShowIf("_isAutomatic"), LabelText("Call"), LabelWidth(50)] private EnableType _automaticCall;
        [SerializeField, BoxGroup("Automatic"), HorizontalGroup("Automatic/enable"), ShowIf("_isAutomatic"), LabelText("Method"), LabelWidth(50)] private EnableVariant _automaticMethod;
        [SerializeField, BoxGroup("Automatic"), HorizontalGroup("Automatic/value"), ShowIf("_isAutomatic"), PropertyRange(0, "@Mathf.Max(0, _states.Length - 1)"), LabelWidth(70), LabelText("From")] private int _automaticFrom;
        [SerializeField, BoxGroup("Automatic"), HorizontalGroup("Automatic/value"), ShowIf("_isAutomatic"), PropertyRange(0, "@Mathf.Max(0, _states.Length - 1)"), LabelWidth(70), LabelText("To")] private int _automaticTo;

        private void Awake()
        {
            if (_automaticCall == EnableType.Awake)
                EnableCall();
        }
        private void OnEnable()
        {
            if (_automaticCall == EnableType.OnEnable)
                EnableCall();
        }
        private void Start()
        {
            if (_automaticCall == EnableType.Start)
                EnableCall();
        }
        private void EnableCall()
        {
            if (!_isAutomatic) return;
            PlayMoment(_automaticFrom);
            if (_automaticMethod == EnableVariant.Moment) PlayMoment(_automaticTo);
            else if (_automaticMethod == EnableVariant.Smooth) PlaySmooth(_automaticTo);
        }
        #endregion

        #region Data
        [SerializeField, BoxGroup("States", ShowLabel = false), HorizontalGroup("States/startSt"), HideInPlayMode, PropertyRange(0, "GetMaxStateId")] private int _startState;
        #region Editor
#if UNITY_EDITOR
        [SerializeField, BoxGroup("States"), HorizontalGroup("States/startSt", Width = 25), HideInPlayMode, Button("<")] private void EditorPrevState() { _startState = (int)Mathf.Repeat(_startState - 1, GetStatesCount()); OnValidate(); }
        [SerializeField, BoxGroup("States"), HorizontalGroup("States/startSt", Width = 25), HideInPlayMode, Button(">")] private void EditorNextState() { _startState = (int)Mathf.Repeat(_startState + 1, GetStatesCount()); OnValidate(); }
#endif
        #endregion
        [SerializeField, BoxGroup("States"), HideInEditorMode, ReadOnly] private int _state;
        [SerializeField, BoxGroup("States", ShowLabel = false)] private EffectorsState[] _states;

        [System.Serializable]
        public class EffectorsState
        {
            [SerializeField, LabelWidth(70)] private string _id; public string ID => _id;
            [SerializeField] private IEffectorComponent[] _effects;
            [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/hor")] private UnityEvent _onPlay;
            [SerializeField, FoldoutGroup("Events"), HorizontalGroup("Events/hor")] private UnityEvent _onComplete;
            private Tween _completeDelay;

            #region IEffectorComponent
            private void CallOnPlay()
            {
                Stop();
                _onPlay?.Invoke();
            }
            private void CallOnComplete()
            {
                _onComplete?.Invoke();
            }
            public void PlayMoment()
            {
                CallOnPlay();
                foreach (var effect in _effects)
                    effect.PlayMoment();
                CallOnComplete();
            }
            public void PlaySmooth()
            {
                CallOnPlay();
                foreach (var effect in _effects)
                    effect.PlaySmooth();
                _completeDelay = Tween.Delay(_effects.Max(v => v.Data.Time.AllDuration), () => CallOnComplete());
            }
            public async Task PlaySmoothAsync()
            {
                CallOnPlay();
                foreach (var effect in _effects)
                    effect.PlaySmooth();
                await UniTask.Delay(Mathf.RoundToInt(_effects.Max(v => v.Data.Time.AllDuration) * 1000f));
                CallOnComplete();
            }
            public void Stop()
            {
                foreach (var effect in _effects)
                    effect.Stop();
                if (_completeDelay.isAlive) _completeDelay.Stop();
            }
            public void Pause()
            {
                foreach (var effect in _effects)
                    effect.Pause();
                if (_completeDelay.isAlive) _completeDelay.isPaused = true;
            }
            public void Resume()
            {
                foreach (var effect in _effects)
                    effect.Resume();
                if (_completeDelay.isAlive) _completeDelay.isPaused = false;
            }
            #endregion
        }
        private int GetStatesCount() => _states != null ? _states.Length : 0;
        private int GetMaxStateId() => GetStatesCount() - 1;

        public int GetState() => _state;
        public string GetStateId() => _states[_state].ID;
        public bool CheckIsState(int state) => state == _state;
        public bool CheckIsState(string idstate) => _states[_state].ID == idstate;
        #endregion

        #region IEffectorComponent
        #region Moment
        public void PlayMoment(bool firstSecond)
        {
            if (_states.Length < 2)
            {
                Debug.LogErrorFormat("Effects: States massive size less than 2!", this);
                return;
            }
            if (firstSecond)
                PlayMoment(0);
            else
                PlayMoment(1);
        }
        public void PlayMoment(string idstate)
        {
            for (int i = 0; i < _states.Length; i++)
                if (_states[i].ID == idstate)
                {
                    PlayMoment(i);
                    return;
                }
            Debug.LogErrorFormat("Effects: State dont include in massive!", this);
        }
        public void PlayMoment(int state)
        {
            Stop();
            if (state < 0 || state >= _states.Length)
            {
                Debug.LogErrorFormat("Effects: State out massive size!", this);
                return;
            }
            _state = state;
            _states[_state].PlayMoment();
        }
        #endregion
        #region Smooth
        public void PlaySmooth(bool firstSecond)
        {
            if (_states.Length < 2)
            {
                Debug.LogErrorFormat("Effects: States massive size less than 2!", this);
                return;
            }
            if (firstSecond)
                PlaySmooth(0);
            else
                PlaySmooth(1);
        }
        public async UniTask PlaySmoothAsync(bool firstSecond)
        {
            if (_states.Length < 2)
            {
                Debug.LogErrorFormat("Effects: States massive size less than 2!", this);
                return;
            }
            if (firstSecond)
                await PlaySmoothAsync(0);
            else
                await PlaySmoothAsync(1);
        }
        public void PlaySmooth(string idstate)
        {
            for (int i = 0; i < _states.Length; i++)
                if (_states[i].ID == idstate)
                {
                    PlaySmooth(i);
                    return;
                }
            Debug.LogErrorFormat("Effects: State dont include in massive!", this);
        }
        public async UniTask PlaySmoothAsync(string idstate)
        {
            for (int i = 0; i < _states.Length; i++)
                if (_states[i].ID == idstate)
                {
                    await PlaySmoothAsync(i);
                    return;
                }
            Debug.LogErrorFormat("Effects: State dont include in massive!", this);
        }
        public void PlaySmooth(int state)
        {
            Stop();
            if (state < 0 || state >= _states.Length)
            {
                Debug.LogErrorFormat("Effects: State out massive size!", this);
                return;
            }
            _state = state;
            _states[_state].PlaySmooth();
        }
        public async UniTask PlaySmoothAsync(int state)
        {
            Stop();
            if (state < 0 || state >= _states.Length)
            {
                Debug.LogErrorFormat("Effects: State out massive size!", this);
                return;
            }
            _state = state;
            await _states[_state].PlaySmoothAsync();
        }
        #endregion
        #region Pause|Resume
        public void Pause()
        {
            _states[_state].Pause();
        }
        public void Resume()
        {
            _states[_state].Resume();
        }
        #endregion
        #region Stop
        public void Stop()
        {
            _states[_state].Stop();
        }
        #endregion
        #endregion

        #region Editor
#if UNITY_EDITOR
        [SerializeField, HorizontalGroup("st"), HideInEditorMode, Button("<Prev"), PropertyOrder(-1)] private void PlayPrevState() { PlaySmooth((int)Mathf.Repeat(_state - 1, GetStatesCount())); }
        [SerializeField, HorizontalGroup("st"), HideInEditorMode, Button("Next>"), PropertyOrder(-1)] private void PlayNextState() { PlaySmooth((int)Mathf.Repeat(_state + 1, GetStatesCount())); }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_states == null || _states.Length == 0) return;
            if (_state != _startState)
                PlayMoment(_startState);
        }
#endif
        #endregion
    }
}