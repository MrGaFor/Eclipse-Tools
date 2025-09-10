using UnityEditor;

namespace GIB.EditorUtilities
{
    /// <summary>
    /// Locks the inspector window.
    /// </summary>
    static class EditorMenus
    {
        [MenuItem("Tools/GIB Toolkit/Toggle Inspector Lock &#q")]
        static void ToggleInspectorLock()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}