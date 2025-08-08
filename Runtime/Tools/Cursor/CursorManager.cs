using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Cursors
{
    [HideMonoScript]
    public class CursorManager : MonoBehaviour
    {
#if !UNITY_IOS && !UNITY_ANDROID
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            I = Resources.Load<CursorManager>("[Cursors]");
            if (I == null)
            {
                Debug.LogWarning("[Cursors] Prefab not found in Resources folder. Please create a <<[Cursors]>> prefab and place it in the Resources folder.");
                return;
            }
            I = Instantiate(I);
            DontDestroyOnLoad(I.gameObject);
        }

        private static CursorManager I;

        [SerializeField, BoxGroup("UI", ShowLabel = false), LabelWidth(100)] private Canvas _canvas;
        [SerializeField, HorizontalGroup("UI/pivot"), LabelWidth(100)] private RectTransform _pivot;
        [SerializeField, HorizontalGroup("UI/pivot"), LabelWidth(100), LabelText("Lerp"), Range(0.1f, 1f)] private float _lerpValue = 0.9f;
        [SerializeField, BoxGroup("UI"), ListDrawerSettings(DraggableItems = false)] private CursorVariant[] _variants;

        [System.Serializable] private class CursorVariant
        {
            [HorizontalGroup, LabelWidth(50)] public string Id;
            [HorizontalGroup, LabelWidth(50)] public GameObject Cursor;
        }

        private static int SelectionIndex = 0;
        private static Camera TargetCamera;

        private void Awake()
        {
            if (!HasManager) return;
            Cursor.visible = false;
            foreach (var item in _variants)
                item.Cursor.SetActive(false);
            SelectCursor(0);
        }

        public static void InstanceCamera(Camera targetCamera)
        {
            if (!HasManager) return;
            TargetCamera = targetCamera;
            I._canvas.worldCamera = targetCamera;
        }

        public static void SetVisible(bool isVisible)
        {
            if (!HasManager) return;
            I._variants[SelectionIndex].Cursor.SetActive(isVisible);
        }

        public static void SelectCursor(int cursorNo)
        {
            if (!HasManager) return;
            I._variants[SelectionIndex].Cursor.SetActive(false);
            SelectionIndex = cursorNo;
            I._variants[SelectionIndex].Cursor.SetActive(true);
        }
        public static void SelectCursor(string cursorId)
        {
            if (!HasManager) return;
            if (I._variants.Length == 0)
            {
                Debug.LogWarning("[Cursors] Dont have cursors variant.");
                return;
            }
            for (int i = 0; i < I._variants.Length; i++)
                if (I._variants[i].Id == cursorId)
                {
                    SelectCursor(i);
                    break;
                }
        }

        private void LateUpdate()
        {
            if (!HasCamera) return;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _canvas.planeDistance;
            _pivot.position = Vector3.Lerp(_pivot.position, _canvas.worldCamera.ScreenToWorldPoint(mousePos), _lerpValue);
        }
        private static bool HasManager 
        { 
            get 
            {
                if (!I)
                    Debug.LogWarning("[Cursors] Manager not instanced.");
                return I; 
            } 
        }
        private static bool HasCamera
        {
            get
            {
                if (TargetCamera && I && !I._pivot.gameObject.activeSelf)
                    I._pivot.gameObject.SetActive(false);
                else if (!TargetCamera && I && I._pivot.gameObject.activeSelf)
                    I._pivot.gameObject.SetActive(true);
                return TargetCamera;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.visible = !focus;
            SetVisible(focus);
        }
#if UNITY_EDITOR
        private static CursorManager _editorCachePrefab;
        public static string[] GetIds()
        {
            if (I)
            {
                string[] ids = new string[I._variants.Length];
                if (I._variants.Length > 0)
                    for (int i = 0; i < I._variants.Length; i++)
                        ids[i] = I._variants[i].Id;
                return ids;
            }
            _editorCachePrefab = Resources.Load<CursorManager>("[Cursors]");
            if (_editorCachePrefab)
            {
                string[] ids = new string[_editorCachePrefab._variants.Length];
                if (_editorCachePrefab._variants.Length > 0)
                    for (int i = 0; i < _editorCachePrefab._variants.Length; i++)
                        ids[i] = _editorCachePrefab._variants[i].Id;
                return ids;
            }
            return new string[0];
        }
#else
        public static string[] GetIds() => new string[0];
#endif
#endif
    }
}