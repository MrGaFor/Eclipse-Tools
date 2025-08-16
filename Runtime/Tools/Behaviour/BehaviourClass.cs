namespace EC.Behaviour
{
    [System.Serializable]
    public abstract class BehaviourClass<T> : BehaviourClass
    {
        public T Core { get; protected set; }
        public virtual void Initialize(T core) { Core = core; }
    }
    [System.Serializable]
    public abstract class  BehaviourClass
    {
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }

#if UNITY_EDITOR
        public virtual void OnValidate() { }
        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
#endif
    }
}
