using UnityEngine;
using UnityEngine.UI;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(Image), typeof(Color), "Image", "Color"), ComponentIcon("Image Icon")]
    public class ImageColorApplier : ApplierBase<Image, Color>
    {
        public override Color Value => Target.color;
        public override void SetValue(Color value) => Target.color = value;
    }
    [Applier(typeof(Image), typeof(float), "Image", "FillAmount"), ComponentIcon("Image Icon")]
    public class ImageFillAmountApplier : ApplierBase<Image, float>
    {
        public override float Value => Target.fillAmount;
        public override void SetValue(float value) => Target.fillAmount = value;
    }
    [Applier(typeof(Image), typeof(float), "Image", "Alpha"), ComponentIcon("Image Icon")]
    public class ImageAlphaApplier : ApplierBase<Image, float>
    {
        public override float Value => Target.color.a;
        public override void SetValue(float value) => Target.color = new Color(Target.color.r, Target.color.g, Target.color.b, value);
    }
}
