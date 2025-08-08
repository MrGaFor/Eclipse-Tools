using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Cursors
{
    [HideMonoScript]
    public class CursorCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

#if !UNITY_IOS && !UNITY_ANDROID
        private void Awake()
        {
            CursorManager.InstanceCamera(_camera);
        }
#endif
    }
}
