using UnityEngine;
using EC.Appliers.Core;
using TMPro;

namespace EC.Appliers.Variants
{
    [Applier(typeof(TextMeshPro), typeof(string), "TextMeshPro", "Text"), ComponentIcon("Text Icon")]
    public class TextMeshProTextApplier : ApplierBase<TextMeshPro, string>
    {
        public override string Value => Target.text;
        public override void SetValue(string value) => Target.text = value;
    }
    [Applier(typeof(TextMeshPro), typeof(float), "TextMeshPro", "FontSize"), ComponentIcon("Text Icon")]
    public class TextMeshProFontSizeApplier : ApplierBase<TextMeshPro, float>
    {
        public override float Value => Target.fontSize;
        public override void SetValue(float value) => Target.fontSize = value;
    }
    [Applier(typeof(TextMeshPro), typeof(float), "TextMeshPro", "Alpha"), ComponentIcon("Text Icon")]
    public class TextMeshProAlphaApplier : ApplierBase<TextMeshPro, float>
    {
        public override float Value => Target.color.a;
        public override void SetValue(float value) => Target.color = new Color(Target.color.r, Target.color.g, Target.color.b, value);
    }
    [Applier(typeof(TextMeshPro), typeof(Color), "TextMeshPro", "Color"), ComponentIcon("Text Icon")]
    public class TextMeshProColorApplier : ApplierBase<TextMeshPro, Color>
    {
        public override Color Value => Target.color;
        public override void SetValue(Color value) => Target.color = value;
    }
}
