using Sirenix.OdinInspector;
using System.Linq;
using System;
using UnityEngine;

namespace EC.Bus
{
    [global::System.SerializableAttribute]
    public class BusSettingsOutGeneric<T> : IBusOutSettings
    {
        [SerializeField, LabelWidth(70)] private string _key;
        [SerializeField, HideLabel] private Compair.CompairElementGeneric<T> _compair = new();

        public void Subscribe() => BusSystem.Subscribe<T>(_key, OnChange);
        public void Unsubscribe() => BusSystem.Unsubscribe<T>(_key, OnChange);

        private void OnChange(T value)
        {
            _compair?.Invoke(value);
        }

    }
    [global::System.SerializableAttribute]
    public class BusSettingsOutDefault : IBusOutSettings
    {
        [SerializeField, HorizontalGroup("type"), LabelWidth(70), OnValueChanged("OnChangeType")] private BusSettingsType _type;
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker] private IBusOutSettings _data;

        public void Subscribe() => _data.Subscribe();
        public void Unsubscribe() => _data.Unsubscribe();

        #region Editor
        private void OnChangeType()
        {
#if UNITY_EDITOR
            _data = CreateEffectInstance(_type);
            var root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault();
            if (root != null) UnityEditor.EditorUtility.SetDirty(root);

            IBusOutSettings CreateEffectInstance(BusSettingsType type)
            {
                if (!BusConvert.BusSettingsTypes.TryGetValue(type, out var targetType))
                    return null;
                var genericType = typeof(BusSettingsOutGeneric<>).MakeGenericType(targetType);
                return (IBusOutSettings)Activator.CreateInstance(genericType);
            }
#endif
        }
        #endregion
    }
}