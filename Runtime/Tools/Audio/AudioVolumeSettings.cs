using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioVolumeSettings : AudioModule
    {
        private string[] VolumeKeys => new[] { _keyVolume, "Sound", "Music" };
        [SerializeField, ValueDropdown("VolumeKeys", AppendNextDrawer = true), LabelWidth(100)] private string _keyVolume = "Sound";
        public string Key => _keyVolume;
        [SerializeField, Range(0f, 1f), LabelWidth(100), LabelText("Volume")] private float _volume = 1f;
        [SerializeField, LabelWidth(100), LabelText("Settings")] private AnimationCurve _volumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public void Apply(AudioSource source)
        {
            source.volume = _volume * _volumeCurve.Evaluate(Bus.BusSystem.Get<float>(_keyVolume, 1f));
        }
    }
}
