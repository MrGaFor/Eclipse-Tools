using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.GPU
{
    [HideMonoScript]
    public class GPUInstanceChild : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderer;

        private void Start()
        {
            foreach (Renderer renderer in _renderer)
                renderer.SetPropertyBlock(GPUInstance.GetBlockForMaterial(renderer.sharedMaterial));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_renderer == null || _renderer != gameObject.GetComponentsInChildren<Renderer>())
                _renderer = gameObject.GetComponentsInChildren<Renderer>();
        }
#endif
    }
}