using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EC.GameSettings
{
    [HideMonoScript]
    public class BusPostProcessing : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] protected string _key = "PostProcessing";
        [SerializeField, HorizontalGroup("input"), HideLabel] protected UniversalAdditionalCameraData _cameraData;

        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<bool>(_key, OnUpdatePostProcessing);
            if (Bus.BusSystem.HasKey(_key))
                OnUpdatePostProcessing(Bus.BusSystem.Get<bool>(_key));
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<bool>(_key, OnUpdatePostProcessing);
        }

        private void OnUpdatePostProcessing(bool activePostProcessing)
        {
            _cameraData.renderPostProcessing = activePostProcessing;
        }
    }
}
