using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorBusStates : MonoBehaviour
    {
        #region Bus
        private enum EnableType { Awake, OnEnable, Start }

        [SerializeField, BoxGroup("Settings", ShowLabel = false), HorizontalGroup("Settings/sett", 200), LabelWidth(60)] private string _busId;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/sett"), HideLabel] private EffectorStates _states;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/sett", 115), LabelText("Enable"), LabelWidth(80)] private bool _enableGet;

        private void OnEnable()
        {
            Bus.BusSystem.Subscribe<string>(_busId, PlayState);
            if (_enableGet)
                PlayState(Bus.BusSystem.Get<string>(_busId, ""));
        }
        private void OnDisable()
        {
            Bus.BusSystem.Unsubscribe<string>(_busId, PlayState);
        }
        private void PlayState(string state)
        {
            _states.PlaySmooth(state);
        }
        #endregion
    }
}