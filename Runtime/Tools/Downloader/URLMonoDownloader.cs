using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Downloader
{
    [HideMonoScript]
    public class URLMonoDownloader<T> : MonoBehaviour
    {
        [SerializeField, HideLabel] private URLDownloadTask<T> _task = new("");
        [FoldoutGroup("Events"), HorizontalGroup("Events/line1")] public UnityEvent OnStart;
        [FoldoutGroup("Events"), HorizontalGroup("Events/line1")] public UnityEvent<string> OnError;
        [FoldoutGroup("Events"), HorizontalGroup("Events/line2")] public UnityEvent<float> OnProgress;
        [FoldoutGroup("Events"), HorizontalGroup("Events/line2")] public UnityEvent<T> OnComplete;

        [Button, GUIColor(0.4f, 0.9f, 0.4f)]
        public void Download()
        {
            _task.GetFileAsync(
                force: true,
                onStart: () => OnStart?.Invoke(),
                onProgress: p => OnProgress?.Invoke(p),
                onComplete: f => OnComplete?.Invoke(f),
                onError: e => OnError?.Invoke(e)
            ).Forget();
        }

        public UniTask<T> GetFile() => _task.GetFileAsync();
        public UniTask<URLDownloader.DownloadResult<T>> GetData() => _task.GetResultAsync();
    }
}
