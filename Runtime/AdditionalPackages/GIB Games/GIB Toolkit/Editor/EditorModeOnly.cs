using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GIB.EditorUtilities
{
    [InitializeOnLoad]
    public class EditorModeOnly
    {
        private const string MENU_NAME = "Tools/GIB Toolkit/Destroy EditorOnly on play";
        public static string EditorModeOnlyTag = "EditorOnly";
        private static bool _enabled;

        static EditorModeOnly()
        {
            EditorModeOnly._enabled = EditorPrefs.GetBool(EditorModeOnly.MENU_NAME, false);
            EditorApplication.delayCall += () => PerformAction(EditorModeOnly._enabled);

            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if ((asset == null) || (asset.Length <= 0)) return;

            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");
            bool hasEditorModeOnlyTag = false;

            for (int i = 0; i < tags.arraySize; ++i)
            {
                SerializedProperty element = tags.GetArrayElementAtIndex(i);
                if (element.stringValue == EditorModeOnlyTag)
                {
                    hasEditorModeOnlyTag = true;
                }
            }

            if (hasEditorModeOnlyTag) return;

            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = EditorModeOnlyTag;
            so.ApplyModifiedProperties();
            so.Update();
        }

        private static GameObject[] FindGameObjectsByTag(string tag)
        {
            List<GameObject> validTransforms = new List<GameObject>();
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();

            foreach (Transform t in objs)
            {
                if (t.hideFlags == HideFlags.None && t.gameObject.CompareTag(tag))
                {
                    validTransforms.Add(t.gameObject);
                }
            }
            return validTransforms.ToArray();
        }

        [PostProcessScene(0)]
        public static void OnPostprocessScene()
        {
            if (!EditorModeOnly._enabled) return;
            UnityEngine.Debug.Log("Destroying EditorOnly Objects.");

            GameObject[] editorModeTaggedObjects = FindGameObjectsByTag(EditorModeOnlyTag);

            foreach (GameObject go in editorModeTaggedObjects)
            {
                // Filter out gameobjects in resources folder if we are just entering playmode and not building the game
                if (!go.scene.IsValid())
                    continue;

                Object.DestroyImmediate(go, false);
            }
        }

        [MenuItem(EditorModeOnly.MENU_NAME)]
        private static void ToggleAction() => PerformAction(!EditorModeOnly._enabled);

        public static void PerformAction(bool enabled)
        {
            // Set checkmark on menu item
            Menu.SetChecked(EditorModeOnly.MENU_NAME, enabled);
            // Saving editor state
            EditorPrefs.SetBool(EditorModeOnly.MENU_NAME, enabled);

            EditorModeOnly._enabled = enabled;
        }

    }
}