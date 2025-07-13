using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Cursors
{
    [HideMonoScript]
    public class CursorCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            CursorManager.SetCursor(_camera);
        }

    }
}
