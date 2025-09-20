using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Mini
{
    [HideMonoScript]
    public class InspectorButton : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField, BoxGroup()] private ButtonSlot[] _buttons;

        [System.Serializable]
        private class ButtonSlot
        {
            [SerializeField, Button("Invoke"), PropertyOrder(-1)] private void Invoke() => _action?.Invoke();
            [SerializeField, FoldoutGroup("Event")] private UnityEngine.Events.UnityEvent _action;
        }
#endif
    }
}
