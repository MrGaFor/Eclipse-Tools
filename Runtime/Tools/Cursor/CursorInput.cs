using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Cursors
{
    [HideMonoScript]
    public class CursorInput : MonoBehaviour
    {
        private enum InputType { No, Id }
        [SerializeField, BoxGroup("UI", false), LabelWidth(100)] private InputType _inputType = InputType.Id;
#if UNITY_EDITOR
        [ValueDropdown("GetIds")]
#endif
        [SerializeField, BoxGroup("UI"), LabelWidth(100), ShowIf("_inputType", InputType.Id)] private string _id = "Default";
#if UNITY_EDITOR
        [PropertyRange(0, "GetIdsCound")]
#endif
        [SerializeField, BoxGroup("UI"), LabelWidth(100), ShowIf("_inputType", InputType.No)] private int _no = 0;

        public void SetCurrent()
        {
            if (_inputType == InputType.No) CursorManager.SelectCursor(_no);
            else if (_inputType == InputType.Id) CursorManager.SelectCursor(_id);
        }
        public void SetId(string id) => CursorManager.SelectCursor(_id);
        public void SetNo(int no) => CursorManager.SelectCursor(no);

#if UNITY_EDITOR
        private string[] GetIds() => CursorManager.GetIds();
        private int GetIdsCound() => GetIds().Length - 1;
#endif
    }
}
