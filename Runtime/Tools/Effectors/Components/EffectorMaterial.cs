using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorMaterial: IEffectorComponent
    {
        #region Data
        private enum FuncList { Int, Float, Range, Color, Vector4 }
        private bool IsFunc(FuncList type) => _dataInt.Func == type;
        public override bool ThisInt => IsFunc(FuncList.Int);
        public override bool ThisFloat => IsFunc(FuncList.Float);
        public bool ThisRange => IsFunc(FuncList.Range);
        public bool ThisFloatOrRange => ThisFloat || ThisRange;
        public override bool ThisColor => IsFunc(FuncList.Color);
        public override bool ThisVector4 => IsFunc(FuncList.Vector4);

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private float RangeMin;
        [SerializeField, HideInInspector] private float RangeMax;

        private string[] GetKeysForActiveData()
        {
            if (_data == null || _data.Component == null)
                return new string[] { "None" };

            var mat = _data.Component;
            var shader = mat.shader;
            int count = UnityEditor.ShaderUtil.GetPropertyCount(shader);
            System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>();
            for (int i = 0; i < count; i++)
            {
                var type = UnityEditor.ShaderUtil.GetPropertyType(shader, i);
                if (type == _data.Func switch
                {
                    FuncList.Int => UnityEditor.ShaderUtil.ShaderPropertyType.Int,
                    FuncList.Float => UnityEditor.ShaderUtil.ShaderPropertyType.Float,
                    FuncList.Range => UnityEditor.ShaderUtil.ShaderPropertyType.Range,
                    FuncList.Color => UnityEditor.ShaderUtil.ShaderPropertyType.Color,
                    FuncList.Vector4 => UnityEditor.ShaderUtil.ShaderPropertyType.Vector,
                    _ => throw new ArgumentOutOfRangeException()
                })
                    keys.Add(UnityEditor.ShaderUtil.GetPropertyName(shader, i));
            }
            return keys.ToArray();
        }
        [ValueDropdown("GetKeysForActiveData")]
#endif
        [SerializeField, BoxGroup("Data", false), HorizontalGroup("Data/value")] private string _key;
        [SerializeField, HideLabel, OnValueChanged("IntUpdate", IncludeChildren = true), ShowIf("ThisInt")] private EffectorComponentFuncData<Material, FuncList, int> _dataInt; public virtual void IntUpdate() { UpdateFuncExclude(_dataInt); base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloatOrRange")] private EffectorComponentFuncData<Material, FuncList, float> _dataFloat; public virtual void FloatUpdate() { UpdateFuncExclude(_dataFloat); base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<Material, FuncList, Color> _dataColor; public virtual void ColorUpdate() { UpdateFuncExclude(_dataColor); base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("Vector4Update", IncludeChildren = true), ShowIf("ThisVector4")] private EffectorComponentFuncData<Material, FuncList, Vector4> _dataVector4; public virtual void Vector4Update() { UpdateFuncExclude(_dataVector4); base.MarkDirty(); }

        private void UpdateFuncExclude(EffectorComponentFunc<Material, FuncList> excl)
        {
            if (excl != _dataInt) _dataInt.Func = excl.Func;
            if (excl != _dataFloat) _dataFloat.Func = excl.Func;
            if (excl != _dataColor) _dataColor.Func = excl.Func;
            if (excl != _dataVector4) _dataVector4.Func = excl.Func;
        }

        public override EffectorEmpty Data => _data; private EffectorComponentFunc<Material, FuncList> _data => _dataInt.Func switch
        {
            FuncList.Int => _dataInt,
            FuncList.Float => _dataFloat,
            FuncList.Range => _dataFloat,
            FuncList.Color => _dataColor,
            FuncList.Vector4 => _dataVector4,
            _ => throw new ArgumentOutOfRangeException()
        };

        public Material Component => _data?.Component;
        #endregion

        #region Start|End Player
        private void StartPlayMoment()
        {
            Stop();
            if (!IsCompiled)
                CompileSettings();
            _data.Events.CallPlayEvent();
        }
        private void EndPlayMoment()
        {
            _data.Events.CallUpdateEvent();
            _data.Events.CallCompleteEvent();
        }
        private void StartPlaySmooth()
        {
            Stop();
            if (!IsCompiled)
                CompileSettings();
            _data.Events.CallPlayEvent();
        }
        private void EndPlaySmooth()
        {
            EffectTween.OnUpdate(target: this, (value, tween) => value._data.Events.CallUpdateEvent());
            EffectTween.OnComplete(target: this, value => value._data.Events.CallCompleteEvent());
        }
        #endregion

        #region Moment Player
        public override void PlayMoment()
        {
            PlayMomentCustom(_dataInt.Value);
            PlayMomentCustom(_dataFloat.Value);
            PlayMomentCustom(_dataColor.Value);
            PlayMomentCustom(_dataVector4.Value);
        }
        public override void PlayMomentCustom(int value)
        {
            if (!ThisInt) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Int:
                    _data.Component.SetInt(_key, value); break;
            }
            EndPlayMoment();
        }
        public override void PlayMomentCustom(float value)
        {
            if (!ThisFloat && !ThisRange) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Float:
                    _data.Component.SetFloat(_key, value); break;
            }
            EndPlayMoment();
        }
        public override void PlayMomentCustom(Color value)
        {
            if (!ThisColor) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Color:
                    _data.Component.SetColor(_key, value); break;
            }
            EndPlayMoment();
        }
        public override void PlayMomentCustom(Vector4 value)
        {
            if (!ThisVector4) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Vector4:
                    _data.Component.SetVector(_key, value); break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override void PlaySmooth()
        {
            PlaySmoothCustom(_dataInt.Value);
            PlaySmoothCustom(_dataFloat.Value);
            PlaySmoothCustom(_dataColor.Value);
            PlaySmoothCustom(_dataVector4.Value);
        }
        public override void PlaySmoothCustom(int value, float duration)
        {
            SmoothIntPart(value, duration);
        }
        public override void PlaySmoothCustom(float value, float duration)
        {
            SmoothFloatPart(value, duration);
        }
        public override void PlaySmoothCustom(Color value, float duration)
        {
            SmoothColorPart(value, duration);
        }
        public override void PlaySmoothCustom(Vector4 value, float duration)
        {
            SmoothVector4Part(value, duration);
        }

        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_dataInt.Value);
            await PlaySmoothCustomAsync(_dataFloat.Value);
            await PlaySmoothCustomAsync(_dataColor.Value);
            await PlaySmoothCustomAsync(_dataVector4.Value);
        }
        public override async UniTask PlaySmoothCustomAsync(int value, float duration)
        {
            if (SmoothIntPart(value, duration)) await EffectTween;
        }
        public override async UniTask PlaySmoothCustomAsync(float value, float duration)
        {
            if (SmoothFloatPart(value, duration)) await EffectTween;
        }
        public override async UniTask PlaySmoothCustomAsync(Color value, float duration)
        {
            if (SmoothColorPart(value, duration)) await EffectTween;
        }
        public override async UniTask PlaySmoothCustomAsync(Vector4 value, float duration)
        {
            if (SmoothVector4Part(value, duration)) await EffectTween;
        }

        private bool SmoothIntPart(int value, float duration)
        {
            if (!ThisInt) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Int:
                    EffectTween = Tween.Custom(_data.Component.GetInt(_key), value, CompiledSettings, value => { _data.Component.SetInt(_key, (int)value); }); break;
                default:
                    used = false; break;
            }
            EndPlaySmooth();
            if (buffDuration != CompiledSettings.duration) CompiledSettings.duration = buffDuration;
            return used;
        }
        private bool SmoothFloatPart(float value, float duration)
        {
            if (!ThisFloat && !ThisRange) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Float or FuncList.Range:
                    EffectTween = Tween.Custom(_data.Component.GetFloat(_key), value, CompiledSettings, value => { _data.Component.SetFloat(_key, value); }); break;
                default:
                    used = false; break;
            }
            EndPlaySmooth();
            if (buffDuration != CompiledSettings.duration) CompiledSettings.duration = buffDuration;
            return used;
        }
        private bool SmoothColorPart(Color value, float duration)
        {
            if (!ThisColor) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Color:
                    EffectTween = Tween.Custom(_data.Component.GetColor(_key), value, CompiledSettings, value => { _data.Component.SetColor(_key, value); }); break;
                default:
                    used = false; break;
            }
            EndPlaySmooth();
            if (buffDuration != CompiledSettings.duration) CompiledSettings.duration = buffDuration;
            return used;
        }
        private bool SmoothVector4Part(Vector4 value, float duration)
        {
            if (!ThisVector4) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Vector4:
                    EffectTween = Tween.Custom((Vector4)_data.Component.GetVector(_key), value, CompiledSettings, value => { _data.Component.SetVector(_key, (Vector4)value); }); break;
                default:
                    used = false; break;
            }
            EndPlaySmooth();
            if (buffDuration != CompiledSettings.duration) CompiledSettings.duration = buffDuration;
            return used;
        }
        #endregion

        #region Stop|Pause|Resume
        public override void Stop()
        {
            if (EffectTween.isAlive)
                if (_data.Stop.StopType == EffectSettingsStopModule.KillAction.Stop)
                    EffectTween.Stop();
                else if (_data.Stop.StopType == EffectSettingsStopModule.KillAction.Complete)
                    EffectTween.Complete();
        }
        public override void Pause()
        {
            if (EffectTween.isAlive && !EffectTween.isPaused)
                EffectTween.isPaused = true;
        }
        public override void Resume()
        {
            if (EffectTween.isAlive && EffectTween.isPaused)
                EffectTween.isPaused = false;
        }
        #endregion
    }
}