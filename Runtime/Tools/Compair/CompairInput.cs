using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Compair
{
    [HideMonoScript]
    public class CompairInput : MonoBehaviour
    {
        #region Data
        [System.Serializable]
        private class CompairEventSettingsInput : CompairSettingsDefault
        {
            [SerializeField, HorizontalGroup("idandauto", Order = -1), LabelWidth(70)] private string _id;
            [SerializeField, HorizontalGroup("idandauto", Order = -1), LabelWidth(70)] private AutoCall _autoCall;

            public string GetId() => _id;
            public AutoCall GetAutoCall() => _autoCall;
        }

        [SerializeField, BoxGroup("Data", ShowLabel = false)] private CompairEventSettingsInput[] _events;
        #endregion

        #region AutoCall
        private void Awake() => CheckCall(AutoCall.Awake);
        private void Start() => CheckCall(AutoCall.Start);
        private void OnEnable() => CheckCall(AutoCall.OnEnable);
        private void OnDisable() => CheckCall(AutoCall.OnDisable);
        private void OnDestroy() => CheckCall(AutoCall.OnDestroy);
        private void CheckCall(AutoCall type)
        {
            foreach (var setting in _events)
                if (setting.GetAutoCall() == type)
                    setting.Invoke();
        }
        #endregion

        #region CallIndex
        public void CallIndex(int index)
        {
            if (index < 0 || index >= _events.Length)
            {
                Debug.LogErrorFormat("Compair: Input => Call index outside massive length!", this);
                return;
            }
            _events[index].Invoke();
        }
        public void CallId(string id)
        {
            int index = -1;
            for (int i = 0; i < _events.Length; i++)
                if (_events[i].GetId() == id)
                {
                    index = i;
                    break;
                }
            if (index == -1)
            {
                Debug.LogErrorFormat("Compair: Input => Call id dont include in massive!", this);
                return;
            }
            CallIndex(index);
        }
        #endregion
    }
}