using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Bus
{
    [HideMonoScript]
    public class BusInput : MonoBehaviour
    {
        #region CallCustom
        [SerializeField, BoxGroup("Custom", ShowLabel = false), HorizontalGroup("Custom/custom", 90), LabelWidth(70), LabelText("Is Custom")] private bool _custom;
        [SerializeField, BoxGroup("Custom"), HorizontalGroup("Custom/custom"), LabelWidth(70), LabelText("Key"), ShowIf("_custom")] private string _customKey;
        [SerializeField, BoxGroup("Custom"), HorizontalGroup("Custom/custom", 70), HideLabel, ShowIf("_custom")] private EventTypes _customEventType;

        public void CallCustom(int value) => CallCustom<int>(value);
        public void CallCustom(float value) => CallCustom<float>(value);
        public void CallCustom(bool value) => CallCustom<bool>(value);
        public void CallCustom(string value) => CallCustom<string>(value);
        public void CallCustom(char value) => CallCustom<char>(value);
        public void CallCustom(Vector2 value) => CallCustom<Vector2>(value);
        public void CallCustom(Vector2Int value) => CallCustom<Vector2Int>(value);
        public void CallCustom(Vector3 value) => CallCustom<Vector3>(value);
        public void CallCustom(Vector3Int value) => CallCustom<Vector3Int>(value);
        public void CallCustom(Quaternion value) => CallCustom<Quaternion>(value);
        public void CallCustom(Color value) => CallCustom<Color>(value);
        public void CallCustom(KeyCode value) => CallCustom<KeyCode>(value);
        public void CallCustom(Transform value) => CallCustom<Transform>(value);
        public void CallCustom(GameObject value) => CallCustom<GameObject>(value);
        public void CallCustom(Camera value) => CallCustom<Camera>(value);
        public void CallCustom(Object value) => CallCustom<Object>(value);

        private void CallCustom<T>(T value)
        {
            if (!_custom) return;
            BusSystem.CallEvent<T>(new BusSettingsGenericFix<T>(_customKey, _customEventType, value));
        }
        #endregion

        #region Data
        [global::System.SerializableAttribute]
        private class BusSettingsDefaultInput : BusSettingsInDefault
        {
            [SerializeField, HorizontalGroup("idandauto", Order = -1), LabelWidth(70)] private string _id;
            [SerializeField, HorizontalGroup("idandauto", Order = -1), LabelWidth(70)] private AutoCall _autoCall;

            public string GetId() => _id;
            public AutoCall GetAutoCall() => _autoCall;
        }

        [SerializeField, BoxGroup("Data", ShowLabel = false)] private BusSettingsDefaultInput[] _settings;


        #endregion

        #region AutoCall
        private void Awake() => CheckCall(AutoCall.Awake);
        private void Start() => CheckCall(AutoCall.Start);
        private void OnEnable() => CheckCall(AutoCall.OnEnable);
        private void OnDisable() => CheckCall(AutoCall.OnDisable);
        private void OnDestroy() => CheckCall(AutoCall.OnDestroy);
        private void CheckCall(AutoCall type)
        {
            foreach (var setting in _settings)
                if (setting.GetAutoCall() == type)
                    setting.Invoke();
        }
        #endregion

        #region CallIndex
        public void CallIndex(int index)
        {
            if (index < 0 || index >= _settings.Length)
            {
                Debug.LogErrorFormat("Bus: Input => Call index outside massive length!", this);
                return;
            }
            _settings[index].Invoke();
        }
        public void CallId(string id)
        {
            int index = -1;
            for (int i = 0; i < _settings.Length; i++)
                if (_settings[i].GetId() == id)
                {
                    index = i;
                    break;
                }
            if (index == -1)
            {
                Debug.LogErrorFormat("Bus: Input => Call id dont include in massive!", this);
                return;
            }
            CallIndex(index);
        }
        #endregion
    }
}