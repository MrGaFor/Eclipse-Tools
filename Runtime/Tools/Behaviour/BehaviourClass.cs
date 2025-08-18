namespace EC.Behaviour
{
    public class BehaviourClass<T> : BehaviourClass
    {
        [UnityEngine.HideInInspector] public T Core;
        public virtual void Initialize(T core) { Core = core; }
    }
    public class BehaviourClass
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
