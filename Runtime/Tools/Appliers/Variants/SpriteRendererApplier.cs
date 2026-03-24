using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(SpriteRenderer), typeof(Color), "SpriteRenderer", "Color"), ComponentIcon("SpriteRenderer Icon")]
    public class SpriteRendererColorApplier : ApplierBase<SpriteRenderer, Color>
    {
        public override Color Value => Target.color;
        public override void SetValue(Color value) => Target.color = value;
    }
    [Applier(typeof(SpriteRenderer), typeof(float), "SpriteRenderer", "Alpha"), ComponentIcon("SpriteRenderer Icon")]
    public class SpriteRendererAlphaApplier : ApplierBase<SpriteRenderer, float>
    {
        public override float Value => Target.color.a;
        public override void SetValue(float value) => Target.color = new Color(Target.color.r, Target.color.g, Target.color.b, value);
    }
}
