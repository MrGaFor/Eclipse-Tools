using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EC.GameSettings
{
    [HideMonoScript]
    public class BusRenderScale : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] protected string _key = "RenderScale";

        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<float>(_key, OnUpdateRenderScale);
            if (Bus.BusSystem.HasKey(_key))
                OnUpdateRenderScale(Bus.BusSystem.Get<float>(_key));
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<float>(_key, OnUpdateRenderScale);
        }

        private void OnUpdateRenderScale(float newScale)
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            urpAsset.renderScale = newScale;
        }
    }
}
