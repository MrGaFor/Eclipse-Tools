using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EC.Inputer
{
    public enum EventVariant { Enter, Exit, Down, Up, Click, BeginDrag, Drag, EndDrag, Swipe, Zoom }
    [HideMonoScript]
    public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
    {
        #region RESULT EVENT
        [SerializeField] private List<InputComponent> _events;

        #region InvokeEvent()
        private void InvokeEvent(EventVariant eventType, float value)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType, value);
        }
        private void InvokeEvent(EventVariant eventType, Vector2 value)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType, value);
        }
        private void InvokeEvent(EventVariant eventType)
        {
            foreach (var el in _events)
                el.InvokeEvent(eventType);
        }
        #endregion
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

        #region EDITOR
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            InputComponent[] components = gameObject.GetComponents<InputComponent>();
            bool haveChanges = false;
            if (_events.Count > 0)
                for (int i = _events.Count - 1; i >= 0; i--)
                    if (!_events[i])
                    {
                        _events.RemoveAt(i);
                        haveChanges = true;
                    }
            foreach (var component in components)
                if (!_events.Contains(component))
                {
                    _events.Add(component);
                    haveChanges = true;
                }
            if (haveChanges)
                UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion
    }
}