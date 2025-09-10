using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	/// <summary>
	///   <para>Custom Editor for the Button Component.</para>
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ButtonPro), true)]
	public class ButtonProEditor : SelectableEditor
	{
		SerializedProperty m_UsePointerEventDataProperty;
		SerializedProperty m_OnClickProperty;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			serializedObject.Update();
			m_UsePointerEventDataProperty = serializedObject.FindProperty("m_UsePointerData");
			EditorGUILayout.PropertyField(m_UsePointerEventDataProperty, new GUIContent("Use payload ?", "Use event payload associated with pointer (mouse / touch) events as a dynamic parameter ?"));

			if (m_UsePointerEventDataProperty.boolValue)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PointerDataEvent"));
			else
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnClick"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}