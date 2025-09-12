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
            Bus.BusSystem.Subscribe<float>(_key, (time) => Loading(time));
            Bus.BusSystem.Subscribe<UniTask>(_key, (task) => Loading(task));
        }
        public void OnDisable()
        {
            Bus.BusSystem.Unsubscribe(_key, () => Loading(_defaultTime));
            Bus.BusSystem.Unsubscribe<float>(_key, (time) => Loading(time));
            Bus.BusSystem.Unsubscribe<UniTask>(_key, (task) => Loading(task));
        }

        private async void Loading(float time)
        {
            await _effectorStates.PlaySmoothAsync("Show");
            await UniTask.Delay((int)(time * 1000));
            await _effectorStates.PlaySmoothAsync("Hide");
        }
        private async void Loading(UniTask task)
        {
            await _effectorStates.PlaySmoothAsync("Show");
            await task;
            await _effectorStates.PlaySmoothAsync("Hide");
        }

    }
}
