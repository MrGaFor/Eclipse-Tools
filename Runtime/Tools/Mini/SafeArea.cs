using Sirenix.OdinInspector;
using UnityEngine;

namespace Ec.Mini
{
    [HideMonoScript]
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private Vector2 minOffset;
        [SerializeField] private Vector2 maxOffset;

        private RectTransform rectTransform;
        private Rect safeArea;
        private Vector2 minAnchor;
        private Vector2 maxAnchor;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            safeArea = Screen.safeArea;
            safeArea.min += minOffset;
            safeArea.min = new Vector2(Mathf.Max(safeArea.min.x, 0), Mathf.Max(safeArea.min.y, 0));
            safeArea.max += maxOffset;
            safeArea.max = new Vector2(Mathf.Min(safeArea.max.x, Screen.width), Mathf.Min(safeArea.max.y, Screen.height));
            //Debug.Log("Screen width: " + Screen.width);
            //Debug.Log("Screen height: " + Screen.height);
            //Debug.Log("Max X: " + safeArea.max.x);
            //Debug.Log("Max Y: " + safeArea.max.y);
            //Debug.Log("Min X: " + safeArea.min.x);
            //Debug.Log("Min Y: " + safeArea.min.y);

            minAnchor = safeArea.position;
            maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }
}