using Sirenix.OdinInspector;
using System;
using UnityEngine;
using EC.Appliers.Interfaces;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif

namespace EC.Appliers
{
    [Serializable, HideLabel]
    public class TargetFilter<TTarget> where TTarget : UnityEngine.Object
    {
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker, ShowIf(nameof(_applier))]
        private IApplier _applier;

        #region Editor
#if UNITY_EDITOR
        [SerializeField, HideInInspector] 
        private string _typeName;
        [ShowInInspector, HideLabel, ValueDropdown(nameof(GetApplierDropdown), SortDropdownItems = true, NumberOfItemsBeforeEnablingSearch = 0, FlattenTreeView = true), PropertyOrder(-1)]
        private Type _type
        {
            get => GetSelectedType();
            set => SetSelectedType(value);
        }

        private IEnumerable<ValueDropdownItem<Type>> GetApplierDropdown()
        {
            var types = TypeCache.GetTypesDerivedFrom<IApplier>();
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                var attr = type.GetCustomAttributes(typeof(ApplierAttribute), false)
                               .FirstOrDefault() as ApplierAttribute;
                if (attr == null || attr.TargetType != typeof(TTarget))
                    continue;
                string name = $"{attr.TargetName}/{attr.ValueName}";
                yield return new ValueDropdownItem<Type>(name, type);
            }
        }

        private Type GetSelectedType()
        {
            if (!string.IsNullOrEmpty(_typeName))
            {
                var t = Type.GetType(_typeName);
                if (t != null)
                    return t;
            }
            return _applier?.GetType();
        }

        private void SetSelectedType(Type type)
        {
            if (type == null)
                return;
            _typeName = type.AssemblyQualifiedName;
            if (_applier == null || _applier.GetType() != type)
                _applier = Activator.CreateInstance(type) as IApplier;
        }

        [OnInspectorInit]
        private void SyncAfterReload()
        {
            if (_applier != null)
                _typeName = _applier.GetType().AssemblyQualifiedName;
            else if (!string.IsNullOrEmpty(_typeName))
            {
                var t = Type.GetType(_typeName);
                if (t != null)
                    _applier = Activator.CreateInstance(t) as IApplier;
            }
        }
#endif
        #endregion

        public bool HasGetter => _applier != null && _applier.HasGetter;
        public bool HasSetter => _applier != null && _applier.HasSetter;

        public object GetValue() => _applier?.GetValue();
        public void SetValue(object value)
        {
            if (_applier != null && HasSetter)
                _applier.SetValueObject(value);
        }
        public Type ValueType => _applier?.ValueType;
    }
}