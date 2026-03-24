using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(CanvasGroup), typeof(float), "CanvasGroup", "Alpha"), ComponentIcon("CanvasGroup Icon")]
    public class CanvasGroupAlphaApplier : ApplierBase<CanvasGroup, float>
    {
        public override float Value => Target.alpha;
        public override void SetValue(float value) => Target.alpha = value;
    }
}
