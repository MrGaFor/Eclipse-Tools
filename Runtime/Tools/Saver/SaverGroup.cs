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
            [HorizontalGroup("data"), LabelWidth(40)] public SaverType Type;
        }
        [SerializeField] private SaveKey[] _keys;

        private void Awake()
        {
            foreach (var key in _keys)
            {
                switch (key.Type)
                {
                    case SaverType.Int: SaverSystem.AddVariable<int>(key.Key); break;
                    case SaverType.Float: SaverSystem.AddVariable<float>(key.Key); break;
                    case SaverType.Bool: SaverSystem.AddVariable<bool>(key.Key); break;
                    case SaverType.String: SaverSystem.AddVariable<string>(key.Key); break;
                    case SaverType.Vector2: SaverSystem.AddVariable<Vector2>(key.Key); break;
                    case SaverType.Vector3: SaverSystem.AddVariable<Vector3>(key.Key); break;
                    case SaverType.Quaternion: SaverSystem.AddVariable<Quaternion>(key.Key); break;
                    case SaverType.Color: SaverSystem.AddVariable<Color>(key.Key); break;
                    case SaverType.Gradient: SaverSystem.AddVariable<Gradient>(key.Key); break;
                    case SaverType.KeyCode: SaverSystem.AddVariable<KeyCode>(key.Key); break;
                }
            }
        }

    }
}
