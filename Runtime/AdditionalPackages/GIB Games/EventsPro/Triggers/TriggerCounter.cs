using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GIB.Triggers
{
    /// <summary>
    /// The TriggerCounter class is responsible for tracking a trigger value and invoking events when specific target values are reached.
    /// </summary>
    public class TriggerCounter : MonoBehaviour
    {
        [Tooltip("The current value of the trigger.")]
        public int TriggerValue;
        [Tooltip("A list of target values and their associated events.")]
        public List<TriggerCounterItem> TargetValues;

        private Dictionary<int, int> internalTargetValues;
        private int initialValue;

        private void Awake()
        {
            internalTargetValues = new Dictionary<int, int>();

            foreach (var item in TargetValues)
            {
                if (!internalTargetValues.ContainsKey(item.targetValue))
                {
                    internalTargetValues.Add(item.targetValue, TargetValues.IndexOf(item));
                }
                else
                {
                    Debug.LogWarning("Duplicate target value found: " + item.targetValue);
                }
            }

            initialValue = TriggerValue;
        }

        /// <summary>
        /// Sets the trigger value to a specific target value and checks for any events to invoke.
        /// </summary>
        /// <param name="targetValue">The target value to set.</param>
        public void SetValue(int targetValue)
        {
            TriggerValue = targetValue;
            TryValue();
        }

        /// <summary>
        /// Adds a specified value to the current trigger value and checks for any events to invoke.
        /// </summary>
        /// <param name="targetValue">The value to add.</param>
        public void AddValue(int targetValue)
        {
            TriggerValue += targetValue;
            TryValue();
        }

        /// <summary>
        /// Subtracts a specified value to the current trigger value and checks for any events to invoke.
        /// </summary>
        /// <param name="targetValue">The value to subtract.</param>
        public void SubtractValue(int targetValue)
        {
            TriggerValue -= targetValue;
            TryValue();
        }

        /// <summary>
        /// Increments the current trigger value by 1 and checks for any events to invoke.
        /// </summary>
        public TriggerCounter IncrementValue()
        {
            TriggerValue++;
            TryValue();
            return this;
        }

        /// <summary>
        /// Decrements the current trigger value by 1 and checks for any events to invoke.
        /// </summary>
        public TriggerCounter DecrementValue()
        {
            TriggerValue--;
            TryValue();
            return this;
        }

        /// <summary>
        /// Checks if a certain value will trigger events and, if so, invokes them.
        /// </summary>
        public void TriggerValueWithoutChange(int targetValue)
        {
            if (internalTargetValues.TryGetValue(targetValue, out int eventToInvoke))
            {
                TargetValues[eventToInvoke].OnTargetValue.Invoke();
            }
        }

        /// <summary>
        /// Sets the trigger value to its initial value and checks for any events to invoke.
        /// </summary>
        public void ResetValue()
        {
            TriggerValue = initialValue;
            TryValue();
        }

        /// <summary>
        /// Sets the trigger value to a specific target value without invoking its associated events.
        /// </summary>
        /// <param name="targetValue">The target value to set.</param>
        public void SetValueWithoutNotify(int targetValue) => TriggerValue = targetValue;

        /// <summary>
        /// Adds a specified value to the current trigger value without invoking its associated events.
        /// </summary>
        /// <param name="targetValue">The value to add.</param>
        public void AddValueWithoutNotify(int targetValue) => TriggerValue += targetValue;

        /// <summary>
        /// Subtracts a specified value from the current trigger value without invoking its associated events.
        /// </summary>
        /// <param name="targetValue">The value to subtract.</param>
        public void SubtractValueWithoutNotify(int targetValue) => TriggerValue -= targetValue;

        /// <summary>
        /// Increments the current trigger value without invoking its associated events.
        /// </summary>
        public void IncrementValueWithoutNotify() => TriggerValue++;

        /// <summary>
        /// Decrements the current trigger value without invoking its associated events.
        /// </summary>
        public void DecrementValueWithoutNotify() => TriggerValue--;

        /// <summary>
        /// Sets the trigger value to its initial value without invoking its associated events.
        /// </summary>
        public void ResetValueWithoutNotify() => TriggerValue = initialValue;

        public static TriggerCounter operator ++(TriggerCounter counter)
        {
            counter.IncrementValue();
            return counter;
        }

        public static TriggerCounter operator --(TriggerCounter counter)
        {
            counter.DecrementValue();
            return counter;
        }

        private void TryValue()
        {
            if (internalTargetValues.TryGetValue(TriggerValue, out int eventToInvoke))
            {
                TargetValues[eventToInvoke].OnTargetValue.Invoke();
            }
        }
    }

    [System.Serializable]
    public class TriggerCounterItem
    {
        public int targetValue;
        public EventPro OnTargetValue;
    }
}