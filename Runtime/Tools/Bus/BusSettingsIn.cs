using EC.Effects;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace EC.Bus
{
    [global::System.SerializableAttribute]
    public class BusSettingsGenericCustom<T> : IBusInSettings
    {
        public BusSettingsGenericCustom(string key, EventTypes type)
        {
            Key = key;
            Type = type;
        }

        [SerializeField, HorizontalGroup("key"), LabelWidth(70)] private string Key;
        [SerializeField, HorizontalGroup("key", 65), HideLabel] private EventTypes Type;
        private T Value;

        public string GetKey() => Key;
        public object GetValue() => Value;
        public EventTypes GetEventType() => Type;

        public void Invoke(T value)
        {
            Value = value;
            BusSystem.CallEvent<T>(this);
        }
    }
    [global::System.SerializableAttribute]
    public class BusSettingsGenericFix<T> : IBusInSettings
    {
        public BusSettingsGenericFix() { }
        public BusSettingsGenericFix(string key, EventTypes type, T value)
        {
            Key = key;
            Type = type;
            Value = value;
        }

        [SerializeField, HorizontalGroup("key"), LabelWidth(70)] private string Key;
        [SerializeField, HorizontalGroup("key", 65), HideLabel] private EventTypes Type;
        [SerializeField, HorizontalGroup("value"), LabelWidth(70)] private T Value;

        public string GetKey() => Key;
        public object GetValue() => Value;
        public EventTypes GetEventType() => Type;

        public void Invoke()
        {
            BusSystem.CallEvent<T>(this);
        }
    }
    [global::System.SerializableAttribute]
    public class BusSettingsInDefault : IBusInSettings
    {
        [SerializeField, HorizontalGroup("type"), LabelWidth(70), OnValueChanged("OnChangeType")] private BusSettingsType _type;
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker] private IBusInSettings _data;

        public string GetKey() => _data.GetKey();
        public object GetValue() => _data.GetValue();
        public EventTypes GetEventType() => _data.GetEventType();

        public void Invoke()
        {
            if (!BusConvert.BusToCallEvents.TryGetValue(_type, out var action))
            {
                Debug.LogErrorFormat("BusSettingsIn: Invoke -> BusSettingsTypes dont include type!", this);
                return;
            }
            action.Invoke(_data);
        }

        #region Editor
        private void OnChangeType()
        {
#if UNITY_EDITOR
            _data = CreateEffectInstance(_type);
            var root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault();
            if (root != null) UnityEditor.EditorUtility.SetDirty(root);

            IBusInSettings CreateEffectInstance(BusSettingsType type)
            {
                if (!BusConvert.BusSettingsTypes.TryGetValue(type, out var targetType))
                    return null;
                var genericType = typeof(BusSettingsGenericFix<>).MakeGenericType(targetType);
                return (IBusInSettings)Activator.CreateInstance(genericType);
            }
#endif
        }
        #endregion
    }
}
