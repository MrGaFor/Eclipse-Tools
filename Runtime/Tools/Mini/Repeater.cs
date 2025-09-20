using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;

namespace EC.Mini
{
    [HideMonoScript]
    public class Repeater : MonoBehaviour
    {
        [SerializeField, BoxGroup("ev", false), LabelWidth(70), HorizontalGroup("ev/duration")] private Vector2 _duration = Vector2.zero;
        [SerializeField, BoxGroup("ev"), LabelWidth(70), HorizontalGroup("ev/interval")] private Vector2 _interval = Vector2.one;
        private enum ActiveType { Manual, Auto }
        [SerializeField, BoxGroup("ev"), LabelWidth(70)] private ActiveType _activeType = ActiveType.Auto;
        private enum State { Wait, Process }
        [SerializeField, BoxGroup("ev"), LabelWidth(70), HideInEditorMode, ReadOnly] private State _state = State.Wait;
#if UNITY_EDITOR
        [SerializeField, BoxGroup("ev"), LabelText("Avg"), LabelWidth(30), ReadOnly, HorizontalGroup("ev/interval", 70)] private float _intervalAvg;
        [SerializeField, BoxGroup("ev"), LabelText("Avg"), LabelWidth(30), ReadOnly, HorizontalGroup("ev/duration", 70)] private float _durationAvg;
        [SerializeField, BoxGroup("ev"), HideLabel, ReadOnly, MinMaxSlider(0f, @"AllTime", false)] private Vector2 _previewDuration = Vector2.zero;
        private float AllTime => _intervalAvg + _durationAvg;
        public void OnValidate()
        {
            _intervalAvg = _interval.x + (_interval.y - _interval.x) * 0.5f;
            _durationAvg = _duration.x + (_duration.y - _duration.x) * 0.5f;
            _previewDuration.x = 0f;
            _previewDuration.y = _durationAvg;
        }
#endif
        [SerializeField, BoxGroup("ev"), FoldoutGroup("ev/Events"), HorizontalGroup("ev/Events/hor")] private UnityEngine.Events.UnityEvent _onEnter;
        [SerializeField, BoxGroup("ev"), FoldoutGroup("ev/Events"), HorizontalGroup("ev/Events/hor")] private UnityEngine.Events.UnityEvent _onExit;

        private UnityEngine.Coroutine _repeaterCoroutine;

        public void OnEnable()
        {
            if (_activeType == ActiveType.Auto)
                StartRepeater();
        }
        public void OnDisable()
        {
            StopRepeater();
        }

        public void StartRepeater()
        {
            StopRepeater();
            _repeaterCoroutine = StartCoroutine(RepeaterCoroutine());
        }
        public void StopRepeater()
        {
            if (_repeaterCoroutine != null)
            {
                StopCoroutine(_repeaterCoroutine);
                if (_state == State.Process)
                {
                    _onExit?.Invoke();
                    _state = State.Wait;
                }
            }
        }

        private IEnumerator RepeaterCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_interval.x, _interval.y));
                _onEnter?.Invoke();
                _state = State.Process;
                yield return new WaitForSeconds(Random.Range(_duration.x, _duration.y));
                _onExit?.Invoke();
                _state = State.Wait;
            }
        }
    }
}
