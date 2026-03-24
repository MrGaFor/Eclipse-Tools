using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(Light), typeof(float), "Light", "Intensity"), ComponentIcon("Light Icon")]
    public class LightIntensityApplier : ApplierBase<Light, float>
    {
        public override float Value => Target.intensity;
        public override void SetValue(float value) => Target.intensity = value;
    }
    [Applier(typeof(Light), typeof(float), "Light", "Range"), ComponentIcon("Light Icon")]
    public class LightRangeApplier : ApplierBase<Light, float>
    {
        public override float Value => Target.range;
        public override void SetValue(float value) => Target.range = value;
    }
    [Applier(typeof(Light), typeof(Color), "Light", "Color"), ComponentIcon("Light Icon")]
    public class LightColorApplier : ApplierBase<Light, Color>
    {
        public override Color Value => Target.color;
        public override void SetValue(Color value) => Target.color = value;
    }
}
