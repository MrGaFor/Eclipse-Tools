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
    public class UniversalApplier : IApplier
    {
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker, ShowIf(nameof(_applier))]
        private IApplier _applier;

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string _applierTypeName;
        [SerializeField, HideInInspector] private string _targetTypeName;

        private Type _targetType;

        [ShowInInspector, HideLabel, HorizontalGroup("dropdown"), ValueDropdown(nameof(GetTargetTypesDropdown), SortDropdownItems = true, NumberOfItemsBeforeEnablingSearch = 0, FlattenTreeView = true), PropertyOrder(-2)]
        private Type _targetTypeDropdown
        {
            get => GetTargetType();
            set => SetTargetType(value);
        }

        [ShowInInspector, HideLabel, HorizontalGroup("dropdown"), ValueDropdown(nameof(GetApplierDropdown), SortDropdownItems = true, NumberOfItemsBeforeEnablingSearch = 0, FlattenTreeView = true), PropertyOrder(-1)]
        private Type _applierType
        {
            get => GetSelectedApplierType();
            set => SetSelectedApplierType(value);
        }

        private IEnumerable<ValueDropdownItem<Type>> GetTargetTypesDropdown()
        {
            var types = TypeCache.GetTypesDerivedFrom<IApplier>();
            var set = new HashSet<Type>();

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var attr = type.GetCustomAttributes(typeof(ApplierAttribute), false)
                    .FirstOrDefault() as ApplierAttribute;

                if (attr != null)
                    set.Add(attr.TargetType);
            }

            foreach (var t in set)
                yield return new ValueDropdownItem<Type>(ObjectNames.NicifyVariableName(t.Name), t);
        }

        private IEnumerable<ValueDropdownItem<Type>> GetApplierDropdown()
        {
            var target = GetTargetType();
            if (target == null)
                yield break;

            var types = TypeCache.GetTypesDerivedFrom<IApplier>();

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var attr = type.GetCustomAttributes(typeof(ApplierAttribute), false)
                    .FirstOrDefault() as ApplierAttribute;

                if (attr == null || attr.TargetType != target)
                    continue;

                yield return new ValueDropdownItem<Type>(attr.ValueName, type);
            }
        }

        private Type GetTargetType()
        {
            if (_targetType != null)
                return _targetType;

            if (!string.IsNullOrEmpty(_targetTypeName))
            {
                var t = Type.GetType(_targetTypeName);
                if (t != null)
                    _targetType = t;
            }

            return _targetType;
        }

        private void SetTargetType(Type type)
        {
            if (type == null)
                return;

            if (_targetType == type)
                return;

            _targetType = type;
            _targetTypeName = type.AssemblyQualifiedName;

            _applier = null;
            _applierTypeName = null;
        }

        private Type GetSelectedApplierType()
        {
            if (!string.IsNullOrEmpty(_applierTypeName))
            {
                var t = Type.GetType(_applierTypeName);
                if (t != null)
                    return t;
            }

            return _applier?.GetType();
        }

        private void SetSelectedApplierType(Type type)
        {
            if (type == null)
                return;

            _applierTypeName = type.AssemblyQualifiedName;

            if (_applier == null || _applier.GetType() != type)
                _applier = Activator.CreateInstance(type) as IApplier;
        }

        [OnInspectorInit]
        private void SyncAfterReload()
        {
            if (!string.IsNullOrEmpty(_targetTypeName))
            {
                var t = Type.GetType(_targetTypeName);
                if (t != null)
                    _targetType = t;
            }

            if (_applier != null)
                _applierTypeName = _applier.GetType().AssemblyQualifiedName;
            else if (!string.IsNullOrEmpty(_applierTypeName))
            {
                var t = Type.GetType(_applierTypeName);
                if (t != null)
                    _applier = Activator.CreateInstance(t) as IApplier;
            }
        }
#endif

        public bool HasGetter => _applier != null && _applier.HasGetter;
        public bool HasSetter => _applier != null && _applier.HasSetter;

        public object GetValue()
        {
            return _applier?.GetValue();
        }

        public void SetValueObject(object value)
        {
            if (_applier != null && HasSetter)
                _applier.SetValueObject(value);
        }

        public Type ValueType => _applier?.ValueType;
    }
}