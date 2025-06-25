using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EC.Inputer
{
    public class TouchManager
    {
        public class TouchData
        {
            public bool IsActive { get; private set; }
            public int ID { get; private set; }

            public Vector2 StartPosition { get; private set; }
            public Vector2 CurrentPosition { get; private set; }

            public void Activate(int id, Vector2 startPos)
            {
                IsActive = true;
                ID = id;

                StartPosition = startPos;
                CurrentPosition = startPos;
            }

            public void UpdatePosition(Vector2 newPos)
            {
                CurrentPosition = newPos;
            }

            public void Deactivate()
            {
                IsActive = false;
            }
        }

        private readonly List<TouchData> _touches = new();
        private readonly int _maxTouches;
        private readonly List<int> _activeTouches = new List<int>();
        private int _touchCount = 0;

        public TouchManager(int initialPoolSize, int maxActiveTouches)
        {
            _maxTouches = maxActiveTouches;

            for (int i = 0; i < initialPoolSize; i++)
                _touches.Add(new TouchData());
        }

        public bool AddTouch(int id, Vector2 position)
        {
            if (GetTouchCount() >= _maxTouches)
                return false;

            var touch = _touches.FirstOrDefault(t => !t.IsActive);
            if (touch == null)
            {
                touch = new TouchData();
                _touches.Add(touch);
            }

            touch.Activate(id, position);
            _touchCount++;
            return true;
        }
        public void RemoveTouch(int id)
        {
            var touch = _touches.FirstOrDefault(t => t.IsActive && t.ID == id);
            _touchCount--;
            touch?.Deactivate();
        }
        public void UpdateTouch(int id, Vector2 newPosition)
        {
            var touch = _touches.FirstOrDefault(t => t.IsActive && t.ID == id);
            touch?.UpdatePosition(newPosition);
        }

        public int GetTouchCount() => _touchCount;
        public bool TryGetTouchById(int id, out TouchData touch)
        {
            touch = _touches.FirstOrDefault(t => t.IsActive && t.ID == id);
            return touch != null;
        }
        public IReadOnlyList<int> GetActiveTouchIds()
        {
            _activeTouches.Clear();
            foreach (var touch in _touches)
            {
                if (touch.IsActive)
                    _activeTouches.Add(touch.ID);
            }
            return _activeTouches;
        }
        public bool TryGetTouchByNo(int no, out TouchData touch)
        {
            if (no >= GetTouchCount())
            {
                touch = null;
                return false;
            }
            else
            {
                touch = _touches[no];
                return true;
            }
        }
    }
}