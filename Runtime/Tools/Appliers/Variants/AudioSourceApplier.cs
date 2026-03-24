using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(AudioSource), typeof(float), "AudioSource", "Volume"), ComponentIcon("AudioSource Icon")]
    public class AudioSourceVolumeApplier : ApplierBase<AudioSource, float>
    {
        public override float Value => Target.volume;
        public override void SetValue(float value) => Target.volume = value;
    }
    [Applier(typeof(AudioSource), typeof(float), "AudioSource", "Pitch"), ComponentIcon("AudioSource Icon")]
    public class AudioSourcePitchApplier : ApplierBase<AudioSource, float>
    {
        public override float Value => Target.pitch;
        public override void SetValue(float value) => Target.pitch = value;
    }
}
