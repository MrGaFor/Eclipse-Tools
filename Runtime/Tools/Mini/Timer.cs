using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace EC
{
    [HideMonoScript]
    public class Timer : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings", false)] private EC.AutoCall _auto; 
        [SerializeField, HideLabel] private TimerCustom _timer;

        private void Awake() { if (_auto == AutoCall.Awake) StartTimer(_timer).Forget(); }
        private void Start() { if (_auto == AutoCall.Start) StartTimer(_timer).Forget(); }
        private void OnEnable() { if (_auto == AutoCall.OnEnable) StartTimer(_timer).Forget(); }
        private void OnDisable() { if (_auto == AutoCall.OnDisable) StartTimer(_timer).Forget(); }
        private void OnDestroy() { if (_auto == AutoCall.OnDestroy) StartTimer(_timer).Forget(); }

        public void StartTimer() => StartTimer(_timer).Forget();
        public void StopTimer() => StopTimer(_timer);
        public async UniTask StartTimerAsync() => await StartTimer(_timer);

        public static async UniTask StartTimer(TimerCustom timer)
        {
            await timer.StartTimer();
        }
        public static async UniTask StartTimer(float duration, UnityEvent onComplete = null)
        {
            await StartTimer(new TimerCustom(duration, onComplete));
        }
        public static void StopTimer(TimerCustom timer)
        {
            timer.Stop();
        }

    }
    [System.Serializable]
    public class TimerCustom
    {
        [BoxGroup("Settings", false)] public float Duration = 1f;
        [BoxGroup("Settings"), FoldoutGroup("Settings/Events"), HorizontalGroup("Settings/Events/evs1")] public UnityEvent OnStart;
        [BoxGroup("Settings"), FoldoutGroup("Settings/Events"), HorizontalGroup("Settings/Events/evs1")] public UnityEvent<float> OnTick;
        [BoxGroup("Settings"), FoldoutGroup("Settings/Events"), HorizontalGroup("Settings/Events/evs2")] public UnityEvent OnStop;
        [BoxGroup("Settings"), FoldoutGroup("Settings/Events"), HorizontalGroup("Settings/Events/evs2")] public UnityEvent OnComplete;

        private CancellationToken _process = new();

        public TimerCustom(float duration, UnityEvent onComplete)
        {
            Duration = duration;
            OnComplete = onComplete;
        }
        public TimerCustom(float duration, UnityEvent onStart = null, UnityEvent<float> onTick = null, UnityEvent onStop = null, UnityEvent onComplete = null)
        {
            Duration = duration;
            OnStart = onStart;
            OnTick = onTick;
            OnStop = onStop;
            OnComplete = onComplete;
        }

        public async UniTask StartTimer()
        {
            OnStart?.Invoke();
            _process = new();
            float t = 0f;
            while (t < Duration)
            {
                t += Time.deltaTime;
                await UniTask.Yield(_process);
                OnTick?.Invoke(t / Duration);
            }
            if (_process.CanBeCanceled)
                OnStop?.Invoke();
            else
                OnComplete?.Invoke();
        }
        public void Stop()
        {
            if (!_process.CanBeCanceled)
                _process.ThrowIfCancellationRequested();
        }
    }
}
