using EC.Appliers.Core;
using UnityEngine.UI;

namespace EC.Appliers.Variants
{
    [Applier(typeof(Slider), typeof(float), "Slider", "Value"), ComponentIcon("Slider Icon")]
    public class SliderValueApplier : ApplierBase<Slider, float>
    {
        public override float Value => Target.value;
        public override void SetValue(float value) => Target.value = value;
    }
}
