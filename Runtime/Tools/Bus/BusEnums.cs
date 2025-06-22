using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Bus
{
    public enum BusSettingsType { NONE, Int, Float, Bool, String, Char, Vector2, Vector2Int, Vector3, Vector3Int, Quaternion, Color, KeyCode, Transform, GameObject, Camera, Object }


    public static class BusConvert
    {
        public static readonly Dictionary<BusSettingsType, global::System.Type> BusSettingsTypes = new()
        {
            { BusSettingsType.Int, typeof(int) },
            { BusSettingsType.Float, typeof(float) },
            { BusSettingsType.Bool, typeof(bool) },
            { BusSettingsType.String, typeof(string) },
            { BusSettingsType.Char, typeof(char) },
            { BusSettingsType.Vector2, typeof(Vector2) },
            { BusSettingsType.Vector2Int, typeof(Vector2Int) },
            { BusSettingsType.Vector3, typeof(Vector3) },
            { BusSettingsType.Vector3Int, typeof(Vector3Int) },
            { BusSettingsType.Quaternion, typeof(Quaternion) },
            { BusSettingsType.Color, typeof(Color) },
            { BusSettingsType.KeyCode, typeof(KeyCode) },
            { BusSettingsType.Transform, typeof(Transform) },
            { BusSettingsType.GameObject, typeof(GameObject) },
            { BusSettingsType.Camera, typeof(Camera) },
            { BusSettingsType.Object, typeof(UnityEngine.Object) },
        };
        public static readonly Dictionary<BusSettingsType, Action<IBusInSettings>> BusToCallEvents = new()
        {
            { BusSettingsType.Int, data => BusSystem.CallEvent<int>(data) },
            { BusSettingsType.Float, data => BusSystem.CallEvent<float>(data) },
            { BusSettingsType.Bool, data => BusSystem.CallEvent<bool>(data) },
            { BusSettingsType.String, data => BusSystem.CallEvent<string>(data) },
            { BusSettingsType.Char, data => BusSystem.CallEvent<char>(data) },
            { BusSettingsType.Vector2, data => BusSystem.CallEvent<Vector2>(data) },
            { BusSettingsType.Vector2Int, data => BusSystem.CallEvent<Vector2Int>(data) },
            { BusSettingsType.Vector3, data => BusSystem.CallEvent<Vector3>(data) },
            { BusSettingsType.Vector3Int, data => BusSystem.CallEvent<Vector3Int>(data) },
            { BusSettingsType.Quaternion, data => BusSystem.CallEvent<Quaternion>(data) },
            { BusSettingsType.Color, data => BusSystem.CallEvent<Color>(data) },
            { BusSettingsType.KeyCode, data => BusSystem.CallEvent<KeyCode>(data) },
            { BusSettingsType.Transform, data => BusSystem.CallEvent<Transform>(data) },
            { BusSettingsType.GameObject, data => BusSystem.CallEvent<GameObject>(data) },
            { BusSettingsType.Camera, data => BusSystem.CallEvent<Camera>(data) },
            { BusSettingsType.Object, data => BusSystem.CallEvent<UnityEngine.Object>(data) },
        };
    }
    public enum EventTypes { Set, Invoke }
    public interface IBusInSettings
    {
        string GetKey();
        object GetValue();
        EventTypes GetEventType();
    }
    public interface IBusOutSettings
    {
        void Subscribe();
        void Unsubscribe();
    }
}