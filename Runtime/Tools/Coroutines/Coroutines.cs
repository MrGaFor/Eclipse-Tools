using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Coroutine
{
    [HideMonoScript]
    public class Coroutines : MonoBehaviour
    {
        public static void Init()
        {
            I = new GameObject("[Coroutines]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(I.gameObject);
        }

        public static Coroutines I;
    }
}