#if EC_THEMES
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Themes
{
    [System.Serializable]
    public class ThemeColorVariant
    {
        [SerializeField, HideInInspector] public string ColorId;
        [SerializeField, LabelText("@ColorId")] public Color Color;
    }
}
#endif