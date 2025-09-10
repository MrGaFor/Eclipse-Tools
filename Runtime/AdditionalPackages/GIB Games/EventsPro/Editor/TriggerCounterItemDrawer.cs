#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GIB.Triggers;

[CustomPropertyDrawer(typeof(TriggerCounterItem))]
public class TriggerCounterItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position.height = EditorGUI.GetPropertyHeight(property, label, true);
        EditorGUI.indentLevel++;

        // Split the position Rect into two parts
        Rect targetValueRect = new Rect(position.x, position.y, position.width * 0.15f, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("targetValue")));
        Rect eventProRect = new Rect(position.x + position.width * 0.17f, position.y, position.width * 0.83f, position.height);

        EditorGUI.PropertyField(targetValueRect, property.FindPropertyRelative("targetValue"), GUIContent.none);
        EditorGUI.PropertyField(eventProRect, property.FindPropertyRelative("OnTargetValue"), GUIContent.none, true);

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("OnTargetValue"), label, true);
    }
}
#endif