namespace EC.Appliers.Interfaces
{
    public interface ITarget
    {
        public bool HasTarget { get; }
    }
    public interface ITarget<T> : ITarget
    {
        public T Target { get; }
        public void SetTarget(T target);
    }
    public interface IApplier
    {
        public bool HasGetter { get; }
        public bool HasSetter { get; }

        object GetValue();
        void SetValueObject(object value);
        System.Type ValueType { get; }
    }
    public interface IApplier<T> : IApplier
    {
        public T Value { get; }
        public void SetValue(T value);
    }
}
