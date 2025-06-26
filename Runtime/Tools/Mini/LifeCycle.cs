using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Mini
{
    [HideMonoScript]
    public class LifeCycle : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("Awake-Start/gr"), FoldoutGroup("Awake-Start")] private UnityEvent _awake;
        [SerializeField, HorizontalGroup("Awake-Start/gr"), FoldoutGroup("Awake-Start")] private UnityEvent _start;

        [SerializeField, HorizontalGroup("OnEnable-OnDisable/gr"), FoldoutGroup("OnEnable-OnDisable")] private UnityEvent _onEnable;
        [SerializeField, HorizontalGroup("OnEnable-OnDisable/gr"), FoldoutGroup("OnEnable-OnDisable")] private UnityEvent _onDisable;

        [SerializeField, FoldoutGroup("OnDestroy")] private UnityEvent _onDestroy;

        private void Awake() => _awake?.Invoke();
        private void OnEnable() => _onEnable?.Invoke();
        private void Start() => _start?.Invoke();

        private void OnDisable() => _onDisable?.Invoke();
        private void OnDestroy() => _onDestroy?.Invoke();
    }
}
