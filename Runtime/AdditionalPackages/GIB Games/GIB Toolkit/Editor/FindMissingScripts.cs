using UnityEditor;
using UnityEngine;

namespace GIB.EditorUtilities
{
    public static class FindMissingScripts
    {
        [MenuItem("Tools/GIB Toolkit/Remove Missing Scripts", false, 100)]
        private static void FindAndRemoveMissingInSelected()
        {
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            int compCount = 0;
            int goCount = 0;

            // Count missing scripts before asking for confirmation
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {
                    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (count > 0)
                    {
                        compCount += count;
                        goCount++;
                    }
                }
            }

            // If there are no missing scripts, inform the user and return
            if (compCount == 0)
            {
                EditorUtility.DisplayDialog("Information",
                    "There are no missing scripts in the selected GameObjects.",
                    "OK");
                return;
            }

            // Ask for confirmation
            if (EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to remove {compCount} missing scripts from {goCount} GameObjects?",
                "Yes",
                "No"))
            {
                // If user pressed 'Yes', then remove the missing scripts
                foreach (var o in deepSelection)
                {
                    if (o is GameObject go)
                    {
                        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        if (count > 0)
                        {
                            // Edit: use undo record object, since undo destroy won't work with missing
                            Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        }
                    }
                }
                Debug.Log($"Removed {compCount} missing scripts from {goCount} GameObjects");
            }
        }
    }
}
