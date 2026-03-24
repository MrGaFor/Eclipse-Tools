using EC.Appliers.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Appliers.Core
{
    public abstract class TargetVariant<T> : ITarget<T> where T : Object
    {
        [SerializeField, HideLabel, HorizontalGroup("Target")] private T _target;

        public bool HasTarget => _target;
        public T Target => _target;
        public void SetTarget(T target) => _target = target;
    }
    public abstract class ApplierBase<TTarget, TValue>
    : TargetVariant<TTarget>, IApplier<TValue>
    where TTarget : Object
    {
        public virtual bool HasGetter => HasTarget;
        public virtual bool HasSetter => HasTarget;

        public abstract TValue Value { get; }
        public abstract void SetValue(TValue value);

        object IApplier.GetValue() => Value;
        void IApplier.SetValueObject(object value)
        {
            if (value is TValue v)
                SetValue(v);
        }
        public System.Type ValueType => typeof(TValue);
    }
}
