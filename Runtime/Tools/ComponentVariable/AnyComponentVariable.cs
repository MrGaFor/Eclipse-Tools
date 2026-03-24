using Sirenix.OdinInspector;
using UnityEngine;


#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
#endif

namespace EC.ComponentVariable
{
    public class AnyComponentVariable : MonoBehaviour
    {
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker, HorizontalGroup("target")] private object variable;
        [SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker] private object target;

        public void Apply()
        {
            if (target is IVariableTarget t && t.HasSetter)
                t.SetObject(variable);
        }
        public void Fetch()
        {
            if (target is IVariableTarget t && t.HasGetter)
                variable = t.GetObject();
        }

        #region Editor
#if UNITY_EDITOR
        [ShowInInspector, ValueDropdown(nameof(GetVariableTypes)), OnValueChanged(nameof(OnVariableTypeChanged)), HideLabel, PropertyOrder(-1)] private Type selectedVariable;
        [ShowInInspector, ValueDropdown(nameof(GetTargetTypes)), OnValueChanged(nameof(OnTargetTypeChanged)), HideLabel, PropertyOrder(-1)] private Type selectedTarget;
        [ShowInInspector, Button("Set"), HorizontalGroup("target", 60), ShowIf(nameof(HasSetter)), PropertySpace(2.5f)] private void BtnSetter() => ApplySet();
        [ShowInInspector, Button("Get"), HorizontalGroup("target", 60, MarginRight = 3), ShowIf(nameof(HasGetter)), PropertySpace(2.5f)] private void BtnGetter() => ApplyGet();
        private bool HasSetter => target is IVariableTarget t && t.HasSetter;
        private bool HasGetter => target is IVariableTarget t && t.HasGetter;

        private void OnEnable() => SyncTypes();
        private void OnValidate() => SyncTypes();
        private void SyncTypes()
        {
            if (variable != null)
                selectedVariable = variable.GetType();
            if (target != null)
                selectedTarget = target.GetType();
        }
        private IEnumerable<ValueDropdownItem<Type>> GetVariableTypes()
        {
            var types = TypeCache.GetTypesDerivedFrom<IVariableTarget>()
                                 .Where(t => !t.IsAbstract && !t.IsInterface);
            var valueTypes = types
                .Select(t => Activator.CreateInstance(t) as IVariableTarget)
                .Where(t => t != null)
                .Select(t => t.ValueType)
                .Distinct();
            foreach (var t in valueTypes)
                yield return new ValueDropdownItem<Type>(
                    ObjectNames.NicifyVariableName(t.Name), t);
        }
        private IEnumerable<ValueDropdownItem<Type>> GetTargetTypes()
        {
            if (selectedVariable == null)
                yield break;
            var types = TypeCache.GetTypesDerivedFrom<IVariableTarget>()
                                 .Where(t => !t.IsAbstract && !t.IsInterface);
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as IVariableTarget;
                if (instance == null)
                    continue;
                if (instance.ValueType == selectedVariable)
                {
                    yield return new ValueDropdownItem<Type>(
                        UnityEditor.ObjectNames.NicifyVariableName(type.Name.Replace("Target", "")),
                        type);
                }
            }
        }
        private void OnVariableTypeChanged()
        {
            if (selectedVariable == null)
                return;
            variable = selectedVariable.IsValueType
                ? Activator.CreateInstance(selectedVariable)
                : null;
            selectedTarget = null;
            target = null;
        }
        private void OnTargetTypeChanged()
        {
            if (selectedTarget == null)
                return;
            target = Activator.CreateInstance(selectedTarget);
        }
        private void ApplySet()
        {
            Apply();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        private void ApplyGet()
        {
            Fetch();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion
    }
}