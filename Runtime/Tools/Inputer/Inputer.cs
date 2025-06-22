using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EC.Input
{
    [HideMonoScript]
    public class Inputer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
    {

        #region PLATFORM
        private enum ActivePlatform { Auto, PC, Mobile }
        [SerializeField, BoxGroup("Platform", ShowLabel = false), LabelWidth(80), ReadOnly] private ActivePlatform Platform;
        private void Awake()
        {
            if (Platform == ActivePlatform.Auto)
            {
#if UNITY_EDITOR
                Platform = ActivePlatform.PC;
#elif PLATFORM_ANDROID
            Platform = ActivePlatform.Mobile;
#endif
            }
        }
        #endregion

        #region RESULT EVENT
        private enum EventVariant { Enter, Exit, Down, Up, Click, BeginDrag, Drag, EndDrag, Swipe, Zoom }
        [SerializeField] private EventData[] _events;

        [System.Serializable]
        private struct EventData
        {
            private enum ResultType { Clear, Vector2, Float }
            private readonly bool IsTargetType(EventVariant eventType) => eventType == _event;

            [SerializeField, BoxGroup("El", ShowLabel = false), HideLabel] private EventVariant _event;
            [SerializeField, BoxGroup("El"), LabelText("Result")] private ResultData[] _results;

            [System.Serializable]
            private struct ResultData
            {
                private enum ResultVariant { Event, Bus, Effector }
                private enum SourceVariant { Source, Custom }
                private enum EffectorType { Smooth, Moment }
                private enum Types { Simple, Int, Float, Bool, String, Vector2, Vector3, GameObject, Object }

                [SerializeField, LabelText("Result"), LabelWidth(50), HorizontalGroup("_type")] private ResultVariant _result;
                [SerializeField, LabelText("Data"), LabelWidth(50), HorizontalGroup("_type"), ShowIf("@_result == ResultVariant.Bus || _result == ResultVariant.Effector")] private SourceVariant _source;
                [HideInInspector] public ResultType _resultType;


                [SerializeField, LabelText("Event"), ShowIf("@_result == ResultVariant.Event && _resultType == ResultType.Clear")] private UnityEvent _eventClear;
                [SerializeField, LabelText("Event"), ShowIf("@_result == ResultVariant.Event && _resultType == ResultType.Vector2")] private UnityEvent<Vector2> _eventVector2;
                [SerializeField, LabelText("Event"), ShowIf("@_result == ResultVariant.Event && _resultType == ResultType.Float")] private UnityEvent<float> _eventFloat;

                [SerializeField, HorizontalGroup("Effector"), LabelText("Effector"), LabelWidth(50), ShowIf("@_result == ResultVariant.Effector")] private Effects.EffectorStates _effector;
                [SerializeField, HorizontalGroup("Effector"), LabelText("_type"), LabelWidth(50), ShowIf("@_result == ResultVariant.Effector")] private EffectorType _effectorType;
                [SerializeField, HorizontalGroup("Effector"), LabelText("Key"), LabelWidth(50), ShowIf("@_result == ResultVariant.Effector && _source == SourceVariant.Custom")] private string _effectorKey;

                [SerializeField, HorizontalGroup("Bus"), LabelText("Key"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus")] private string _busKey;
                [SerializeField, HorizontalGroup("Bus"), LabelText("_type"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom")] private Types _busType;

                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Int")] private int _busInt;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Float")] private float _busFloat;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Bool")] private bool _busBool;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.String")] private string _busString;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Vector2")] private Vector2 _busVector2;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Vector3")] private Vector3 _busVector3;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.GameObject")] private GameObject _busGameObject;
                [SerializeField, LabelText("Value"), LabelWidth(50), ShowIf("@_result == ResultVariant.Bus && _source == SourceVariant.Custom && _busType == Types.Object")] private Object _busObject;

                #region InvokeEvent()
                public void InvokeEvent(EventVariant eventType, Vector2 vectorValue, float floatValue)
                {
                    switch (_result)
                    {
                        case ResultVariant.Event:
                            switch (_resultType)
                            {
                                case ResultType.Clear: _eventClear?.Invoke(); break;
                                case ResultType.Vector2: _eventVector2?.Invoke(vectorValue); break;
                                case ResultType.Float: _eventFloat?.Invoke(floatValue); break;
                            }
                            break;
                        case ResultVariant.Bus:
                            switch (_source)
                            {
                                case SourceVariant.Source:
                                    switch (_resultType)
                                    {
                                        case ResultType.Clear: Bus.BusSystem.Invoke(_busKey); break;
                                        case ResultType.Vector2: Bus.BusSystem.Set<Vector2>(_busKey, vectorValue); break;
                                        case ResultType.Float: Bus.BusSystem.Set<float>(_busKey, floatValue); break;
                                    }
                                    break;
                                case SourceVariant.Custom:
                                    switch (_busType)
                                    {
                                        case Types.Simple: Bus.BusSystem.Invoke(_busKey); break;
                                        case Types.Int: Bus.BusSystem.Set<int>(_busKey, _busInt); break;
                                        case Types.Float: Bus.BusSystem.Set<float>(_busKey, _busFloat); break;
                                        case Types.Bool: Bus.BusSystem.Set<bool>(_busKey, _busBool); break;
                                        case Types.String: Bus.BusSystem.Set<string>(_busKey, _busString); break;
                                        case Types.Vector2: Bus.BusSystem.Set<Vector2>(_busKey, _busVector2); break;
                                        case Types.Vector3: Bus.BusSystem.Set<Vector3>(_busKey, _busVector3); break;
                                        case Types.GameObject: Bus.BusSystem.Set<GameObject>(_busKey, _busGameObject); break;
                                        case Types.Object: Bus.BusSystem.Set<Object>(_busKey, _busObject); break;
                                    }
                                    break;
                            }
                            break;
                        case ResultVariant.Effector:
                            string key = _source == SourceVariant.Source ? eventType.ToString() : _effectorKey;
                            switch (_effectorType)
                            {
                                case EffectorType.Smooth:
                                    _effector.PlaySmooth(key); break;
                                case EffectorType.Moment:
                                    _effector.PlayMoment(key); break;
                            }
                            break;
                    }
                }
                #endregion
            }

            #region InvokeEvent()
            public void InvokeEvent(EventVariant eventType, Vector2 vectorValue, float floatValue)
            {
                if (!IsTargetType(eventType)) return;
                foreach (var ev in _results)
                    ev.InvokeEvent(eventType, vectorValue, floatValue);
            }
            #endregion


#if UNITY_EDITOR
            public void OnValidate()
            {
                if (_results.Length > 0)
                {
                    ResultType type = _event switch
                    {
                        EventVariant.Swipe or EventVariant.Drag => ResultType.Vector2,
                        EventVariant.Zoom => ResultType.Float,
                        _ => ResultType.Clear
                    };
                    for (int i = 0; i < _results.Length; i++)
                        if (_results[i]._resultType != type)
                            _results[i]._resultType = type;
                }
            }
#endif
        }

        #region InvokeEvent()
        private void InvokeEvent(EventVariant eventType, float value)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType, default, value);
        }
        private void InvokeEvent(EventVariant eventType, Vector2 value)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType, value, default);
        }
        private void InvokeEvent(EventVariant eventType)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType, default, default);
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            foreach (var ev in _events)
                ev.OnValidate();
        }
#endif
        #endregion

        #region TOUCHES
        private readonly TouchManager _touches = new(1, 2);
        private UnityEngine.Coroutine _drag;
        private UnityEngine.Coroutine _zoom;

        #region Enter | Exit
        public void OnPointerEnter(PointerEventData eventData) => Enter();
        public void OnPointerExit(PointerEventData eventData) => Exit(eventData.pointerId);

        private void Enter()
        {
            InvokeEvent(EventVariant.Enter);
        }
        private void Exit(int id)
        {
            InvokeEvent(EventVariant.Exit);
            if (!_touches.TryGetTouchById(id, out var touch)) return;
            Up(id);
            _touches.RemoveTouch(id);
            CheckDragActive();
            CheckZoomActive();
        }
        #endregion

        #region Down | Up | Click
        public void OnPointerDown(PointerEventData eventData) => Down(eventData.pointerId, eventData.pressPosition);
        public void OnPointerUp(PointerEventData eventData) => Up(eventData.pointerId);
        public void OnPointerClick(PointerEventData eventData) => Click(eventData.pointerId);

        private void Down(int id, Vector2 position)
        {
            if (!_touches.AddTouch(id, position)) return;
            InvokeEvent(EventVariant.Down);
            CheckDragActive();
            CheckZoomActive();
        }
        private void Up(int id)
        {
            if (!_touches.TryGetTouchById(id, out var touch)) return;
            InvokeEvent(EventVariant.Up);
        }
        private void Click(int id)
        {
            if (!_touches.TryGetTouchById(id, out var touch)) return;
            _touches.RemoveTouch(id);
            CheckDragActive();
            CheckZoomActive();
            Swipe(touch.StartPosition, touch.CurrentPosition);
            InvokeEvent(EventVariant.Click);
        }
        #endregion

        #region Move
        public void OnPointerMove(PointerEventData eventData) => Move(eventData.pointerId, eventData.position);

        private void Move(int id, Vector2 position)
        {
            _touches.UpdateTouch(id, position);
        }
        #endregion

        #region Swipe
        private void Swipe(Vector2 startPosition, Vector2 endPosition)
        {
            if (Vector2.Distance(startPosition, endPosition) < 25f) return;
            Vector2 dir = (endPosition - startPosition).normalized;
            Vector2 swipeDir = Vector2.zero;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                swipeDir = dir.x > 0 ? Vector2.right : -Vector2.right;
            else
                swipeDir = dir.y > 0 ? Vector2.up : -Vector2.up;
            InvokeEvent(EventVariant.Swipe, swipeDir);
        }
        #endregion

        #region Drag
        private void CheckDragActive()
        {
            if (_touches.GetTouchCount() == 1)
            {
                BeginDrag();
                _drag = StartCoroutine(DragUpdate());
            }
            else if (_drag != null)
            {
                StopCoroutine(_drag);
                _drag = null;
                EndDrag();
            }
        }
        private IEnumerator DragUpdate()
        {
            _touches.TryGetTouchByNo(0, out TouchManager.TouchData touch);
            bool isDrag = false;
            Vector2 lastPosition = Vector2.zero;
            while (true)
            {
                if (isDrag)
                {
                    Drag(touch.CurrentPosition - lastPosition);
                    lastPosition = touch.CurrentPosition;
                }
                else if (Vector2.Distance(touch.StartPosition, touch.CurrentPosition) > 5f)
                {
                    isDrag = true;
                    lastPosition = touch.CurrentPosition;
                }
                yield return null;
            }
        }
        public void BeginDrag()
        {
            InvokeEvent(EventVariant.BeginDrag);
        }
        public void Drag(Vector2 delta)
        {
            InvokeEvent(EventVariant.Drag, delta);
        }
        public void EndDrag()
        {
            InvokeEvent(EventVariant.EndDrag);
        }
        #endregion

        #region Zoom
        private void CheckZoomActive()
        {
            if (_touches.GetTouchCount() == 2)
                _zoom = StartCoroutine(ZoomUpdate());
            else if (_zoom != null)
            {
                StopCoroutine(_zoom);
                _zoom = null;
            }
        }
        private IEnumerator ZoomUpdate()
        {
            _touches.TryGetTouchByNo(0, out TouchManager.TouchData touch1);
            _touches.TryGetTouchByNo(1, out TouchManager.TouchData touch2);

            float lastDistance = Vector2.Distance(touch1.CurrentPosition, touch2.CurrentPosition);
            float nowDistance = lastDistance;
            while (true)
            {
                nowDistance = Vector2.Distance(touch1.CurrentPosition, touch2.CurrentPosition);
                Zoom(nowDistance - lastDistance);
                lastDistance = nowDistance;
                yield return null;
            }
        }
        private void Zoom(float delta)
        {
            InvokeEvent(EventVariant.Zoom, delta);
        }
        #endregion

        private void OnDisable()
        {
            foreach (var touch in _touches.GetActiveTouchIds())
            {
                Up(touch);
                Click(touch);
            }
        }
        #endregion
    }
}