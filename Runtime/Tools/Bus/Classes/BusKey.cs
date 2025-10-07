using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript, System.Serializable]
    public class BusKey
    {
        public string Key => _key;
        private string[] AllKeys() => BusKeysData.GetKeys();
        private ValueDropdownItem<string>[] AllKeysWithType()
        {
            BusKeysData.KeyWithType[] keys = BusKeysData.GetKeysWithType();
            ValueDropdownItem<string>[] keysWithType = new ValueDropdownItem<string>[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                if (string.IsNullOrEmpty(keys[i].Type))
                    keysWithType[i] = new ValueDropdownItem<string>($"{keys[i].Key} [No Info]", keys[i].Key);
                else
                    keysWithType[i] = new ValueDropdownItem<string>($"{keys[i].Key} [{keys[i].Type}]", keys[i].Key);
            return keysWithType;
        }

        private enum InspectorState { Selecting, Editing, Removing }
        private InspectorState _inspectorState;
        private bool _isSelecting => _inspectorState == InspectorState.Selecting;
        private bool _isEditing => _inspectorState == InspectorState.Editing;
        private bool _isRemoving => _inspectorState == InspectorState.Removing;

        public void TransferKey(string key)
        {
            _key = key;
            BusKeysData.TryAddKey(key);
            _inspectorState = InspectorState.Selecting;
        }

        // SELECTING
        [SerializeField, ShowIf("_isSelecting"), HorizontalGroup(), LabelText("Key"), LabelWidth(70), ValueDropdown("AllKeysWithType")] private string _key;
        [SerializeField, ShowIf("_isSelecting"), HorizontalGroup(25f), Button("➕"), Tooltip("Add new Key"), PropertySpace(2)] 
        private void BtnCreate()
        {
            _inspectorState = InspectorState.Editing;
            _newKey = "";
        }
        [SerializeField, ShowIf("_isSelecting"), HorizontalGroup(25f), Button("➖"), Tooltip("Remove Key"), PropertySpace(2)]
        private void BtnRemove()
        {
            _inspectorState = InspectorState.Removing;
        }

        // ADDITING
        [SerializeField, ShowIf("_isEditing"), HorizontalGroup(), LabelText("New Key"), LabelWidth(70)] private string _newKey;
        [SerializeField, ShowIf("_isEditing"), HorizontalGroup(25f), Button("✔️"), Tooltip("Confirm"), PropertySpace(2)]
        private void BtnAdd()
        {
            _key = _newKey;
            BusKeysData.TryAddKey(_key);
            _inspectorState = InspectorState.Selecting;
        }
        [SerializeField, ShowIf("_isEditing"), HorizontalGroup(25f), Button("✖️"), Tooltip("Cancel"), PropertySpace(2)]
        private void BtnCancel()
        {
            _inspectorState = InspectorState.Selecting;
        }

        // REMOVING
        [SerializeField, ShowIf("_isRemoving"), HorizontalGroup(), Button("✔️ Confirm"), Tooltip("Confirm removing Key"), PropertySpace(2)]
        private void BtnYesRemoving()
        {
            BusKeysData.TryRemoveKey(_key);
            _key = "";
            _inspectorState = InspectorState.Selecting;
        }
        [SerializeField, ShowIf("_isRemoving"), HorizontalGroup(), Button("✖️ Cancel"), Tooltip("Cancel removing Key"), PropertySpace(2)]
        private void BtnNoRemoving()
        {
            _inspectorState = InspectorState.Selecting;
        }

        public void Unselect()
        {
            if (_isEditing) BtnCancel();
            if (_isRemoving) BtnNoRemoving();
        }

    }
}
