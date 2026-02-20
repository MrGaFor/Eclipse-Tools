#if EC_THEMES
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EC.Themes
{
    public class ThemeOutput : MonoBehaviour
    {
        private enum SourceType { Image, SpriteRenderer, Material, Event, Effector }

        [SerializeField, ValueDropdown("ColorIds")] private string _colorId;
        [SerializeField] private SourceType _source;
        [SerializeField, ShowIf("_source", SourceType.Image)] private Image _image;
        [SerializeField, ShowIf("_source", SourceType.SpriteRenderer)] private SpriteRenderer _spriteRenderer;
        [SerializeField, ShowIf("_source", SourceType.Material)] private string _materialColorKey;
        [SerializeField, ShowIf("_source", SourceType.Material)] private Material _material;
        [SerializeField, ShowIf("_source", SourceType.Event)] private UnityEvent<Color> _event;
        [SerializeField, ShowIf("_source", SourceType.Effector)] private Effects.IEffectorComponent _effector;

        private string[] ColorIds => ThemeSettingsProvider.Settings.ColorIds;
        private ThemeManager Manager => Services.GameLocator.TryGet<ThemeManager>(out var manager) ? manager : null;

        private void OnEnable()
        {
            Manager.OnChangeTheme += OnChangedTheme;
            OnChangedTheme();
        }
        private void OnDisable()
        {
            Manager.OnChangeTheme -= OnChangedTheme;
        }

        private void OnChangedTheme()
        {
            SetColor(Manager.GetColor(_colorId));
        }
        private void SetColor(Color color)
        {
            switch (_source)
            {
                case SourceType.Image:
                    if (Application.isPlaying || _image)
                        _image.color = color;
                    break;
                case SourceType.SpriteRenderer:
                    if (Application.isPlaying || _spriteRenderer)
                        _spriteRenderer.color = color; 
                    break;
                case SourceType.Material:
                    if (Application.isPlaying || _material)
                        _material.SetColor(_materialColorKey, color); 
                    break;
                case SourceType.Event:
                    _event?.Invoke(color); 
                    break;
                case SourceType.Effector:
                    if (Application.isPlaying || _effector)
                        _effector.PlaySmoothCustom(color); 
                    break;
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (Application.isPlaying)
                OnChangedTheme();
            else
            {
                if (string.IsNullOrEmpty(_colorId))
                    _colorId = ThemeSettingsProvider.Settings.ColorIds[0];
                SetColor(ThemeManager.GetColorEditor(_colorId));
            }
        }
#endif

    }
}
#endif