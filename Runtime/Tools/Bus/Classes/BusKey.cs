using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript, System.Serializable]
    public class BusKey
    {
        public string Key => _key;

        private enum InspectorState { Selecting, Editing, Removing }
        private InspectorState _inspectorState;

        private bool _isSelecting => _inspectorState == InspectorState.Selecting;
        private bool _isEditing => _inspectorState == InspectorState.Editing;
        private bool _isRemoving => _inspectorState == InspectorState.Removing;

#if UNITY_EDITOR
        private string[] AllKeys() => BusSettingsProvider.Settings.GetKeys();

        private ValueDropdownItem<string>[] AllKeysWithType()
        {
            var keys = BusSettingsProvider.Settings.GetKeysWithType();
            var items = new ValueDropdownItem<string>[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                var typeInfo = string.IsNullOrEmpty(keys[i].Type) ? "No Info" : keys[i].Type;
                items[i] = new ValueDropdownItem<string>($"{keys[i].Key} [{typeInfo}]", keys[i].Key);
            }
            return items;
        }

        public void TransferKey(string key)
        {
            _key = key;
            BusSettingsProvider.Settings.TryAddKey(key);
            _inspectorState = InspectorState.Selecting;
        }
#endif

        // SELECTING
        [SerializeField, ShowIf("_isSelecting"), HorizontalGroup(), LabelText("Key"), LabelWidth(70),
#if UNITY_EDITOR
         ValueDropdown("AllKeysWithType")
#endif
        ]
        private string _key;

#if UNITY_EDITOR
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

        // ADDING
        [SerializeField, ShowIf("_isEditing"), HorizontalGroup(), LabelText("New Key"), LabelWidth(70)]
        private string _newKey;

        [SerializeField, ShowIf("_isEditing"), HorizontalGroup(25f), Button("✔️"), Tooltip("Confirm"), PropertySpace(2)]
        private void BtnAdd()
        {
            _key = _newKey;
            BusSettingsProvider.Settings.TryAddKey(_key);
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
            BusSettingsProvider.Settings.TryRemoveKey(_key);
            _key = "";
            _inspectorState = InspectorState.Selecting;
        }

        [SerializeField, ShowIf("_isRemoving"), HorizontalGroup(), Button("✖️ Cancel"), Tooltip("Cancel removing Key"), PropertySpace(2)]
        private void BtnNoRemoving()
        {
            _inspectorState = InspectorState.Selecting;
        }
#endif

        public void Unselect()
        {
#if UNITY_EDITOR
            if (_isEditing) BtnCancel();
            if (_isRemoving) BtnNoRemoving();
#endif
        }
    }
}
