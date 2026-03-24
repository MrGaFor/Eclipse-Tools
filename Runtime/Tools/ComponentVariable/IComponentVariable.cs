namespace EC.ComponentVariable
{
    public interface IVariableTarget
    {
        System.Type ValueType { get; }
        bool HasSetter { get; }
        bool HasGetter { get; }

        void SetObject(object value);
        object GetObject();
    }

    public interface IVariableTarget<T> : IVariableTarget
    {
        void SetValue(T value);
        T GetValue();
    }

    public abstract class VariableTargetBase<T> : IVariableTarget<T>
    {
        public System.Type ValueType => typeof(T);

        public virtual bool HasSetter => true;
        public virtual bool HasGetter => true;

        public abstract void SetValue(T value);
        public abstract T GetValue();

        public void SetObject(object value)
        {
            if (value is T typed)
                SetValue(typed);
        }

        public object GetObject() => GetValue();
    }
}
