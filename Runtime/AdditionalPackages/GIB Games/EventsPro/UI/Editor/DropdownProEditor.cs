#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	/// <summary>
	/// Custom editor for the Dropdown component
	/// Extend this class to write a custom editor for a component derived from Dropdown.
	/// </summary>
	[CustomEditor(typeof(DropdownPro), true)]
	[CanEditMultipleObjects]
	public class DropdownProEditor : SelectableEditor
	{
		SerializedProperty m_Template;
		SerializedProperty m_CaptionText;
		SerializedProperty m_CaptionImage;
		SerializedProperty m_ItemText;
		SerializedProperty m_ItemImage;
		SerializedProperty m_Value;
		SerializedProperty m_Options;
		SerializedProperty m_AlphaFadeSpeed;
		SerializedProperty m_CallbackType;

		protected override void OnEnable()
		{
			base.OnEnable();
			m_Template = serializedObject.FindProperty("m_Template");
			m_CaptionText = serializedObject.FindProperty("m_CaptionText");
			m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
			m_ItemText = serializedObject.FindProperty("m_ItemText");
			m_ItemImage = serializedObject.FindProperty("m_ItemImage");
			m_Value = serializedObject.FindProperty("m_Value");
			m_Options = serializedObject.FindProperty("m_Options");
			m_AlphaFadeSpeed = serializedObject.FindProperty("m_AlphaFadeSpeed");
			m_CallbackType = serializedObject.FindProperty("m_CallbackType");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			serializedObject.Update();
			EditorGUILayout.PropertyField(m_Template);
			EditorGUILayout.PropertyField(m_CaptionText);
			EditorGUILayout.PropertyField(m_CaptionImage);
			EditorGUILayout.PropertyField(m_ItemText);
			EditorGUILayout.PropertyField(m_ItemImage);
			EditorGUILayout.PropertyField(m_Value);
			EditorGUILayout.PropertyField(m_AlphaFadeSpeed);
			EditorGUILayout.PropertyField(m_Options);

			EditorGUILayout.PropertyField(m_CallbackType, new GUIContent("Callback Type", "The type of callback when dropdown value changes"));

			DropdownPro.CallbackType callbackType = (DropdownPro.CallbackType)m_CallbackType.intValue;

#if UNITY_2020_2_OR_NEWER
			string callbackString = callbackType switch
			{
				DropdownPro.CallbackType.Value => "m_OnValueChanged",
				DropdownPro.CallbackType.Label => "m_OnLabelChanged",
				DropdownPro.CallbackType.Sprite => "m_OnSpriteChanged",
				DropdownPro.CallbackType.ValueLabel => "m_OnValueLabelChanged",
				DropdownPro.CallbackType.ValueSprite => "m_OnValueSpriteChanged",
				DropdownPro.CallbackType.LabelSprite => "m_OnLabelSpriteChanged",
				DropdownPro.CallbackType.ValueLabelSprite => "m_OnValueLabelSpriteChanged",
				_ => "m_OnValueChanged"
			};
#else
			string callbackString;

			switch (callbackType)
			{
				case DropdownPro.CallbackType.Value:
					callbackString = "m_OnValueChanged";
					break;
				case DropdownPro.CallbackType.Label:
					callbackString = "m_OnLabelChanged";
					break;
				case DropdownPro.CallbackType.Sprite:
					callbackString = "m_OnSpriteChanged";
					break;
				case DropdownPro.CallbackType.ValueLabel:
					callbackString = "m_OnValueLabelChanged";
					break;
				case DropdownPro.CallbackType.ValueSprite:
					callbackString = "m_OnValueSpriteChanged";
					break;
				case DropdownPro.CallbackType.LabelSprite:
					callbackString = "m_OnLabelSpriteChanged";
					break;
				case DropdownPro.CallbackType.ValueLabelSprite:
					callbackString = "m_OnValueLabelSpriteChanged";
					break;
				default:
					callbackString = "m_OnValueChanged";
					break;
			}
#endif

			EditorGUILayout.PropertyField(serializedObject.FindProperty(callbackString));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif