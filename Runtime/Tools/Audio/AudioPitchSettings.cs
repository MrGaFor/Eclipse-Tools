using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioPitchSettings : AudioModule
    {
        [SerializeField, LabelWidth(100)] private bool _pitch = false;
        [SerializeField, LabelWidth(100), ShowIf("_pitch"), MinMaxSlider(-3, 3, ShowFields = true)] private Vector2 _range = Vector2.one;

        public void Apply(AudioSource source)
        {
            source.pitch = _pitch ? Random.Range(_range.x, _range.y) : 1f;
        }
    }
}
