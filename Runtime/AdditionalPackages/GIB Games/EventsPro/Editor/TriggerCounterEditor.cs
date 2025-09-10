#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using GIB.Triggers;

[CustomEditor(typeof(TriggerCounter))]
public class TriggerCounterEditor : Editor
{
    SerializedProperty triggerValueProp;
    SerializedProperty targetValuesProp;

    private void OnEnable()
    {
        triggerValueProp = serializedObject.FindProperty("TriggerValue");
        targetValuesProp = serializedObject.FindProperty("TargetValues");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(triggerValueProp);

        CheckForDuplicateTargetValues();

        ShowTargetValuesList();

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowTargetValuesList()
    {
        EditorGUILayout.PropertyField(targetValuesProp, new GUIContent("Target Values"));

        if (GUILayout.Button("Add New Target Value"))
        {
            targetValuesProp.arraySize++;
            serializedObject.ApplyModifiedProperties();

            int lastValue = triggerValueProp.intValue;
            if (targetValuesProp.arraySize > 1)
            {
                SerializedProperty lastItem = targetValuesProp.GetArrayElementAtIndex(targetValuesProp.arraySize - 2);
                lastValue = lastItem.FindPropertyRelative("targetValue").intValue;
            }
            targetValuesProp.GetArrayElementAtIndex(targetValuesProp.arraySize - 1).FindPropertyRelative("targetValue").intValue = lastValue + 1;
        }
    }

    private void CheckForDuplicateTargetValues()
    {
        var targetValues = new int[targetValuesProp.arraySize];
        for (int i = 0; i < targetValuesProp.arraySize; i++)
        {
            SerializedProperty itemProp = targetValuesProp.GetArrayElementAtIndex(i);
            targetValues[i] = itemProp.FindPropertyRelative("targetValue").intValue;
        }

        if (targetValues.Distinct().Count() != targetValues.Length)
        {
            EditorGUILayout.HelpBox("There are duplicate target values in the list.", MessageType.Warning);
        }
    }
}
#endif