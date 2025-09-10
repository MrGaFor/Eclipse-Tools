﻿using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.Utility;

namespace UnityEngine.UI
{
	/// <summary>
	/// A standard slider that can be moved between a minimum and maximum value.
	/// </summary>
	/// <remarks>
	/// The slider component is a Selectable that controls a fill, a handle, or both. The fill, when used, spans from the minimum value to the current value while the handle, when used, follow the current value.
	/// The anchors of the fill and handle RectTransforms are driven by the Slider. The fill and handle can be direct children of the GameObject with the Slider, or intermediary RectTransforms can be placed in between for additional control.
	/// When a change to the slider value occurs, a callback is sent to any registered listeners of UI.Slider.onValueChanged.
	/// </remarks>
	[AddComponentMenu("UI/Slider Pro", 34)]
	[RequireComponent(typeof(RectTransform))]
	public class SliderPro : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		[Serializable]
		public class EP_SliderPro : EventPro<float> { }

		[SerializeField]
        [Tooltip("Optional RectTransform to use as fill for the slider.")]
		private RectTransform m_FillRect;
		/// <summary>
		/// Optional RectTransform to use as fill for the slider.
		/// </summary>
		public RectTransform fillRect { get { return m_FillRect; } set { if (EP_SetPropertyUtility.SetClass(ref m_FillRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

		[SerializeField]
		[Tooltip("Optional RectTransform to use as a handle for the slider.")]
		private RectTransform m_HandleRect;
		/// <summary>
		/// Optional RectTransform to use as a handle for the slider.
		/// </summary>
		public RectTransform handleRect { get { return m_HandleRect; } set { if (EP_SetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

		[Space]

		[SerializeField]
		[Tooltip("The direction of the slider, from minimum to maximum value.")]
		private Slider.Direction m_Direction = Slider.Direction.LeftToRight;
		/// <summary>
		/// The direction of the slider, from minimum to maximum value.
		/// </summary>
		public Slider.Direction direction { get { return m_Direction; } set { if (EP_SetPropertyUtility.SetStruct(ref m_Direction, value)) UpdateVisuals(); } }

		[SerializeField]
		[Tooltip("The minimum allowed value of the slider.")]
		private float m_MinValue = 0;
		/// <summary>
		/// The minimum allowed value of the slider.
		/// </summary>

		public float minValue { get { return m_MinValue; } set { if (EP_SetPropertyUtility.SetStruct(ref m_MinValue, value)) { Set(m_Value); UpdateVisuals(); } } }


		[SerializeField]
		[Tooltip("The maximum allowed value of the slider.")]
		private float m_MaxValue = 1;
		/// <summary>
		/// The maximum allowed value of the slider.
		/// </summary>
		public float maxValue { get { return m_MaxValue; } set { if (EP_SetPropertyUtility.SetStruct(ref m_MaxValue, value)) { Set(m_Value); UpdateVisuals(); } } }

		[SerializeField]
		[Tooltip("Whether the value should only allow whole numbers.")]
		private bool m_WholeNumbers = false;
		/// <summary>
		/// Whether the value should only allow whole numbers.
		/// </summary>
		public bool wholeNumbers { get { return m_WholeNumbers; } set { if (EP_SetPropertyUtility.SetStruct(ref m_WholeNumbers, value)) { Set(m_Value); UpdateVisuals(); } } }

		[SerializeField]
		[Tooltip("Whether the value should only allow whole numbers.")]
		protected float m_Value;
		/// <summary>
		/// The current value of the slider.
		/// </summary>
		public virtual float value
        {
            get => wholeNumbers ? Mathf.Round(m_Value) : m_Value;
            set
            {
                Set(value);
            }
        }


		/// <summary>
		/// Set the value of the slider without invoking onValueChanged callback.
		/// </summary>
		/// <param name="input">The new value for the slider.</param>
		public virtual void SetValueWithoutNotify(float input)
		{
			Set(input, false);
		}

		/// <summary>
		/// The current value of the slider normalized into a value between 0 and 1.
		/// </summary>
		public float normalizedValue
		{
			get
			{
				if (Mathf.Approximately(minValue, maxValue))
					return 0;
				return Mathf.InverseLerp(minValue, maxValue, value);
			}
			set
			{
				this.value = Mathf.Lerp(minValue, maxValue, value);
			}
		}

		[Space]

		// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
		[SerializeField]
		[Tooltip("Events to trigger when this slider value is changed")]
		private EP_SliderPro m_OnValueChanged = new EP_SliderPro();
		/// <summary>
		/// Callback executed when the value of the slider is changed.
		/// </summary>
		public EP_SliderPro onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

		// Private fields

		private Image m_FillImage;
		private Transform m_FillTransform;
		private RectTransform m_FillContainerRect;
		private Transform m_HandleTransform;
		private RectTransform m_HandleContainerRect;

		// The offset from handle position to mouse down position
		private Vector2 m_Offset = Vector2.zero;

#pragma warning disable 649
		private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

		private bool m_DelayedUpdateVisuals = false;

		// Size of each step.
		float stepSize { get { return wholeNumbers ? 1 : (maxValue - minValue) * 0.1f; } }

		protected SliderPro()
		{ }

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (wholeNumbers)
			{
				m_MinValue = Mathf.Round(m_MinValue);
				m_MaxValue = Mathf.Round(m_MaxValue);
			}

			//Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
			if (IsActive())
			{
				UpdateCachedReferences();
				// Update rects in next update since other things might affect them even if value didn't change.
				m_DelayedUpdateVisuals = true;
			}

			if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}
			


#endif
		public virtual void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout)
				onValueChanged.Invoke(value);
#endif
		}

		/// <summary>
		/// <seealso cref="ICanvasElement.LayoutComplete"/>
		/// </summary>
		public virtual void LayoutComplete()
		{ }

		/// <summary>
		/// <seealso cref="ICanvasElement.GraphicUpdateComplete"/>
		/// </summary>
		public virtual void GraphicUpdateComplete()
		{ }

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateCachedReferences();
			Set(m_Value, false);
			// Update rects since they need to be initialized correctly.
			UpdateVisuals();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			base.OnDisable();
		}

		/// <summary>
		/// Update the rect based on the delayed update visuals.
		/// Got around issue of calling sendMessage from onValidate.
		/// </summary>
		protected virtual void Update()
		{
			if (m_DelayedUpdateVisuals)
			{
				m_DelayedUpdateVisuals = false;
				Set(m_Value, false);
				UpdateVisuals();
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			// Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
			// We also need to ensure the value stays within min/max.
			m_Value = ClampValue(m_Value);
			float oldNormalizedValue = normalizedValue;
			if (m_FillContainerRect != null)
			{
				if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
					oldNormalizedValue = m_FillImage.fillAmount;
				else
					oldNormalizedValue = (reverseValue ? 1 - m_FillRect.anchorMin[(int)axis] : m_FillRect.anchorMax[(int)axis]);
			}
			else if (m_HandleContainerRect != null)
				oldNormalizedValue = (reverseValue ? 1 - m_HandleRect.anchorMin[(int)axis] : m_HandleRect.anchorMin[(int)axis]);

			UpdateVisuals();

			if (oldNormalizedValue != normalizedValue)
			{
				UISystemProfilerApi.AddMarker("SliderPro.value", this);
				onValueChanged.Invoke(m_Value);
			}
		}

		private void UpdateCachedReferences()
		{
			if (m_FillRect && m_FillRect != (RectTransform)transform)
			{
				m_FillTransform = m_FillRect.transform;
				m_FillImage = m_FillRect.GetComponent<Image>();
				if (m_FillTransform.parent != null)
					m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
			}
			else
			{
				m_FillRect = null;
				m_FillContainerRect = null;
				m_FillImage = null;
			}

			if (m_HandleRect && m_HandleRect != (RectTransform)transform)
			{
				m_HandleTransform = m_HandleRect.transform;
				if (m_HandleTransform.parent != null)
					m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
			}
			else
			{
				m_HandleRect = null;
				m_HandleContainerRect = null;
			}
		}

		private float ClampValue(float input)
		{
			float newValue = Mathf.Clamp(input, minValue, maxValue);
			if (wholeNumbers)
				newValue = Mathf.Round(newValue);
			return newValue;
		}

		// Set the valueUpdate the visible Image.
		protected virtual void Set(float input)
		{
			Set(input, true);
		}

		protected virtual void Set(float input, bool sendCallback)
		{
			// Clamp the input
			float newValue = ClampValue(input);

			// If the stepped value doesn't match the last one, it's time to update
			if (m_Value == newValue)
				return;

			m_Value = newValue;
			UpdateVisuals();
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("Slider.value", this);
				m_OnValueChanged.Invoke(newValue);
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			//This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
			if (!IsActive())
				return;

			UpdateVisuals();
		}

		enum Axis
		{
			Horizontal = 0,
			Vertical = 1
		}

		Axis axis { get { return (m_Direction == Slider.Direction.LeftToRight || m_Direction == Slider.Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }
		bool reverseValue { get { return m_Direction == Slider.Direction.RightToLeft || m_Direction == Slider.Direction.TopToBottom; } }

		// Force-update the slider. Useful if you've changed the properties and want it to update visually.
		private void UpdateVisuals()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				UpdateCachedReferences();
#endif

			m_Tracker.Clear();

			if (m_FillContainerRect != null)
			{
				m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;

				if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
				{
					m_FillImage.fillAmount = normalizedValue;
				}
				else
				{
					if (reverseValue)
						anchorMin[(int)axis] = 1 - normalizedValue;
					else
						anchorMax[(int)axis] = normalizedValue;
				}

				m_FillRect.anchorMin = anchorMin;
				m_FillRect.anchorMax = anchorMax;
			}

			if (m_HandleContainerRect != null)
			{
				m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;
				anchorMin[(int)axis] = anchorMax[(int)axis] = (reverseValue ? (1 - normalizedValue) : normalizedValue);
				m_HandleRect.anchorMin = anchorMin;
				m_HandleRect.anchorMax = anchorMax;
			}
		}

		// Update the slider's position based on the mouse.
		void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform clickRect = m_HandleContainerRect ?? m_FillContainerRect;
			if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
			{
				Vector2 position = Vector2.zero;
				if (!EP_MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
					return;

				Vector2 localCursor;
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
					return;
				localCursor -= clickRect.rect.position;

				float val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / clickRect.rect.size[(int)axis]);
				normalizedValue = (reverseValue ? 1f - val : val);
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			base.OnPointerDown(eventData);

			m_Offset = Vector2.zero;
			if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
			{
				Vector2 localMousePos;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
					m_Offset = localMousePos;
			}
			else
			{
				// Outside the slider handle - jump to this point instead
				UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;
			UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				base.OnMove(eventData);
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
						Set(reverseValue ? value + stepSize : value - stepSize);
					else
						base.OnMove(eventData);
					break;
				case MoveDirection.Right:
					if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
						Set(reverseValue ? value - stepSize : value + stepSize);
					else
						base.OnMove(eventData);
					break;
				case MoveDirection.Up:
					if (axis == Axis.Vertical && FindSelectableOnUp() == null)
						Set(reverseValue ? value - stepSize : value + stepSize);
					else
						base.OnMove(eventData);
					break;
				case MoveDirection.Down:
					if (axis == Axis.Vertical && FindSelectableOnDown() == null)
						Set(reverseValue ? value + stepSize : value - stepSize);
					else
						base.OnMove(eventData);
					break;
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
				return null;
			return base.FindSelectableOnLeft();
		}

		public override Selectable FindSelectableOnRight()
		{
			if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
				return null;
			return base.FindSelectableOnRight();
		}

		public override Selectable FindSelectableOnUp()
		{
			if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
				return null;
			return base.FindSelectableOnUp();
		}

		public override Selectable FindSelectableOnDown()
		{
			if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
				return null;
			return base.FindSelectableOnDown();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		/// <summary>
		/// Sets the direction of this slider, optionally changing the layout as well.
		/// </summary>
		/// <param name="direction">The direction of the slider</param>
		/// <param name="includeRectLayouts">Should the layout be flipped together with the slider direction</param>

		public void SetDirection(Slider.Direction direction, bool includeRectLayouts)
		{
			Axis oldAxis = axis;
			bool oldReverse = reverseValue;
			this.direction = direction;

			if (!includeRectLayouts)
				return;

			if (axis != oldAxis)
				RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

			if (reverseValue != oldReverse)
				RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
		}
	}
}