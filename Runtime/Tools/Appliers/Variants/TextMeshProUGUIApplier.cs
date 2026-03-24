using UnityEngine;
using EC.Appliers.Core;
using TMPro;

namespace EC.Appliers.Variants
{
    [Applier(typeof(TextMeshProUGUI), typeof(string), "TextMeshProUGUI", "Text"), ComponentIcon("Text Icon")]
    public class TextMeshProUGUITextApplier : ApplierBase<TextMeshProUGUI, string>
    {
        public override string Value => Target.text;
        public override void SetValue(string value) => Target.text = value;
    }
    [Applier(typeof(TextMeshProUGUI), typeof(float), "TextMeshProUGUI", "FontSize"), ComponentIcon("Text Icon")]
    public class TextMeshProUGUIFontSizeApplier : ApplierBase<TextMeshProUGUI, float>
    {
        public override float Value => Target.fontSize;
        public override void SetValue(float value) => Target.fontSize = value;
    }
    [Applier(typeof(TextMeshProUGUI), typeof(float), "TextMeshProUGUI", "Alpha"), ComponentIcon("Text Icon")]
    public class TextMeshProUGUIAlphaApplier : ApplierBase<TextMeshProUGUI, float>
    {
        public override float Value => Target.color.a;
        public override void SetValue(float value) => Target.color = new Color(Target.color.r, Target.color.g, Target.color.b, value);
    }
    [Applier(typeof(TextMeshProUGUI), typeof(Color), "TextMeshProUGUI", "Color"), ComponentIcon("Text Icon")]
    public class TextMeshProUGUIColorApplier : ApplierBase<TextMeshProUGUI, Color>
    {
        public override Color Value => Target.color;
        public override void SetValue(Color value) => Target.color = value;
    }
}
