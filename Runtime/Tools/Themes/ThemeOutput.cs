#if EC_THEMES
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EC.Themes
{
    public class ThemeOutput : MonoBehaviour
    {
        private string[] ColorIds => ThemeSettingsProvider.Settings.ColorIds;
        private ThemeManager Manager => Services.GameLocator.TryGet<ThemeManager>(out var manager) ? manager : null;

        [SerializeField, LabelWidth(70), ValueDropdown(nameof(ColorIds)), BoxGroup("color", false), HorizontalGroup("color/c")] private string _colorId;
        [SerializeField, LabelWidth(70), BoxGroup("color"), HorizontalGroup("color/c", 85)] private bool _useAlpha = true;
        private enum CountVariant { One, Many }
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnChangeCount))]
#endif
        [SerializeField, LabelWidth(70), HorizontalGroup("target/one", MinWidth = 140, Width = 0.3f)] private CountVariant _count = CountVariant.One;

        [SerializeField, LabelWidth(70), ShowIf("@_count", CountVariant.One), HorizontalGroup("target/one")] private Appliers.ApplierFilter<Color> _target;
        [SerializeField, LabelWidth(70), ShowIf("@_count", CountVariant.Many), BoxGroup("target", false), ListDrawerSettings(DraggableItems = false, ShowFoldout = false, DefaultExpandedState = true)] private Appliers.ApplierFilter<Color>[] _targets;

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
            if (_count == CountVariant.One)
            {
                if (_target == null) return;
                if (!_useAlpha && _target.HasGetter)
                    color.a = ((Color)_target.GetValue()).a;
                if (_target.HasSetter)
                    _target.SetValue(color);
            }
            else
            {
                if (_targets == null) return;
                float a = color.a;
                foreach (var target in _targets)
                {
                    if (!_useAlpha && target.HasGetter)
                        color.a = ((Color)target.GetValue()).a;
                    else if (!_useAlpha)
                        color.a = a;
                    if (target.HasSetter)
                        target.SetValue(color);
                }
            }
        }

        #region Editor
#if UNITY_EDITOR
        private void OnChangeCount()
        {
            if (_count == CountVariant.One && _targets != null)
                _targets = null;
            else if (_count == CountVariant.Many && _target != null)
                _target = null;
        }
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
        private bool HasSetter => _count == CountVariant.One ? (_target != null && _target.HasSetter)
            : (_targets != null && _targets.Any(t => t.HasSetter));
        [ShowInInspector, Button("Apply"), BoxGroup("color"), HorizontalGroup("color/c", 60), PropertySpace(1.9f), ShowIf(nameof(HasSetter)), PropertyOrder(1)]
        private void ApplySet()
        {
            if (Application.isPlaying)
                OnChangedTheme();
            else
            {
                SetColor(ThemeManager.GetColorEditor(_colorId));
                AssetDatabase.SaveAssets();
            }
        }
#endif
        #endregion
    }
}
#endif