using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.GPU
{
    [HideMonoScript]
    public class GPUInstanceChild : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _renderer;

        private void Start()
        {
            foreach (MeshRenderer renderer in _renderer)
                if (!renderer.GetComponent<GPUInstanceLocalDisable>())
                    renderer.SetPropertyBlock(GPUInstance.GetBlockForMaterial(renderer.sharedMaterial));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_renderer == null || _renderer != gameObject.GetComponentsInChildren<MeshRenderer>())
                _renderer = gameObject.GetComponentsInChildren<MeshRenderer>();
        }
#endif
    }
}