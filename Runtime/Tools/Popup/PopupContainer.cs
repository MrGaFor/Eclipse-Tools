using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Popup
{
    [HideMonoScript]
    public sealed class PopupContainer : MonoBehaviour
    {
        public enum Scope { Scene, Game }
        [Serializable]
        private struct PopupEntry
        {
            public enum ResolveMode
            {
                ByType,
                ById
            }

            [HorizontalGroup(Width = 90)]
            [HideLabel]
            public ResolveMode Mode;

            [HorizontalGroup, ShowIf(nameof(Mode), ResolveMode.ById)]
            [HideLabel]
            public string Id;

            [HorizontalGroup]
            [HideLabel]
            public PopupBase Popup;
        }

        [SerializeField] private Scope _scope;
        #region Editor
#if UNITY_EDITOR
        [InfoBox("Game scope requires non-empty Container Id", InfoMessageType.Error, VisibleIf = nameof(GameScopeWithoutId))]
#endif
        #endregion
        [SerializeField, ShowIf(nameof(_scope), Scope.Game)] private string _containerId;

        #region Editor
#if UNITY_EDITOR
        [InfoBox("Popup list is empty", InfoMessageType.Error, VisibleIf = nameof(PopupListEmpty))]
        [InfoBox("Null popup reference detected", InfoMessageType.Error, VisibleIf = nameof(HasNullPopup))]
        [InfoBox("ById mode requires non-empty Id", InfoMessageType.Error, VisibleIf = nameof(HasEmptyIdInByIdMode))]
        [InfoBox("Duplicate popup Id detected", InfoMessageType.Error, VisibleIf = nameof(HasDuplicateIds))]
        [InfoBox("Duplicate popup Type without Id detected", InfoMessageType.Error, VisibleIf = nameof(HasDuplicateTypesWithoutId))]
#endif
        #endregion
        [SerializeField] private PopupEntry[] _popups;

        private readonly Dictionary<string, PopupBase> _byId = new();
        private readonly Dictionary<Type, PopupBase> _byType = new();

        private static HashSet<string> _gameContainers = new();

        #region Editor
#if UNITY_EDITOR
        private bool GameScopeWithoutId => _scope == Scope.Game && string.IsNullOrWhiteSpace(_containerId);
        private bool PopupListEmpty => _popups == null || _popups.Length == 0;
        private bool HasNullPopup
        {
            get
            {
                if (_popups == null) return false;

                foreach (var e in _popups)
                    if (e.Popup == null)
                        return true;

                return false;
            }
        }
        private bool HasEmptyIdInByIdMode
        {
            get
            {
                if (_popups == null) return false;

                foreach (var e in _popups)
                {
                    if (e.Mode == PopupEntry.ResolveMode.ById &&
                        string.IsNullOrWhiteSpace(e.Id))
                        return true;
                }
                return false;
            }
        }
        private bool HasDuplicateIds
        {
            get
            {
                if (_popups == null) return false;

                HashSet<string> ids = new();

                for (int i = 0; i < _popups.Length; i++)
                {
                    var e = _popups[i];
                    if (e.Popup == null || string.IsNullOrEmpty(e.Id))
                        continue;

                    if (!ids.Add(e.Id))
                        return true;
                }
                return false;
            }
        }
        private bool HasDuplicateTypesWithoutId
        {
            get
            {
                if (_popups == null) return false;

                HashSet<Type> types = new();

                for (int i = 0; i < _popups.Length; i++)
                {
                    var e = _popups[i];
                    if (e.Popup == null || !string.IsNullOrEmpty(e.Id))
                        continue;

                    var type = e.Popup.GetType();
                    if (!types.Add(type))
                        return true;
                }
                return false;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _gameContainers.Clear();
        }
#endif
        #endregion

        private void Awake()
        {
            if (_scope == Scope.Game)
            {
                if (string.IsNullOrWhiteSpace(_containerId) || !_gameContainers.Add(_containerId))
                {
                    Destroy(gameObject);
                    return;
                }
                DontDestroyOnLoad(gameObject);
            }

            for (int i = 0; i < _popups.Length; i++)
            {
                var p = _popups[i];
                if (p.Popup == null)
                    continue;

                if (!string.IsNullOrEmpty(p.Id))
                    _byId.TryAdd(p.Id, p.Popup);

                _byType.TryAdd(p.Popup.GetType(), p.Popup);
            }

            PopupManager.Register(this);
        }

        private void OnDestroy()
        {
            PopupManager.Unregister(this);

            if (_scope == Scope.Game && !string.IsNullOrEmpty(_containerId))
                _gameContainers.Remove(_containerId);
        }

        public bool TryGet<T>(Type type, out T popup) where T : PopupBase
        {
            if (_byType.TryGetValue(type, out var pop))
            {
                popup = (T)pop;
                return true;
            }
            popup = default(T);
            return false;
        }

        public bool TryGet<T>(string id, out T popup) where T : PopupBase
        {
            if (_byId.TryGetValue(id, out var pop))
            {
                popup = (T)pop;
                return true;
            }
            popup = default(T);
            return false;
        }
    }
}
