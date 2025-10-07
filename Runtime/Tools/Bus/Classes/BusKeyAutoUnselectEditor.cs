#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EC.Bus.EditorTools
{
    [InitializeOnLoad]
    static class BusKeySelectionWatcher
    {
        static GameObject _lastSelected;

        static BusKeySelectionWatcher()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            var current = Selection.activeGameObject;
            var prev = _lastSelected;
            _lastSelected = current;
            if (prev == null) return;
            EditorApplication.delayCall += () => UnselectAllIn(prev);
        }

        private static void UnselectAllIn(GameObject go)
        {
            if (go == null) return;

            foreach (var mb in go.GetComponents<MonoBehaviour>())
            {
                var fields = mb.GetType().GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                bool dirty = false;
                foreach (var f in fields)
                {
                    var val = f.GetValue(mb);
                    if (val == null) continue;

                    if (val is BusKey b) { b.Unselect(); dirty = true; }
                    else if (val is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                            if (item is BusKey ib) { ib.Unselect(); dirty = true; }
                    }
                }

                if (dirty)
                    EditorUtility.SetDirty(mb);
            }
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
}
#endif
