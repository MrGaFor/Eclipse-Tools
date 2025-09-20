using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace EC.Mini
{
    [HideMonoScript]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField, BoxGroup()] private string _key;
        [SerializeField, BoxGroup()] private float _defaultTime = 1f;
        [SerializeField, BoxGroup()] private Effects.EffectorStates _effectorStates;

        public void OnEnable()
        {
            Bus.BusSystem.Subscribe(_key, () => Loading(_defaultTime));
            Bus.BusSystem.Subscribe<bool>(_key, isActive => Loading(isActive).Forget());
            Bus.BusSystem.Subscribe<float>(_key, (time) => Loading(time));
            Bus.BusSystem.Subscribe<UniTask>(_key, (task) => Loading(task));
        }
        public void OnDisable()
        {
            Bus.BusSystem.Unsubscribe(_key, () => Loading(_defaultTime));
            Bus.BusSystem.Unsubscribe<bool>(_key, isActive => Loading(isActive).Forget());
            Bus.BusSystem.Unsubscribe<float>(_key, (time) => Loading(time));
            Bus.BusSystem.Unsubscribe<UniTask>(_key, (task) => Loading(task));
        }

        private async UniTask Loading(bool isActive)
        {
            await _effectorStates.PlaySmoothAsync(isActive ? "Show" : "Hide");
        }
        private async void Loading(float time)
        {
            await Loading(true);
            await UniTask.Delay((int)(time * 1000));
            await Loading(false);
        }
        private async void Loading(UniTask task)
        {
            await Loading(true);
            await task;
            await Loading(false);
        }

    }
}
