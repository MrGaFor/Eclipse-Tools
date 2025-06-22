using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Updater
{
    public interface IUpdaterObject
    {
        int Priority { get; }
        void CustomUpdate();
    }
    public interface IFixedUpdaterObject
    {
        int Priority { get; }
        void CustomFixedUpdate();
    }
    public interface ILateUpdaterObject
    {
        int Priority { get; }
        void CustomLateUpdate();
    }
    [HideMonoScript]
    public class UpdaterCore : MonoBehaviour
    {
        public static void Init()
        {
            Instance = new GameObject("[UpdaterCore]").AddComponent<UpdaterCore>();
            Object.DontDestroyOnLoad(Instance.gameObject);
        }

        public static UpdaterCore Instance;

        #region Updater
        private static List<IUpdaterObject> _updaters = new List<IUpdaterObject>();
        public static void AddUpdater(IUpdaterObject updater, int priority = 0)
        {
            int index = _updaters.FindLastIndex(u => u.Priority >= priority);
            _updaters.Insert(index + 1, updater);
        }
        public static void RemoveUpdater(IUpdaterObject updater) => _updaters.Remove(updater);

        private void Update()
        {
            if (_updaters.Count == 0) return;
            for (int i = _updaters.Count - 1; i >= 0; i--)
                _updaters[i].CustomUpdate();
        }
        #endregion

        #region FixedUpdater
        private static List<IFixedUpdaterObject> _fixedupdaters = new List<IFixedUpdaterObject>();
        public static void AddFixedUpdater(IFixedUpdaterObject updater, int priority = 0)
        {
            int index = _fixedupdaters.FindLastIndex(u => u.Priority >= priority);
            _fixedupdaters.Insert(index + 1, updater);
        }
        public static void RemoveFixedUpdater(IFixedUpdaterObject updater) => _fixedupdaters.Remove(updater);

        private void FixedUpdate()
        {
            if (_fixedupdaters.Count == 0) return;
            for (int i = _fixedupdaters.Count - 1; i >= 0; i--)
                _fixedupdaters[i].CustomFixedUpdate();
        }
        #endregion

        #region LateUpdater
        private static List<ILateUpdaterObject> _lateupdaters = new List<ILateUpdaterObject>();
        public static void AddLateUpdater(ILateUpdaterObject updater, int priority = 0)
        {
            int index = _lateupdaters.FindLastIndex(u => u.Priority >= priority);
            _lateupdaters.Insert(index + 1, updater);
        }
        public static void RemoveLateUpdater(ILateUpdaterObject updater) => _lateupdaters.Remove(updater);

        private void LateUpdate()
        {
            if (_lateupdaters.Count == 0) return;
            for (int i = _lateupdaters.Count - 1; i >= 0; i--)
                _lateupdaters[i].CustomLateUpdate();
        }
        #endregion
    }
}