#if UNITY_EDITOR && EC_HIERARCHY
using UnityEditor;

namespace CustomHierarchy
{
    public static partial class CustomHierarchyPreferences
    {
        private class BoolPreference : Preference<bool>
        {
            protected override bool GetImpl()
            {
                return EditorPrefs.GetBool(Key);
            }

            protected override void SetImpl(bool value)
            {
                EditorPrefs.SetBool(Key, value);
                OnSettingsChangedEvents.Invoke();
            }
        }
    }
}
#endif