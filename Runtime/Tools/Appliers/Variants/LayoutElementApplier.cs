using EC.Appliers.Core;
using UnityEngine.UI;

namespace EC.Appliers.Variants
{
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "MinWidth"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementMinWidthApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.minWidth;
        public override void SetValue(float value) => Target.minWidth = value;
    }
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "PreferredWidth"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementPreferredWidthApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.preferredWidth;
        public override void SetValue(float value) => Target.preferredWidth = value;
    }
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "FlexibleWidth"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementFlexibleWidthApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.flexibleWidth;
        public override void SetValue(float value) => Target.flexibleWidth = value;
    }
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "MinHeight"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementMinHeightApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.minHeight;
        public override void SetValue(float value) => Target.minHeight = value;
    }
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "PreferredHeight"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementPreferredHeightApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.preferredHeight;
        public override void SetValue(float value) => Target.preferredHeight = value;
    }
    [Applier(typeof(LayoutElement), typeof(float), "LayoutElement", "FlexibleHeight"), ComponentIcon("RectTransform Icon")]
    public class LayoutElementFlexibleHeightApplier : ApplierBase<LayoutElement, float>
    {
        public override float Value => Target.flexibleHeight;
        public override void SetValue(float value) => Target.flexibleHeight = value;
    }
}
