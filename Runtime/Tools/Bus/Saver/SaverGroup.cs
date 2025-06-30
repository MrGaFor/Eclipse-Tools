using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Saver
{
    [HideMonoScript]
    public class SaverGroup : MonoBehaviour
    {
        [System.Serializable]
        public class SaveKey
        {
            [HorizontalGroup("data"), LabelWidth(40)] public string Key;
            [HorizontalGroup("data", 60), HideLabel] public SaverType Type;

            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Int)] public int _intValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Float)] public float _floatValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Bool)] public bool _boolValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.String)] public string _stringValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Vector2)] public Vector2 _vector2Value;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Vector3)] public Vector3 _vector3Value;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Quaternion)] public Quaternion _quaternionValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Color)] public Color _colorValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.Gradient)] public Gradient _gradientValue;
            [HorizontalGroup("data"), LabelWidth(40), LabelText("Value"), ShowIf("Type", SaverType.KeyCode)] public KeyCode _keyCodeValue;
        }
        [SerializeField] private SaveKey[] _keys;

        private void Awake()
        {
            foreach (var key in _keys)
            {
                switch (key.Type)
                {
                    case SaverType.Int: SaverSystem.AddVariable<int>(key.Key, key._intValue); break;
                    case SaverType.Float: SaverSystem.AddVariable<float>(key.Key, key._floatValue); break;
                    case SaverType.Bool: SaverSystem.AddVariable<bool>(key.Key, key._boolValue); break;
                    case SaverType.String: SaverSystem.AddVariable<string>(key.Key, key._stringValue); break;
                    case SaverType.Vector2: SaverSystem.AddVariable<Vector2>(key.Key, key._vector2Value); break;
                    case SaverType.Vector3: SaverSystem.AddVariable<Vector3>(key.Key, key._vector3Value); break;
                    case SaverType.Quaternion: SaverSystem.AddVariable<Quaternion>(key.Key, key._quaternionValue); break;
                    case SaverType.Color: SaverSystem.AddVariable<Color>(key.Key, key._colorValue); break;
                    case SaverType.Gradient: SaverSystem.AddVariable<Gradient>(key.Key, key._gradientValue); break;
                    case SaverType.KeyCode: SaverSystem.AddVariable<KeyCode>(key.Key, key._keyCodeValue); break;
                }
            }
        }

    }
}
