using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GIB.Triggers
{
    /// <summary>
    /// A script which fires events when a target Renderer becomes visible.
    /// </summary>
    public class VisibleTrigger : MonoBehaviour
    {
        [InfoBox("Expensive Component! \n Be careful not to use this type of trigger very often," +
            " and to leave it armed for the shortest duration possible.", InfoMessageType.Warning)]
        /// <summary>
        /// Whether this trigger has been triggered. Can be set to False to disable this trigger.
        /// <para>Can be set with <see cref="SetTriggerState(bool)"/>, <see cref="ArmTrigger"/> and <see cref="DisarmTrigger"/></para>
        /// </summary>
        [SerializeField] private bool isArmed = true;

        [SerializeField] private Camera targetCamera;

        private Renderer _renderer;

        [SerializeField] private RendererType rendererType;

        [Space, SerializeField]
        private bool hasDelay;

        [SerializeField, ShowIf("hasDelay"), Range(0, 60f)]
        [BoxGroup("Delay")]
        private float delaySeconds;

        [Tooltip("The UnityEvents which are triggered by this component.")]
        [SerializeField, BoxGroup("Triggered Events")]
        private EventPro OnFireEvent;

        [SerializeField] private bool destroyAfterFire;

        private void Start()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            switch (rendererType)
            {
                case RendererType.MeshRenderer:
                    _renderer = GetComponent<MeshRenderer>();
                    break;
                case RendererType.SkinnedMeshRenderer:
                    _renderer = GetComponent<SkinnedMeshRenderer>();
                    break;
                case RendererType.LineRenderer:
                    _renderer = GetComponent<LineRenderer>();
                    break;
                default:
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (isArmed
                && targetCamera.IsObjectVisible(_renderer))
            {
                TriggerEvents();
            }
        }

        #region Public Methods
        /// <summary>
        /// Checks if the object can fire its events, and does so if able.
        /// </summary>
        [Button]
        public void TriggerEvents()
        {
            if (isArmed)
            {
                isArmed = false;
                // Check if the script has valid UnityEvents
                if (OnFireEvent == null)
                {
                    GIBUtils.Warn($"{this.gameObject.name}> Attempted to fire events; was null. Destroying this trigger");
                    Destroy(this);
                }

                // if the event is on a delay, start the delay coroutine.
                // otherwise, just fire the event
                if (hasDelay)
                {
                    StartCoroutine(FireDelayedUnityEvent(delaySeconds));
                }
                else
                {
                    FireEvents();
                }
            }
        }

        [Button]
        public void ArmTrigger()
        {
            SetTriggerState(true);
        }

        [Button]
        public void DisarmTrigger()
        {
            SetTriggerState(false);
        }

        /// <summary>
        /// Sets the armed state of the trigger, allowing trigger to be triggered again
        /// or disabling it.
        /// </summary>
        public void SetTriggerState(bool targetState)
        {
            isArmed = targetState;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Coroutine to fire target events with a delay of target seconds.
        /// </summary>
        private IEnumerator FireDelayedUnityEvent(float delay)
        {
            yield return new WaitForSeconds(delay);
            FireEvents();
        }

        /// <summary>
        /// Invokes the target Unity Events.
        /// </summary>
        protected virtual void FireEvents()
        {
            // Fire the event
            OnFireEvent.Invoke();

            // Perform the ending behavior
            if (destroyAfterFire) Destroy(this);
        }

        #endregion

    }

    #region enums

    public enum RendererType
    {
        MeshRenderer,
        SkinnedMeshRenderer,
        LineRenderer
    }

    #endregion


}