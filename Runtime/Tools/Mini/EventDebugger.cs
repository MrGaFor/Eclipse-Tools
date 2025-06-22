using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Mini
{
    [HideMonoScript]
    public class EventDebugger : MonoBehaviour
    {
        public static void DebugInt(int value) => DebugText(value.ToString());
        public static void DebugFloat(float value) => DebugText(value.ToString());
        public static void DebugBool(bool value) => DebugText(value.ToString());
        public static void DebugString(string value) => DebugText(value);
        public static void DebugVector2(Vector2 value) => DebugText(value.ToString());
        public static void DebugVector3(Vector3 value) => DebugText(value.ToString());
        public static void DebugGameObject(GameObject value) => DebugText(value.name.ToString());
        public static void DebugObject(Object value) => DebugText(value.name.ToString());

        private static void DebugText(string text)
        {
            Debug.Log(text);
        }

    }
}