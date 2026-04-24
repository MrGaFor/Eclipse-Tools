using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace EC.ComponentVariable
{
    // IMAGE
    [Serializable]
    public class ImageColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private UnityEngine.UI.Image image;

        public override bool HasSetter => image != null;
        public override bool HasGetter => image != null;

        public override void SetValue(Color value)
        {
            if (image != null)
                image.color = value;
        }

        public override Color GetValue()
        {
            return image != null ? image.color : Color.clear;
        }
    }
    // SPRITE RENDERER
    [Serializable]
    public class SpriteRendererColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private SpriteRenderer spriteRenderer;

        public override bool HasSetter => spriteRenderer != null;
        public override bool HasGetter => spriteRenderer != null;

        public override void SetValue(Color value)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = value;
        }

        public override Color GetValue()
        {
            return spriteRenderer != null ? spriteRenderer.color : Color.clear;
        }
    }
    // TEXTMESHPRO
    [Serializable]
    public class TextMeshProColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private TMPro.TextMeshPro textMeshPro;

        public override bool HasSetter => textMeshPro != null;
        public override bool HasGetter => textMeshPro != null;

        public override void SetValue(Color value)
        {
            if (textMeshPro != null)
                textMeshPro.color = value;
        }

        public override Color GetValue()
        {
            return textMeshPro != null ? textMeshPro.color : Color.clear;
        }
    }
    // TEXTMESHPROUGUI
    [Serializable]
    public class TextMeshProUGUIColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private TMPro.TextMeshProUGUI textMeshProUGUI;

        public override bool HasSetter => textMeshProUGUI != null;
        public override bool HasGetter => textMeshProUGUI != null;

        public override void SetValue(Color value)
        {
            if (textMeshProUGUI != null)
                textMeshProUGUI.color = value;
        }

        public override Color GetValue()
        {
            return textMeshProUGUI != null ? textMeshProUGUI.color : Color.clear;
        }
    }
    // EVENT
    [Serializable]
    public class EventColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private UnityEvent<Color> onEvent;

        public override bool HasSetter => true;
        public override bool HasGetter => false;

        public override void SetValue(Color value)
        {
            onEvent?.Invoke(value);
        }

        public override Color GetValue()
        {
            return Color.clear;
        }
    }
    // MATERIAL
    [Serializable]
    public class MaterialColorTarget : VariableTargetBase<Color>
    {
        [SerializeField, HideLabel]
        private Material material;
#if UNITY_EDITOR
        [ValueDropdown("GetColorPropertyNames")]
#endif
        [SerializeField, HideLabel, ShowIf("material")]
        private string materialKey;

#if UNITY_EDITOR
        [Obsolete]
        private string[] GetColorPropertyNames()
        {
            if (material == null || material.shader == null)
                return System.Array.Empty<string>();

            var shader = material.shader;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);

            var result = new List<string>();

            for (int i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Color)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, i);
                    result.Add(propertyName);
                }
            }

            return result.ToArray();
        }
#endif

        public override bool HasSetter => material != null;
        public override bool HasGetter => material != null;

        public override void SetValue(Color value)
        {
            if (material != null)
                material.SetColor(materialKey, value);
        }

        public override Color GetValue()
        {
            return material != null ? material.GetColor(materialKey) : Color.clear;
        }
    }
    // EFFECTOR
    [Serializable]
    public class EffectorColorTarget : VariableTargetBase<Color>
    {
        private enum EffectorVariant { Moment, Smooth }

        [SerializeField, HideLabel, HorizontalGroup("effector")]
        private Effects.IEffectorComponent effector;
        [SerializeField, HideLabel, HorizontalGroup("effector", 100)]
        private EffectorVariant type;

        public override bool HasSetter => effector != null;
        public override bool HasGetter => false;

        public override void SetValue(Color value)
        {
            if (effector != null)
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    effector.PlayMomentCustom(value);
                else
#endif
                    switch (type)
                    {
                        case EffectorVariant.Moment:
                            effector.PlayMomentCustom(value); break;
                        case EffectorVariant.Smooth:
                            effector.PlaySmoothCustom(value); break;
                    }
        }

        public override Color GetValue()
        {
            return Color.clear;
        }
    }
}