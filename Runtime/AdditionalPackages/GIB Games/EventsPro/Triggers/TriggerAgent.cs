using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GIB.EventTriggers;
using Sirenix.OdinInspector;

namespace GIB.Triggers
{
    /// <summary>
    ///     A UnityEvent-based trigger-action system.
    /// </summary>
    public class TriggerAgent : MonoBehaviour, ITrigger
    {
        [SerializeField]
        private EventTriggers triggerSource;

        /// <summary>
        ///     Whether this trigger has been triggered. Can be set to False to disable this trigger.
        ///     <para>
        ///         Can be set with <see cref="SetTriggerState(bool)" />, <see cref="ArmTrigger" /> and
        ///         <see cref="DisarmTrigger" />
        ///     </para>
        /// </summary>
        public bool isArmed = true;

        [Header("Options")]
        [SerializeField]
        [ShowIf("triggerSource", EventTriggers.Event)]
        private string eventName = "DummyEvent";

        [Tooltip("Whether the colliding object must have specific name or tags for " +
                 "that trigger this effect.")]
        [SerializeField]
        private DetectTarget selectiveDetection;

        /// <summary>
        /// List of strings that trigger this effect.
        /// </summary>
        [Tooltip("List of colliding objects that trigger this effect.")]
        [SerializeField]
        [ShowIf("HasDetectionList")]
        [BoxGroup("Detection Rules")]
        public List<string> DetectionList = new List<string>();

        [Tooltip("List of specific colliders that trigger this effect.")]
        [SerializeField]
        [ShowIf("selectiveDetection", DetectTarget.Collider)]
        [BoxGroup("Detection Rules")]
        private List<Collider> colliderList = new List<Collider>();

        [Space][SerializeField] private bool hasDelay;

        [SerializeField]
        [ShowIf("hasDelay")]
        [Range(0, 60f)]
        [BoxGroup("Delay")]
        private float delay;

        private UnityAction onEvent;

        /// <summary>
        /// The UnityEvents invoked when this trigger fires.
        /// </summary>
        [Tooltip("The UnityEvents which are triggered by this component.")]
        [BoxGroup("Triggered Events")]
        public EventPro OnFireEvent;

        [Tooltip("Ending behavior. Default = Disarm")]
        [Space][SerializeField] private PostScriptEvents endingBehavior = PostScriptEvents.Disarm;

        #region Init

        private void OnEnable()
        {
            onEvent = Trigger;

            if (triggerSource == EventTriggers.Event)
                EventController.Subscribe(eventName, onEvent, gameObject.name);
        }

        private void OnDisable()
        {
            if (triggerSource == EventTriggers.Event)
                EventController.Unsubscribe(eventName, onEvent, gameObject.name);
        }

        #endregion

        #region Triggers

        private void Start()
        {
            if (CheckArmedSource(EventTriggers.Start))
                Trigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckArmedSource(EventTriggers.TriggerEnter))
                CheckTriggerConditions(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (CheckArmedSource(EventTriggers.TriggerExit))
                CheckTriggerConditions(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CheckArmedSource(EventTriggers.Collision))
            {
                if (selectiveDetection == DetectTarget.Collider && colliderList.Contains(collision.collider))
                    Trigger();
                else
                    CheckTriggerConditions(collision.collider.gameObject);
            }
        }

        private bool CheckArmedSource(EventTriggers source) =>
            isArmed && triggerSource == source;

        #endregion

        #region Trigger Conditions

        private void CheckTriggerConditions(GameObject targetObject)
        {
            //If selective detection is set to none we don't need to check anything else
            if (selectiveDetection == DetectTarget.None)
            {
                Trigger();
                return;
            }

            TriggerTarget triggerTarget = targetObject.GetComponent<TriggerTarget>();
            switch (selectiveDetection)
            {
                case DetectTarget.Player:
                    {
                        if (targetObject.CompareTag("Player"))
                            Trigger();
                        break;
                    }
                case DetectTarget.Collider:
                    {
                        if(TryGetComponent(out Collider collider))
                        {
                            if (colliderList.Contains(collider))
                                Trigger();
                        }
                        break;
                    }
                case DetectTarget.Tags:
                    string targetTag = targetObject.tag;
                    if (DetectionList.Contains(targetTag)) Trigger();
                    break;
                case DetectTarget.Name:
                    string targetName = targetObject.name;
                    if (DetectionList.Contains(targetName)) Trigger();
                    break;
                case DetectTarget.TriggerTarget:
                    if (triggerTarget != null) Trigger();
                    break;
                case DetectTarget.TargetID:
                    if (triggerTarget != null && DetectionList.Contains(triggerTarget.TargetId)) Trigger();
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Checks if the object can fire its events, and does so if able.
        /// </summary>
        [Button(buttonSize: (int)ButtonSizes.Large)]
        public void Trigger()
        {
            if (!isArmed)
            {
                GIBUtils.Log($"{gameObject.name}> Target trigger was unarmed.");
                return;
            }

            // Check if the script has valid UnityEvents
            if (OnFireEvent == null)
            {
                GIBUtils.Warn(
                    $"{gameObject.name}> Attempted to fire events; was null. Destroying this trigger");
                Destroy(this);
            }

            // if the event is on a delay, start the delay coroutine.
            // otherwise, just fire the event
            if (hasDelay)
                StartCoroutine(FireDelayedUnityEvent(delay));
            else
                FireEvents();
        }

        /// <summary>
        ///     Fires target Unity Events without checking for trigger state or delay.
        ///     <para><see cref="Trigger" /> is preferred.</para>
        /// </summary>
        [Button]
        public void TriggerImmediately() => FireEvents();

        /// <summary>
        /// Arms the trigger so it has the potential to fire when 
        /// <see cref="Trigger"/> is called.
        /// </summary>
        [Button()]
        public void ArmTrigger() => SetTriggerState(true);

        /// <summary>
        /// Disarms the trigger so it should not fire when 
        /// <see cref="Trigger"/> is called.
        /// </summary>
        [Button()]
        public void DisarmTrigger() => SetTriggerState(false);

        /// <summary>
        ///     Sets the armed state of the trigger, allowing trigger to be triggered again
        ///     or disabling it.
        /// </summary>
        public void SetTriggerState(bool targetState) => isArmed = targetState;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Invokes the target Unity Events.
        /// </summary>
        protected virtual void FireEvents()
        {
            // Fire the event
            OnFireEvent.Invoke();

            // Perform the ending behavior
            FinishEvents();
        }

        /// <summary>
        ///     Sets the trigger state to either reset, not reset, or destroy itself.
        /// </summary>
        private void FinishEvents(PostScriptEvents finish)
        {
            // Set the script into the correct state for after the trigger.
            switch (finish)
            {
                case PostScriptEvents.Arm:
                    //isArmed remains true so it can be triggered again.
                    ArmTrigger();
                    break;
                case PostScriptEvents.Disarm:
                    DisarmTrigger();
                    break;
                case PostScriptEvents.Destroy:
                    Destroy(this);
                    break;
                case PostScriptEvents.DestroyObject:
                    //Debug.Log("You Monster.");
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }

        private void FinishEvents() => FinishEvents(endingBehavior);

        #endregion

        #region Coroutines

        /// <summary>
        ///     Coroutine to fire target events with a delay of target seconds.
        /// </summary>
        /// <param name="delay">The delay, in seconds.</param>
        private IEnumerator FireDelayedUnityEvent(float delay)
        {
            yield return new WaitForSeconds(delay);
            FireEvents();
        }

        #endregion

        #region Enums

        public enum EventTriggers
        {
            /// <summary>
            ///     Triggered with a TriggerEvents() method call.
            /// </summary>
            Manual = 0,

            /// <summary>
            ///     Triggered when OnTriggerEnter() is called.
            /// </summary>
            TriggerEnter = 1,

            /// <summary>
            ///     Triggered when OnTriggerExit() is called.
            /// </summary>
            TriggerExit = 2,

            /// <summary>
            ///     Triggered when Start() is called.
            /// </summary>
            Start = 3,

            /// <summary>
            ///     OBSOLETE: Use <see cref="VisibleTrigger" />.
            /// </summary>
            IsVisible = 4,

            /// <summary>
            ///     Triggered when colliding with a target object
            /// </summary>
            Collision = 5,

            /// <summary>
            ///     Triggered when the EventController calls a target event.
            /// </summary>
            Event = 6
        }

        public enum PostScriptEvents
        {
            /// <summary>
            ///     If this event should only ever happen once and only once.
            /// </summary>
            Disarm = 1,
            /// <summary>
            ///     If this event can be triggered over and over.
            /// </summary>
            Arm = 0,
            /// <summary>
            ///     If this event can be triggered again but must first be re-armed.
            /// </summary>

            Destroy = 2,

            /// <summary>
            ///     Destroy the whole object, you monster.
            /// </summary>
            DestroyObject = 3
        }

        public enum DetectTarget
        {
            /// <summary>
            /// No selective detection.
            /// </summary>
            None = 0,
            /// <summary>
            /// The colliding object has a <see cref="GibPlayerController"/>.
            /// </summary>
            Player = 6,
            /// <summary>
            /// The colliding object has target tags.
            /// </summary>
            Tags = 1,
            /// <summary>
            /// The colliding object has a target name.
            /// </summary>
            Name = 2,
            /// <summary>
            /// The colliding object is target <see cref="Collider"/>.
            /// </summary>
            Collider = 3,
            /// <summary>
            /// The colliding object is target <see cref="TriggerTarget"/>.
            /// </summary>
            TriggerTarget = 4,
            /// <summary>
            /// The colliding object has target <see cref="TriggerTarget.TargetId"/>.
            /// </summary>
            TargetID = 5
        }

        #endregion

        #region Context Menu

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/GIB/Trigger Agent", false, 10)]
        private static void CreateEventAgent()
        {
            GameObject go = new GameObject("Trigger Agent");
            go.AddComponent<TriggerAgent>();
        }
#endif

        #endregion

        #region Odin
        public bool HasDetectionList()
        {
#if UNITY_2020_2_OR_NEWER
            return selectiveDetection switch
            {
                DetectTarget.Tags => true,
                DetectTarget.Name => true,
                DetectTarget.TargetID => true,
                _ => false,
            };
# else
            switch (selectiveDetection)
                {
                    case DetectTarget.Tags:
                        return true;
                    case DetectTarget.Name:
                        return true;
                    case DetectTarget.TargetID:
                        return true;
                    default:
                        return false;
                }
            
#endif
        }

        private bool IsVisibleSelected() => triggerSource == EventTriggers.IsVisible;
        #endregion
    }
}