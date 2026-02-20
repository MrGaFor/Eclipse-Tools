#if EC_THEMES
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Themes
{
    [System.Serializable]
    public class ThemeColorVariant
    {
        public string ColorId { get; set; }
        [LabelText("@ColorId")] public Color Color;
    }
}
#endif