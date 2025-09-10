using UnityEngine;
using UnityEditor;

namespace GIB.EditorUtilities
{
    /// <summary>
    /// Replaces the selected object with a specific prefab.
    /// </summary>
    public class ReplaceWithPrefab : EditorWindow
    {
        [SerializeField] private GameObject prefab;

        [MenuItem("Tools/GIB Toolkit/Replace With Prefab", false,100)]
        static void CreateReplaceWithPrefab()
        {
            GetWindow(typeof(ReplaceWithPrefab), false, "Replace With Prefab");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "This tool replaces the selected objects with a target prefab.",
                EditorStyles.wordWrappedLabel);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

            if (GUILayout.Button("Replace"))
            {
                var selection = Selection.gameObjects;

                for (var i = selection.Length - 1; i >= 0; --i)
                {
                    var selected = selection[i];
                    var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                    GameObject newObject;

                    if (prefabType == PrefabAssetType.Regular)
                    {
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    }
                    else
                    {
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                    }

                    if (newObject == null)
                    {
                        Debug.LogError("Error instantiating prefab");
                        break;
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                    newObject.transform.parent = selected.transform.parent;
                    newObject.transform.localPosition = selected.transform.localPosition;
                    newObject.transform.localRotation = selected.transform.localRotation;
                    newObject.transform.localScale = selected.transform.localScale;
                    newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                    Undo.DestroyObjectImmediate(selected);
                }
            }

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }
    }
}