using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;
using UnityEngine.UI.Utility;

namespace UnityEngine.UI
{
	/// <summary>
	///   A standard dropdown that presents a list of options when clicked, of which one can be chosen.
	/// </summary>
	/// <remarks>
	/// The dropdown component is a Selectable. When an option is chosen, the label and/or image of the control changes to show the chosen option.
	///
	/// When a dropdown event occurs a callback is sent to any registered listeners of onValueChanged.
	/// </remarks>
	[AddComponentMenu("UI/Dropdown Pro", 36)]
	[RequireComponent(typeof(RectTransform))]
	public class DropdownPro : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
	{
		public enum CallbackType
		{
			Value,
			Label,
			Sprite,
			ValueLabel,
			ValueSprite,
			LabelSprite,
			ValueLabelSprite
		}

		protected class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler
		{
			[SerializeField]
			private Text m_Text;
			[SerializeField]
			private Image m_Image;
			[SerializeField]
			private RectTransform m_RectTransform;
			[SerializeField]
			private TogglePro m_Toggle;

			public Text text { get { return m_Text; } set { m_Text = value; } }
			public Image image { get { return m_Image; } set { m_Image = value; } }
			public RectTransform rectTransform { get { return m_RectTransform; } set { m_RectTransform = value; } }
			public TogglePro toggle { get { return m_Toggle; } set { m_Toggle = value; } }

            public virtual void OnPointerEnter(PointerEventData eventData) =>
				EventSystem.current.SetSelectedGameObject(gameObject);

            public virtual void OnCancel(BaseEventData eventData)
			{
				DropdownPro dropdown = GetComponentInParent<DropdownPro>();
				if (dropdown)
					dropdown.Hide();
			}
		}

		[Serializable]
		public class EP_DropdownIndex : EventPro<int> { }
		[Serializable]
		public class EP_DropdownLabel : EventPro<string> { }
		[Serializable]
		public class EP_DropdownSprite : EventPro<Sprite> { }
		[Serializable]
		public class EP_DropdownIndexLabel : EventPro<int, string> { }
		[Serializable]
		public class EP_DropdownIndexSprite : EventPro<int, Sprite> { }
		[Serializable]
		public class EP_DropdownLabelSprite : EventPro<string, Sprite> { }
		[Serializable]
		public class EP_DropdownAll : EventPro<int, string, Sprite> { }

		[SerializeField]
		private RectTransform m_Template;
		/// <summary>
		/// The Rect Transform of the template for the dropdown list.
		/// </summary>
		public RectTransform template { get { return m_Template; } set { m_Template = value; RefreshShownValue(); } }

		[SerializeField]
		private Text m_CaptionText;
		/// <summary>
		/// The Text component to hold the text of the currently selected option.
		/// </summary>
		public Text captionText { get { return m_CaptionText; } set { m_CaptionText = value; RefreshShownValue(); } }

		[SerializeField]
		private Image m_CaptionImage;
		/// <summary>
		/// The Image component to hold the image of the currently selected option.
		/// </summary>
		public Image captionImage { get { return m_CaptionImage; } set { m_CaptionImage = value; RefreshShownValue(); } }

		[Space]

		[SerializeField]
		private Text m_ItemText;
		/// <summary>
		/// The Text component to hold the text of the item.
		/// </summary>
		public Text itemText { get { return m_ItemText; } set { m_ItemText = value; RefreshShownValue(); } }

		[SerializeField]
		private Image m_ItemImage;
		/// <summary>
		/// The Image component to hold the image of the item
		/// </summary>
		public Image itemImage { get { return m_ItemImage; } set { m_ItemImage = value; RefreshShownValue(); } }

		[Space]

		[SerializeField]
		private int m_Value;

		[Space]

		[SerializeField]
		private Dropdown.OptionDataList m_Options = new Dropdown.OptionDataList();
		/// <summary>
		/// The list of possible options. A text string and an image can be specified for each option.
		/// </summary>
		/// <remarks>
		/// This is the list of options within the Dropdown. Each option contains Text and/or image data that you can specify using UI.Dropdown.OptionData before adding to the Dropdown list.
		/// This also unlocks the ability to edit the Dropdown, including the insertion, removal, and finding of options, as well as other useful tools
		/// </remarks>
		public List<Dropdown.OptionData> options
		{
			get { return m_Options.options; }
			set { m_Options.options = value; RefreshShownValue(); }
		}

		[Space]

		// Notification triggered when the dropdown changes.
		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed")]
		private EP_DropdownIndex m_OnValueChanged = new EP_DropdownIndex();
		public EP_DropdownIndex onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownLabel m_OnLabelChanged = new EP_DropdownLabel();
		public EP_DropdownLabel onLabelChanged { get { return m_OnLabelChanged; } set { m_OnLabelChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownSprite m_OnSpriteChanged = new EP_DropdownSprite();
		public EP_DropdownSprite onSpriteChanged { get { return m_OnSpriteChanged; } set { m_OnSpriteChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownIndexLabel m_OnValueLabelChanged = new EP_DropdownIndexLabel();
		public EP_DropdownIndexLabel onValueLabelChanged { get { return m_OnValueLabelChanged; } set { m_OnValueLabelChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownIndexSprite m_OnValueSpriteChanged = new EP_DropdownIndexSprite();
		public EP_DropdownIndexSprite onValueSpriteChanged { get { return m_OnValueSpriteChanged; } set { m_OnValueSpriteChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownLabelSprite m_OnLabelSpriteChanged = new EP_DropdownLabelSprite();
		public EP_DropdownLabelSprite onLabelSpriteChanged { get { return m_OnLabelSpriteChanged; } set { m_OnLabelSpriteChanged = value; } }

		[SerializeField]
		[Tooltip("Events to trigger when this dropdown option is changed. The index of the new selected option is passed as parameter")]
		private EP_DropdownAll m_OnValueLabelSpriteChanged = new EP_DropdownAll();
		public EP_DropdownAll onValueLabelSpriteChanged { get { return m_OnValueLabelSpriteChanged; } set { m_OnValueLabelSpriteChanged = value; } }

		[SerializeField]
		private CallbackType m_CallbackType = CallbackType.Value;
		public CallbackType callbackType { get { return m_CallbackType; } set { m_CallbackType = value; } }

		[SerializeField]
		private float m_AlphaFadeSpeed = 0.15f;

		/// <summary>
		/// The time interval at which a drop down will appear and disappear
		/// </summary>
		public float alphaFadeSpeed { get { return m_AlphaFadeSpeed; } set { m_AlphaFadeSpeed = value; } }

		private GameObject m_Dropdown;
		private GameObject m_Blocker;
		private List<DropdownItem> m_Items = new List<DropdownItem>();
		private TweenRunnerPro<FloatTweenPro> m_AlphaTweenRunner;
		private bool validTemplate = false;

		private static Dropdown.OptionData s_NoOptionData = new Dropdown.OptionData();

		// Current value.
		public int value
		{
			get
			{
				return m_Value;
			}
			set
			{
				if (Application.isPlaying && (value == m_Value || options.Count == 0))
					return;

				m_Value = Mathf.Clamp(value, 0, options.Count - 1);
				RefreshShownValue();

				// Notify all listeners
				UISystemProfilerApi.AddMarker("DropdownPro.value", this);

				switch (callbackType)
				{
					case CallbackType.Value:
						onValueChanged.Invoke(m_Value);
						break;
					case CallbackType.Label:
						onLabelChanged.Invoke(options[m_Value].text);
						break;
					case CallbackType.Sprite:
						onSpriteChanged.Invoke(options[m_Value].image);
						break;
					case CallbackType.ValueLabel:
						onValueLabelChanged.Invoke(m_Value, options[m_Value].text);
						break;
					case CallbackType.ValueSprite:
						onValueSpriteChanged.Invoke(m_Value, options[m_Value].image);
						break;
					case CallbackType.LabelSprite:
						onLabelSpriteChanged.Invoke(options[m_Value].text, options[m_Value].image);
						break;
					case CallbackType.ValueLabelSprite:
						onValueLabelSpriteChanged.Invoke(m_Value, options[m_Value].text, options[m_Value].image);
						break;
				}
			}
		}

		/// <summary>
		/// Set index number of the current selection in the Dropdown without invoking onValueChanged callback.
		/// </summary>
		/// <param name="input"> The new index for the current selection. </param>
		public void SetValueWithoutNotify(int input)
		{
			Set(input, false);
		}

		void Set(int value, bool sendCallback = true)
		{
			if (Application.isPlaying && (value == m_Value || options.Count == 0))
				return;

			m_Value = Mathf.Clamp(value, 0, options.Count - 1);
			RefreshShownValue();

			if (sendCallback)
			{
				// Notify all listeners
				UISystemProfilerApi.AddMarker("Dropdown.value", this);
				m_OnValueChanged.Invoke(m_Value);
			}
		}

		protected DropdownPro()
		{ }

		protected override void Awake()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (m_CaptionImage)
				m_CaptionImage.enabled = (m_CaptionImage.sprite != null);

			if (m_Template)
				m_Template.gameObject.SetActive(false);
		}

        protected override void Start()
        {
			m_AlphaTweenRunner = new TweenRunnerPro<FloatTweenPro>();
			m_AlphaTweenRunner.Init(this);
			base.Start();
			RefreshShownValue();
		}

#if UNITY_EDITOR
        protected override void OnValidate()
		{
			base.OnValidate();

			if (!IsActive())
				return;

			RefreshShownValue();
		}

#endif

		protected override void OnDisable()
		{
			//Destroy dropdown and blocker in case user deactivates the dropdown when they click an option (case 935649)
			ImmediateDestroyDropdownList();

			if (m_Blocker != null)
				DestroyBlocker(m_Blocker);
			m_Blocker = null;

			base.OnDisable();
		}

		public void RefreshShownValue()
		{
			Dropdown.OptionData data = s_NoOptionData;

			if (options.Count > 0)
				data = options[Mathf.Clamp(m_Value, 0, options.Count - 1)];

			if (m_CaptionText)
			{
				if (data != null && data.text != null)
					m_CaptionText.text = data.text;
				else
					m_CaptionText.text = "";
			}

			if (m_CaptionImage)
			{
				if (data != null)
					m_CaptionImage.sprite = data.image;
				else
					m_CaptionImage.sprite = null;
				m_CaptionImage.enabled = (m_CaptionImage.sprite != null);
			}
		}

		/// <summary>
		/// Add multiple options to the options of the Dropdown based on a list of OptionData objects.
		/// </summary>
		/// <param name="options">The list of OptionData to add.</param>
		/// /// <remarks>
		/// See AddOptions(List<string> options) for code example of usages.
		/// </remarks>
		public void AddOptions(List<Dropdown.OptionData> options)
		{
			this.options.AddRange(options);
			RefreshShownValue();
		}

		/// <summary>
		/// Add multiple text-only options to the options of the Dropdown based on a list of strings.
		/// </summary>
		/// <remarks>
		/// Add a List of string messages to the Dropdown. The Dropdown shows each member of the list as a separate option.
		/// </remarks>
		/// <param name="options">The list of text strings to add.</param>
		public void AddOptions(List<string> options)
		{
			for (int i = 0; i < options.Count; i++)
				this.options.Add(new Dropdown.OptionData(options[i]));
			RefreshShownValue();
		}

		/// <summary>
		/// Add multiple image-only options to the options of the Dropdown based on a list of Sprites.
		/// </summary>
		/// <param name="options">The list of Sprites to add.</param>
		/// <remarks>
		/// See AddOptions(List<string> options) for code example of usages.
		/// </remarks>
		public void AddOptions(List<Sprite> options)
		{
			for (int i = 0; i < options.Count; i++)
				this.options.Add(new Dropdown.OptionData(options[i]));
			RefreshShownValue();
		}

		/// <summary>
		/// Clear the list of options in the Dropdown.
		/// </summary>
		public void ClearOptions()
		{
			options.Clear();
			RefreshShownValue();
		}

		private void SetupTemplate()
		{
			validTemplate = false;

			if (!m_Template)
			{
				Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
				return;
			}

			GameObject templateGo = m_Template.gameObject;
			templateGo.SetActive(true);
			TogglePro itemToggle = m_Template.GetComponentInChildren<TogglePro>();

			validTemplate = true;
			if (!itemToggle || itemToggle.transform == template)
			{
				validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", template);
			}
			else if (!(itemToggle.transform.parent is RectTransform))
			{
				validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", template);
			}
			else if (itemText != null && !itemText.transform.IsChildOf(itemToggle.transform))
			{
				validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", template);
			}
			else if (itemImage != null && !itemImage.transform.IsChildOf(itemToggle.transform))
			{
				validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", template);
			}

			if (!validTemplate)
			{
				templateGo.SetActive(false);
				return;
			}

			DropdownItem item = itemToggle.gameObject.AddComponent<DropdownItem>();
			item.text = m_ItemText;
			item.image = m_ItemImage;
			item.toggle = itemToggle;
			item.rectTransform = (RectTransform)itemToggle.transform;

			// Find the Canvas that this dropdown is a part of
			Canvas parentCanvas = null;
			Transform parentTransform = m_Template.parent;
			while (parentTransform != null)
			{
				parentCanvas = parentTransform.GetComponent<Canvas>();
				if (parentCanvas != null)
					break;

				parentTransform = parentTransform.parent;
			}

			Canvas popupCanvas = GetOrAddComponent<Canvas>(templateGo);
			popupCanvas.overrideSorting = true;
			popupCanvas.sortingOrder = 30000;

			// If we have a parent canvas, apply the same raycasters as the parent for consistency.
			if (parentCanvas != null)
			{
				Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
				for (int i = 0; i < components.Length; i++)
				{
					Type raycasterType = components[i].GetType();
					if (templateGo.GetComponent(raycasterType) == null)
					{
						templateGo.AddComponent(raycasterType);
					}
				}
			}
			else
			{
				GetOrAddComponent<GraphicRaycaster>(templateGo);
			}

			GetOrAddComponent<CanvasGroup>(templateGo);
			templateGo.SetActive(false);

			validTemplate = true;
		}

		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T comp = go.GetComponent<T>();
			if (!comp)
				comp = go.AddComponent<T>();
			return comp;
		}

		/// <summary>
		/// Handling for when the dropdown is initially 'clicked'. Typically shows the dropdown
		/// </summary>
		/// <param name="eventData">The asocciated event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData) => Show();

		/// <summary>
		/// Handling for when the dropdown is selected and a submit event is processed. Typically shows the dropdown
		/// </summary>
		/// <param name="eventData">The asocciated event data.</param>
		public virtual void OnSubmit(BaseEventData eventData) => Show();

		/// <summary>
		/// This will hide the dropdown list.
		/// </summary>
		/// <remarks>
		/// Called by a BaseInputModule when a Cancel event occurs.
		/// </remarks>
		/// <param name="eventData">The asocciated event data.</param>
		public virtual void OnCancel(BaseEventData eventData) => Hide();

		/// <summary>
		/// Show the dropdown.
		///
		/// Plan for dropdown scrolling to ensure dropdown is contained within screen.
		///
		/// We assume the Canvas is the screen that the dropdown must be kept inside.
		/// This is always valid for screen space canvas modes.
		/// For world space canvases we don't know how it's used, but it could be e.g. for an in-game monitor.
		/// We consider it a fair constraint that the canvas must be big enough to contain dropdowns.
		/// </summary>
		public void Show()
		{
			if (!IsActive() || !IsInteractable() || m_Dropdown != null)
				return;

			// Get root Canvas.
			var list = EP_ListPool<Canvas>.Get();
			gameObject.GetComponentsInParent(false, list);
			if (list.Count == 0)
				return;

			// rootCanvas should be last element returned by GetComponentsInParent()
			var listCount = list.Count;
			Canvas rootCanvas = list[listCount - 1];
			for (int i = 0; i < listCount; i++)
			{
				if (list[i].isRootCanvas || list[i].overrideSorting)
				{
					rootCanvas = list[i];
					break;
				}
			}

			EP_ListPool<Canvas>.Release(list);

			if (!validTemplate)
			{
				SetupTemplate();
				if (!validTemplate)
					return;
			}

			m_Template.gameObject.SetActive(true);

			// popupCanvas used to assume the root canvas had the default sorting Layer, next line fixes (case 958281 - [UI] Dropdown list does not copy the parent canvas layer when the panel is opened)
			m_Template.GetComponent<Canvas>().sortingLayerID = rootCanvas.sortingLayerID;

			// Instantiate the drop-down template
			m_Dropdown = CreateDropdownList(m_Template.gameObject);
			m_Dropdown.name = "Dropdown List";
			m_Dropdown.SetActive(true);

			// Make drop-down RectTransform have same values as original.
			RectTransform dropdownRectTransform = m_Dropdown.transform as RectTransform;
			dropdownRectTransform.SetParent(m_Template.transform.parent, false);

			// Instantiate the drop-down list items

			// Find the dropdown item and disable it.
			DropdownItem itemTemplate = m_Dropdown.GetComponentInChildren<DropdownItem>();

			GameObject content = itemTemplate.rectTransform.parent.gameObject;
			RectTransform contentRectTransform = content.transform as RectTransform;
			itemTemplate.rectTransform.gameObject.SetActive(true);

			// Get the rects of the dropdown and item
			Rect dropdownContentRect = contentRectTransform.rect;
			Rect itemTemplateRect = itemTemplate.rectTransform.rect;

			// Calculate the visual offset between the item's edges and the background's edges
			Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + (Vector2)itemTemplate.rectTransform.localPosition;
			Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + (Vector2)itemTemplate.rectTransform.localPosition;
			Vector2 itemSize = itemTemplateRect.size;

			m_Items.Clear();

			TogglePro prev = null;
			for (int i = 0; i < options.Count; ++i)
			{
				Dropdown.OptionData data = options[i];
				DropdownItem item = AddItem(data, value == i, itemTemplate, m_Items);
				if (item == null)
					continue;

				// Automatically set up a toggle state change listener
				item.toggle.isOn = value == i;
				item.toggle.onValueChanged.AddListener(x => OnSelectItem(item.toggle));

				// Select current option
				if (item.toggle.isOn)
					item.toggle.Select();

				// Automatically set up explicit navigation
				if (prev != null)
				{
					Navigation prevNav = prev.navigation;
					Navigation toggleNav = item.toggle.navigation;
					prevNav.mode = Navigation.Mode.Explicit;
					toggleNav.mode = Navigation.Mode.Explicit;

					prevNav.selectOnDown = item.toggle;
					prevNav.selectOnRight = item.toggle;
					toggleNav.selectOnLeft = prev;
					toggleNav.selectOnUp = prev;

					prev.navigation = prevNav;
					item.toggle.navigation = toggleNav;
				}
				prev = item.toggle;
			}

			// Reposition all items now that all of them have been added
			Vector2 sizeDelta = contentRectTransform.sizeDelta;
			sizeDelta.y = itemSize.y * m_Items.Count + offsetMin.y - offsetMax.y;
			contentRectTransform.sizeDelta = sizeDelta;

			float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
			if (extraSpace > 0)
				dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);

			// Invert anchoring and position if dropdown is partially or fully outside of canvas rect.
			// Typically this will have the effect of placing the dropdown above the button instead of below,
			// but it works as inversion regardless of initial setup.
			Vector3[] corners = new Vector3[4];
			dropdownRectTransform.GetWorldCorners(corners);

			RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;
			Rect rootCanvasRect = rootCanvasRectTransform.rect;
			for (int axis = 0; axis < 2; axis++)
			{
				bool outside = false;
				for (int i = 0; i < 4; i++)
				{
					Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
					if (corner[axis] < rootCanvasRect.min[axis] || corner[axis] > rootCanvasRect.max[axis])
					{
						outside = true;
						break;
					}
				}
				if (outside)
					RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
			}

			var itemsCount = m_Items.Count;
			for (int i = 0; i < itemsCount; i++)
			{
				RectTransform itemRect = m_Items[i].rectTransform;
				itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0);
				itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0);
				itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (itemsCount - 1 - i) + itemSize.y * itemRect.pivot.y);
				itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
			}

			// Fade in the popup
			AlphaFadeList(m_AlphaFadeSpeed, 0f, 1f);

			// Make drop-down template and item template inactive
			m_Template.gameObject.SetActive(false);
			itemTemplate.gameObject.SetActive(false);

			m_Blocker = CreateBlocker(rootCanvas);
		}

		/// <summary>
		/// Create a blocker that blocks clicks to other controls while the dropdown list is open.
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to obtain a blocker GameObject.
		/// </remarks>
		/// <param name="rootCanvas">The root canvas the dropdown is under.</param>
		/// <returns>The created blocker object</returns>
		protected virtual GameObject CreateBlocker(Canvas rootCanvas)
		{
			// Create blocker GameObject.
			GameObject blocker = new GameObject("Blocker");

			// Setup blocker RectTransform to cover entire root canvas area.
			RectTransform blockerRect = blocker.AddComponent<RectTransform>();
			blockerRect.SetParent(rootCanvas.transform, false);
			blockerRect.anchorMin = Vector3.zero;
			blockerRect.anchorMax = Vector3.one;
			blockerRect.sizeDelta = Vector2.zero;

			// Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
			Canvas blockerCanvas = blocker.AddComponent<Canvas>();
			blockerCanvas.overrideSorting = true;
			Canvas dropdownCanvas = m_Dropdown.GetComponent<Canvas>();
			blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
			blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

			// Find the Canvas that this dropdown is a part of
			Canvas parentCanvas = null;
			Transform parentTransform = m_Template.parent;
			while (parentTransform != null)
			{
				parentCanvas = parentTransform.GetComponent<Canvas>();
				if (parentCanvas != null)
					break;

				parentTransform = parentTransform.parent;
			}

			// If we have a parent canvas, apply the same raycasters as the parent for consistency.
			if (parentCanvas != null)
			{
				Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
				for (int i = 0; i < components.Length; i++)
				{
					Type raycasterType = components[i].GetType();
					if (blocker.GetComponent(raycasterType) == null)
					{
						blocker.AddComponent(raycasterType);
					}
				}
			}
			else
			{
				// Add raycaster since it's needed to block.
				GetOrAddComponent<GraphicRaycaster>(blocker);
			}


			// Add image since it's needed to block, but make it clear.
			Image blockerImage = blocker.AddComponent<Image>();
			blockerImage.color = Color.clear;

			// Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
			Button blockerButton = blocker.AddComponent<Button>();
			blockerButton.onClick.AddListener(Hide);

			return blocker;
		}

		/// <summary>
		/// Convenience method to explicitly destroy the previously generated blocker object
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to dispose of a blocker GameObject that blocks clicks to other controls while the dropdown list is open.
		/// </remarks>
		/// <param name="blocker">The blocker object to destroy.</param>
		protected virtual void DestroyBlocker(GameObject blocker)
		{
			Destroy(blocker);
		}

		/// <summary>
		/// Create the dropdown list to be shown when the dropdown is clicked. The dropdown list should correspond to the provided template GameObject, equivalent to instantiating a copy of it.
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to obtain a dropdown list GameObject.
		/// </remarks>
		/// <param name="template">The template to create the dropdown list from.</param>
		/// <returns>The created drop down list gameobject.</returns>
		protected virtual GameObject CreateDropdownList(GameObject template)
		{
			return Instantiate(template);
		}

		/// <summary>
		/// Convenience method to explicitly destroy the previously generated dropdown list
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to dispose of a dropdown list GameObject.
		/// </remarks>
		/// <param name="dropdownList">The dropdown list GameObject to destroy</param>
		protected virtual void DestroyDropdownList(GameObject dropdownList)
		{
			Destroy(dropdownList);
		}

		/// <summary>
		/// Create a dropdown item based upon the item template.
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to obtain an option item.
		/// The option item should correspond to the provided template DropdownItem and its GameObject, equivalent to instantiating a copy of it.
		/// </remarks>
		/// <param name="itemTemplate">e template to create the option item from.</param>
		/// <returns>The created dropdown item component</returns>
		protected virtual DropdownItem CreateItem(DropdownItem itemTemplate)
		{
			return Instantiate(itemTemplate);
		}

		/// <summary>
		///  Convenience method to explicitly destroy the previously generated Items.
		/// </summary>
		/// <remarks>
		/// Override this method to implement a different way to dispose of an option item.
		/// Likely no action needed since destroying the dropdown list destroys all contained items as well.
		/// </remarks>
		/// <param name="item">The Item to destroy.</param>
		protected virtual void DestroyItem(DropdownItem item) { }

		// Add a new drop-down list item with the specified values.
		private DropdownItem AddItem(Dropdown.OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items)
		{
			// Add a new item to the dropdown.
			DropdownItem item = CreateItem(itemTemplate);
			item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);

			item.gameObject.SetActive(true);
			item.gameObject.name = "Item " + items.Count + (data.text != null ? ": " + data.text : "");

			if (item.toggle != null)
			{
				item.toggle.isOn = false;
			}

			// Set the item's data
			if (item.text)
				item.text.text = data.text;
			if (item.image)
			{
				item.image.sprite = data.image;
				item.image.enabled = (item.image.sprite != null);
			}

			items.Add(item);
			return item;
		}

		private void AlphaFadeList(float duration, float alpha)
		{
			CanvasGroup group = m_Dropdown.GetComponent<CanvasGroup>();
			AlphaFadeList(duration, group.alpha, alpha);
		}

		private void AlphaFadeList(float duration, float start, float end)
		{
			if (end.Equals(start))
				return;

			FloatTweenPro tween = new FloatTweenPro { duration = duration, startValue = start, targetValue = end };
			tween.AddOnChangedCallback(SetAlpha);
			tween.ignoreTimeScale = true;
			m_AlphaTweenRunner.StartTween(tween);
		}

		private void SetAlpha(float alpha)
		{
			if (!m_Dropdown)
				return;
			CanvasGroup group = m_Dropdown.GetComponent<CanvasGroup>();
			group.alpha = alpha;
		}

		/// <summary>
		/// Hide the dropdown list (close it)
		/// </summary>
		public void Hide()
		{
			if (m_Dropdown != null)
			{
				AlphaFadeList(0.15f, 0f);

				// User could have disabled the dropdown during the OnValueChanged call.
				if (IsActive())
					StartCoroutine(DelayedDestroyDropdownList(0.15f));
			}
			if (m_Blocker != null)
				DestroyBlocker(m_Blocker);
			m_Blocker = null;
			Select();
		}

		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			ImmediateDestroyDropdownList();
		}

		private void ImmediateDestroyDropdownList()
		{
			var itemsCount = m_Items.Count;
			for (int i = 0; i < itemsCount; i++)
			{
				if (m_Items[i] != null)
					DestroyItem(m_Items[i]);
			}
			m_Items.Clear();
			if (m_Dropdown != null)
				DestroyDropdownList(m_Dropdown);
			m_Dropdown = null;
		}

		// Change the value and hide the dropdown.
		private void OnSelectItem(TogglePro toggle)
		{
			if (!toggle.isOn)
				toggle.isOn = true;

			int selectedIndex = -1;
			Transform tr = toggle.transform;
			Transform parent = tr.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == tr)
				{
					// Subtract one to account for template child.
					selectedIndex = i - 1;
					break;
				}
			}

			if (selectedIndex < 0)
				return;

			value = selectedIndex;
			Hide();
		}
	}
}