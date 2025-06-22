using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.GPU
{
    [HideMonoScript, RequireComponent(typeof(Renderer))]
    public class GPUInstanceLocal : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private void Start()
        {
            _renderer.SetPropertyBlock(GPUInstance.GetBlockForMaterial(_renderer.sharedMaterial));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (!_renderer)
                _renderer = GetComponent<Renderer>();
        }
#endif
    }
}