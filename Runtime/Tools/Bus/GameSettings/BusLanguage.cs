using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.GameSettings
{
    [HideMonoScript]
    public class BusLanguage : MonoBehaviour
    {
        [SerializeField, HorizontalGroup("input"), LabelWidth(40)] private string _key = "Language";

        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<int>(_key, OnUpdateLanguage);
            if (Bus.BusSystem.HasKey(_key))
                OnUpdateLanguage(Bus.BusSystem.Get<int>(_key));
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<int>(_key, OnUpdateLanguage);
        }

        private void OnUpdateLanguage(int language)
        {
            EC.Localization.LocalizationSystem.SetLanguage(language);
        }
    }
}
