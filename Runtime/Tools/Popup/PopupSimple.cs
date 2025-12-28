using System;
using UnityEngine;

namespace EC.Popup
{
    public class PopupSimple : PopupBase
    {
        [SerializeField] private Effects.EffectorStates _states;

        private Action _onClick;
        private Action _onClose;

        public void Init(Action onClick = null, Action onClose = null)
        {
            _onClick = onClick;
            _onClose = onClose;
        }
        public override void Show()
        {
            _states.PlaySmooth("Show");
        }
        public override void Hide()
        {
            _states.PlaySmooth("Hide");
        }
        public void Click()
        {
            _onClick?.Invoke();
            Hide();
        }
        public void Close()
        {
            _onClose?.Invoke();
            Hide();
        }
    }
}
