using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace EC.Downloader
{
    public static class URLDownloader
    {
        [Serializable]
        public class DownloadResult<T>
        {
            public bool Success;
            public T File;
            public string Path;
            public int Size;
            public string Error;
            public Type Type => typeof(T);
        }

        public static async UniTask<DownloadResult<T>> DownloadAsync<T>(
            string url,
            Action onStart = null,
            Action<float> onProgress = null,
            Action<T> onComplete = null,
            Action<string> onError = null,
            CancellationToken token = default)
        {
            var result = new DownloadResult<T> { Path = url };

            if (string.IsNullOrWhiteSpace(url))
            {
                result.Error = "Empty URL";
                onError?.Invoke(result.Error);
                return result;
            }

            try
            {
                using var req = UnityWebRequest.Get(url);
                onStart?.Invoke();

                var op = req.SendWebRequest();
                while (!op.isDone)
                {
                    onProgress?.Invoke(op.progress);
                    await UniTask.Yield(token);
                }

                if (req.result != UnityWebRequest.Result.Success)
                {
                    result.Error = req.error;
                    Debug.LogError($"Download failed: {url}\n{req.error}");
                    onError?.Invoke(req.error);
                    return result;
                }

                var data = req.downloadHandler.data;
                result.File = ConvertData<T>(req, data);
                result.Size = data?.Length ?? 0;
                result.Success = true;
                onComplete?.Invoke(result.File);
                return result;
            }
            catch (Exception e)
            {
                result.Error = e.Message;
                onError?.Invoke(e.Message);
                return result;
            }
        }

        static T ConvertData<T>(UnityWebRequest req, byte[] data)
        {
            var t = typeof(T);

            if (t == typeof(string))
                return (T)(object)req.downloadHandler.text;

            if (t == typeof(Texture2D))
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);
                return (T)(object)tex;
            }

            if (t == typeof(AudioClip))
                return (T)(object)DownloadHandlerAudioClip.GetContent(req);

            if (!t.IsPrimitive && !string.IsNullOrWhiteSpace(req.downloadHandler.text))
                return JsonUtility.FromJson<T>(req.downloadHandler.text);

            return default;
        }
    }
}