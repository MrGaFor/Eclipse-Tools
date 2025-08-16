using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Behaviour
{
    [HideMonoScript]
    public abstract class BehaviourGroupClass<T> : MonoBehaviour
    {
        public virtual BehaviourClass<T>[] BehaviourClasses { get; protected set; }
        public virtual T Core { get; protected set; }

        public virtual void Initialize()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Initialize(Core);
        }
        public virtual void Awake()
        {
            Initialize();
            foreach (var behaviour in BehaviourClasses)
                behaviour.Awake();
        }
        public virtual void Start()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Start();
        }
        public virtual void Update()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Update();
        }
        public virtual void FixedUpdate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.LateUpdate();
        }
        public virtual void OnEnable()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnEnable();
        }
        public virtual void OnDisable()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDisable();
        }
        public virtual void OnDestroy()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDestroy();
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnValidate();
        }
        public virtual void OnDrawGizmos()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDrawGizmos();
        }
        public virtual void OnDrawGizmosSelected()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDrawGizmosSelected();
        }
#endif
    }

    [HideMonoScript]
    public abstract class BehaviourGroupClass : MonoBehaviour
    {
        public virtual BehaviourClass[] BehaviourClasses { get; protected set; }

        public virtual void Awake()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Awake();
        }
        public virtual void Start()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Start();
        }
        public virtual void Update()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.Update();
        }
        public virtual void FixedUpdate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.LateUpdate();
        }
        public virtual void OnEnable()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnEnable();
        }
        public virtual void OnDisable()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDisable();
        }
        public virtual void OnDestroy()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDestroy();
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnValidate();
        }
        public virtual void OnDrawGizmos()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDrawGizmos();
        }
        public virtual void OnDrawGizmosSelected()
        {
            foreach (var behaviour in BehaviourClasses)
                behaviour.OnDrawGizmosSelected();
        }
#endif
    }
}
