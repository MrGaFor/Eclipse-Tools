using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public class BusOutput : MonoBehaviour
    {
        [SerializeField, BoxGroup("Data", ShowLabel = false)] private BusSettingsOutDefault[] _settings;

        #region LifeCycle
        private void OnEnable()
        {
            foreach (var settings in _settings)
                settings.Subscribe();
        }
        private void OnDisable()
        {
            foreach (var settings in _settings)
                settings.Unsubscribe();
        }
        #endregion
    }
}