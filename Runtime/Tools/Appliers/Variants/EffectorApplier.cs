using EC.Appliers.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Appliers.Variants
{
    [Applier(typeof(Effects.IEffectorComponent), typeof(Color), "Effector", "Color")]
    public class EffectorColorApplier : ApplierBase<Effects.IEffectorComponent, Color>
    {
        private enum EffectVariant { Smooth, Moment }
        [SerializeField, HideLabel, HorizontalGroup("Target", 75)] private EffectVariant _variant;

        public override bool HasGetter => false;
        public override Color Value => Color.clear;
        public override void SetValue(Color value)
        {
            if (!Application.isPlaying || _variant == EffectVariant.Moment)
                Target.PlayMomentCustom(value);
            else
                Target.PlaySmoothCustom(value);
        }
    }
}
