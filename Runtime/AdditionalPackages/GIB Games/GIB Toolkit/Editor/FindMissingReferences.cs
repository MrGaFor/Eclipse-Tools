#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GIB.EditorUtilities
{
    /// <summary>
    /// A helper editor script for finding missing references to objects.
    /// </summary>
    public class MissingReferencesFinder : MonoBehaviour
    {
        private const string MENU_ROOT = "Tools/GIB Toolkit/Find Missing References";

        /// <summary>
        /// Finds all missing references to objects in the currently loaded scene.
        /// </summary>
        [MenuItem(MENU_ROOT + "/Search in scene", false, 50)]
        public static void FindMissingReferencesInCurrentScene()
        {
            var sceneObjects = GetSceneObjects();
            FindMissingReferences(EditorSceneManager.GetActiveScene().path, sceneObjects);
        }

        /// <summary>
        /// Finds all missing references to objects in all enabled scenes in the project.
        /// This works by loading the scenes one by one and checking for missing object references.
        /// </summary>
        [MenuItem(MENU_ROOT + "/Search in all scenes", false, 51)]
        public static void FindMissingReferencesInAllScenes()
        {
            foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                EditorSceneManager.OpenScene(scene.path);
                FindMissingReferencesInCurrentScene();
            }
        }

        /// <summary>
        /// Finds all missing references to objects in assets (objects from the project window).
        /// </summary>
        [MenuItem(MENU_ROOT + "/Search in assets", false, 52)]
        public static void FindMissingReferencesInAssets()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
            GameObject[] objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject)
                .Where(a => a != null).ToArray();

            FindMissingReferences("Project", objs);
        }

        private static void FindMissingReferences(string context, GameObject[] gameObjects)
        {
            if (gameObjects == null)
            {
                return;
            }

            foreach (GameObject go in gameObjects)
            {
                Component[] components = go.GetComponents<Component>();

                foreach (Component c in components)
                {
                    // Missing components will be null, we can't find their type, etc.
                    if (!c)
                    {
                        Debug.LogErrorFormat(go, $"Missing Component in GameObject: {GetFullPath(go)}");

                        continue;
                    }

                    SerializedObject so = new SerializedObject(c);
                    SerializedProperty sp = so.GetIterator();

                    PropertyInfo objRefValueMethod = typeof(SerializedProperty).GetProperty("objectReferenceStringValue",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    // Iterate over the components' properties.
                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;

                        string objectReferenceStringValue = string.Empty;

                        if (objRefValueMethod != null)
                        {
                            objectReferenceStringValue =
                                (string) objRefValueMethod.GetGetMethod(true).Invoke(sp, new object[] { });
                        }

                        if (sp.objectReferenceValue == null
                            && (sp.objectReferenceInstanceIDValue != 0 ||
                                objectReferenceStringValue.StartsWith("Missing")))
                        {
                            ShowError(context, go, c.GetType().Name,
                                ObjectNames.NicifyVariableName(sp.name));
                        }
                    }
                }
            }
        }

        private static GameObject[] GetSceneObjects()
        {
            // Use this method since GameObject.FindObjectsOfType will not return disabled objects.
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
                             && go.hideFlags == HideFlags.None).ToArray();
        }

        private static void ShowError(string context, GameObject go, string componentName, string propertyName)
        {
            var ERROR_TEMPLATE = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

            Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
        }

        private static string GetFullPath(GameObject go)
        {
            return go.transform.parent == null
                ? go.name
                : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
        }
    }
}
#endif