using Sirenix.OdinInspector;
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
                Debug.LogError("[Cursors] prefab not found in Resources folder. Please create a <<[Cursors]>> prefab and place it in the Resources folder.");
                return;
            }
            I = Instantiate(I);
            DontDestroyOnLoad(I.gameObject);
        }

        private static CursorManager I;

        [SerializeField, BoxGroup("UI", ShowLabel = false), LabelWidth(100)] private Canvas _canvas;
        [SerializeField, HorizontalGroup("UI/pivot"), LabelWidth(100)] private RectTransform _pivot;
        [SerializeField, HorizontalGroup("UI/pivot"), LabelWidth(100), LabelText("Lerp"), Range(0.1f, 1f)] private float _lerpValue = 0.9f;

        private Camera _targetCamera;

        private void Awake()
        {
            Cursor.visible = false;
        }

        public static void SetCursor(Camera targetCamera)
        {
            I._targetCamera = targetCamera;
            I._canvas.worldCamera = targetCamera;
        }

        private void LateUpdate()
        {
            if (!_targetCamera) return;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _canvas.planeDistance;
            _pivot.position = Vector3.Lerp(_pivot.position, _canvas.worldCamera.ScreenToWorldPoint(mousePos), _lerpValue);
        }
        private void OnApplicationFocus(bool focus)
        {
            Cursor.visible = !focus;
        }
#endif
    }
}