using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace EC.Popup
{
    public static class PopupManager
    {
        private static readonly List<PopupContainer> _containers = new();

        #region Editor
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _containers.Clear();
        }
#endif
        #endregion

        internal static void Register(PopupContainer container)
        {
            if (!_containers.Contains(container))
                _containers.Add(container);
        }
        internal static void Unregister(PopupContainer container)
        {
            _containers.Remove(container);
        }

        public static bool TryGet<T>(out T popup) where T : PopupBase
        {
            for (int i = _containers.Count - 1; i >= 0; i--)
                if (_containers[i].TryGet(typeof(T), out var p))
                {
                    popup = (T)p;
                    return true;
                }
            popup = null;
            return false;
        }

        public static bool TryGet(string id, out PopupBase popup)
        {
            for (int i = _containers.Count - 1; i >= 0; i--)
                if (_containers[i].TryGet(id, out popup))
                    return true;
            popup = null;
            return false;
        }

        public static T Get<T>() where T : PopupBase
        {
            TryGet(out T popup);
            return popup;
        }

        public static PopupBase Get(string id)
        {
            TryGet(id, out var popup);
            return popup;
        }
    }
}
