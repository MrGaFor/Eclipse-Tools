using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	/// <summary>
	/// A standard button that sends an event when clicked.
	/// </summary>
	[AddComponentMenu("UI/Button Pro", 30)]
	public class ButtonPro : Selectable, IPointerClickHandler, ISubmitHandler
	{
		/// <summary>
		/// Function definition for a button click event.
		/// </summary>
		[System.Serializable]
		public class ButtonProClickEvent : EventPro { }

		[Tooltip("Events that are triggered when this button is clicked")]
		[SerializeField]
        [FormerlySerializedAs("onClick")]
		private ButtonProClickEvent m_OnClick = new ButtonProClickEvent();

		[SerializeField]
		private bool m_UsePointerData = false;

		[Tooltip("Events that are triggered when this button is clicked")]
		[SerializeField]
        [FormerlySerializedAs("onPointerData")]
		private PointerDataEvent m_PointerDataEvent = new PointerDataEvent();

		protected ButtonPro() { }

		private void Press(PointerEventData eventData = null)
		{
			if (!IsActive() || !IsInteractable())
				return;

			if (m_UsePointerData && eventData != null)
				m_PointerDataEvent.Invoke(eventData);
			else
				m_OnClick.Invoke();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!m_UsePointerData && eventData.button != PointerEventData.InputButton.Left)
				return;

			Press(eventData);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			Press();

			// if we get set disabled during the press
			// don't run the coroutine.
			if (!IsActive() || !IsInteractable())
				return;

			DoStateTransition(SelectionState.Pressed, false);
			StartCoroutine(OnFinishSubmit());
		}

		private IEnumerator OnFinishSubmit()
		{
			var fadeTime = colors.fadeDuration;
			var elapsedTime = 0f;

			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			DoStateTransition(currentSelectionState, false);
		}

		public ButtonProClickEvent onClick
		{
			get { return m_OnClick; }
			set { m_OnClick = value; }
		}

		public PointerDataEvent onPointerData
		{
			get { return m_PointerDataEvent; }
			set { m_PointerDataEvent = value; }
		}

		public bool usePayload
		{
			get { return m_UsePointerData; }
			set { m_UsePointerData = value; }
		}



		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class PointerDataEvent : EventPro<PointerEventData> { }
	}
}