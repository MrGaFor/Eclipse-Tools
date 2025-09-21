using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioFadeSettings : AudioModule
    {
        [SerializeField, LabelWidth(100), HorizontalGroup("hor"), VerticalGroup("hor/ver1")] private bool _fadeIn = false;
        [SerializeField, LabelWidth(100), VerticalGroup("hor/ver1"), ShowIf("_fadeIn"), LabelText("Time")] private float _inDuration = 0.1f;
        [SerializeField, LabelWidth(100), HorizontalGroup("hor"), VerticalGroup("hor/ver2")] private bool _fadeOut = false;
        [SerializeField, LabelWidth(100), VerticalGroup("hor/ver2"), ShowIf("_fadeOut"), LabelText("Time")] private float _outDuration = 0.1f;

        public float InDuration => _fadeIn ? _inDuration : 0f;
        public float OutDuration => _fadeOut ? _outDuration : 0f;

        public void Apply(AudioSource source)
        {
            if (_fadeIn)
            {
                float targetVolume = source.volume;
                source.volume = 0f;
                PrimeTween.Tween.AudioVolume(source, targetVolume, _inDuration);
            }
        }
        public void Stop(AudioSource source)
        {
            if (_fadeOut)
            {
                PrimeTween.Tween.AudioVolume(source, 0f, _outDuration).OnComplete(() => source.gameObject.SetActive(false));
            }
            else
            {
                source.gameObject.SetActive(false);
            }
        }

    }
}
