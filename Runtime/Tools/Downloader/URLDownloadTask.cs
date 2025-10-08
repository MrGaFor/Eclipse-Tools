using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using UnityEngine;

namespace EC.Downloader
{
    [Serializable]
    public class URLDownloadTask<T>
    {
        [SerializeField] private string _url;
        [ShowInInspector, ReadOnly, HideInEditorMode, HideLabel] private URLDownloader.DownloadResult<T> _result;

        public URLDownloadTask(string url = "") => _url = url;

        public bool HasResult => _result is { Success: true };
        public string Url => _url;
        public T File => _result.File;

        public async UniTask<T> GetFileAsync(
            bool force = false,
            Action onStart = null,
            Action<float> onProgress = null,
            Action<T> onComplete = null,
            Action<string> onError = null,
            CancellationToken token = default)
        {
            if (!force && HasResult) return _result.File;

            _result = await URLDownloader.DownloadAsync<T>(
                _url,
                onStart,
                onProgress,
                onComplete,
                onError,
                token
            );

            return _result.File;
        }

        public async UniTask<URLDownloader.DownloadResult<T>> GetResultAsync(
            bool force = false,
            Action onStart = null,
            Action<float> onProgress = null,
            Action<T> onComplete = null,
            Action<string> onError = null,
            CancellationToken token = default)
        {
            await GetFileAsync(force, onStart, onProgress, onComplete, onError, token);
            return _result;
        }
    }
}
