using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Bus
{
    public enum BusSettingsType { Int, Float, Bool, String, Char, Vector2, Vector2Int, Vector3, Vector3Int, Quaternion, Color, Gradient, KeyCode, Transform, GameObject, Camera, Object }


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
            { BusSettingsType.Gradient, typeof(Gradient) },
            { BusSettingsType.KeyCode, typeof(KeyCode) },
            { BusSettingsType.Transform, typeof(Transform) },
            { BusSettingsType.GameObject, typeof(GameObject) },
            { BusSettingsType.Camera, typeof(Camera) },
            { BusSettingsType.Object, typeof(UnityEngine.Object) },
        };
        public static readonly Dictionary<BusSettingsType, Action<IBusInSettings>> BusToCallEventsSafe = new()
        {
            { BusSettingsType.Int, data => BusSettingsExecutor.CallSafe<int>(data) },
            { BusSettingsType.Float, data => BusSettingsExecutor.CallSafe<float>(data) },
            { BusSettingsType.Bool, data => BusSettingsExecutor.CallSafe<bool>(data) },
            { BusSettingsType.String, data => BusSettingsExecutor.CallSafe<string>(data) },
            { BusSettingsType.Char, data => BusSettingsExecutor.CallSafe<char>(data) },
            { BusSettingsType.Vector2, data => BusSettingsExecutor.CallSafe<Vector2>(data) },
            { BusSettingsType.Vector2Int, data => BusSettingsExecutor.CallSafe<Vector2Int>(data) },
            { BusSettingsType.Vector3, data => BusSettingsExecutor.CallSafe<Vector3>(data) },
            { BusSettingsType.Vector3Int, data => BusSettingsExecutor.CallSafe<Vector3Int>(data) },
            { BusSettingsType.Quaternion, data => BusSettingsExecutor.CallSafe<Quaternion>(data) },
            { BusSettingsType.Color, data => BusSettingsExecutor.CallSafe<Color>(data) },
            { BusSettingsType.Gradient, data => BusSettingsExecutor.CallSafe<Gradient>(data) },
            { BusSettingsType.KeyCode, data => BusSettingsExecutor.CallSafe<KeyCode>(data) },
            { BusSettingsType.Transform, data => BusSettingsExecutor.CallSafe<Transform>(data) },
            { BusSettingsType.GameObject, data => BusSettingsExecutor.CallSafe<GameObject>(data) },
            { BusSettingsType.Camera, data => BusSettingsExecutor.CallSafe<Camera>(data) },
            { BusSettingsType.Object, data => BusSettingsExecutor.CallSafe<UnityEngine.Object>(data) },
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