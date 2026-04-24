using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace EC.Effects
{
    [AddComponentMenu(""), ComponentIcon("Animation Icon")]
    public abstract class IEffectorComponent : MonoBehaviour
    {
        #region Data
        /* Example vars:
        public override bool ThisFloat => false;
        public override bool ThisString => false;
        public override bool ThisColor => false;
        public override bool ThisGradient => false;
        public override bool ThisVector2 => false;
        public override bool ThisVector3 => false;

        [field: SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<RectTransform, FuncList, float> _dataFloat; public virtual void FloatUpdate() { _dataVector3.Func = _dataFloat.Func; _dataColor.Func = _dataFloat.Func; base.MarkDirty(); }
        [field: SerializeField, HideLabel, OnValueChanged("Vector3Update", IncludeChildren = true), ShowIf("ThisVector3")] private EffectorComponentFuncData<RectTransform, FuncList, Vector3> _dataVector3; public virtual void Vector3Update() { _dataFloat.Func = _dataVector3.Func; _dataColor.Func = _dataVector3.Func; base.MarkDirty(); }
        [field: SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<RectTransform, FuncList, Color> _dataColor;public virtual void ColorUpdate() { _dataFloat.Func = _dataColor.Func; _dataVector3.Func = _dataColor.Func; base.MarkDirty(); }

        public override IEffectorData Data => _data;
        private EffectorComponentFunc<RectTransform, FuncList> _data => ThisFloat ? _dataFloat : ThisVector3 ? _dataVector3 : ThisColor ? _dataColor : null;
        */
        public virtual IEffectorData Data { get; set; }
        public virtual bool ThisInt => false;
        public virtual bool ThisFloat => false;
        public virtual bool ThisString => false;
        public virtual bool ThisColor => false;
        public virtual bool ThisGradient => false;
        public virtual bool ThisVector2 => false;
        public virtual bool ThisVector3 => false;
        public virtual bool ThisVector4 => false;

        public virtual bool TryGetModule<T>(out T data)
        {
            if (Data is T tdata)
            {
                data = tdata;
                return true;
            }
            data = default;
            return false;
        }
        public virtual IEffectorTimeModuleData GetTime() => Data as IEffectorTimeModuleData;

        #region Compiling
        protected void MarkDirty()
        {
            IsCompiled = false;
        }

        protected bool IsCompiled = false;
        protected TweenSettings CompiledSettings;
        protected Tween EffectTween;

        protected virtual void CompileSettings()
        {
            IsCompiled = true;
            TryGetModule<IEffectorTimeModuleData>(out var timeModule);
            TryGetModule<IEffectorCurveModuleData>(out var curveModule);
            TryGetModule<IEffectorLoopModuleData>(out var loopModule);
            CompiledSettings = new TweenSettings()
            {
                startDelay = timeModule.TimeModule.StartDelay,
                endDelay = timeModule.TimeModule.EndDelay,
                duration = timeModule.TimeModule.Duration,
                ease = curveModule.CurveModule.CurveType == EffectSettingsCurveModule.CurveTypes.Ease ? curveModule.CurveModule.Ease : Ease.Custom,
                cycleMode = loopModule.LoopModule.LoopMode,
                cycles = loopModule.LoopModule.LoopType == EffectSettingsLoopModule.LoopTypes.None ? 0 : (loopModule.LoopModule.LoopType == EffectSettingsLoopModule.LoopTypes.Loop ? -1 : loopModule.LoopModule.LoopCount)
            };
            if (curveModule.CurveModule.CurveType != EffectSettingsCurveModule.CurveTypes.Ease) CompiledSettings.customEase = curveModule.CurveModule.Curve;
        }
        #endregion
        #endregion

        public virtual void PlayMoment() { }
        public virtual void PlayMomentCustom(int value) { }
        public virtual void PlayMomentCustom(float value) { }
        public virtual void PlayMomentCustom(string value) { }
        public virtual void PlayMomentCustom(Color value) { }
        public virtual void PlayMomentCustom(Gradient value) { }
        public virtual void PlayMomentCustom(Vector2 value) { }
        public virtual void PlayMomentCustom(Vector3 value) { }
        public virtual void PlayMomentCustom(Vector4 value) { }

        public virtual void PlaySmooth() { }
        public virtual void PlaySmoothCustom(int value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(int value, float duration) { }
        public virtual void PlaySmoothCustom(float value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(float value, float duration) { }
        public virtual void PlaySmoothCustom(string value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(string value, float duration) { }
        public virtual void PlaySmoothCustom(Color value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(Color value, float duration) { }
        public virtual void PlaySmoothCustom(Gradient value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(Gradient value, float duration) { }
        public virtual void PlaySmoothCustom(Vector2 value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(Vector2 value, float duration) { }
        public virtual void PlaySmoothCustom(Vector3 value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(Vector3 value, float duration) { }
        public virtual void PlaySmoothCustom(Vector4 value) { PlaySmoothCustom(value, GetTime().AllDuration); }
        public virtual void PlaySmoothCustom(Vector4 value, float duration) { }

#pragma warning disable CS1998
        public virtual async UniTask PlaySmoothAsync() { }
        public virtual async UniTask PlaySmoothCustomAsync(int value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(int value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(float value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(float value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(string value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(string value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(Color value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(Color value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(Gradient value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(Gradient value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(Vector2 value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(Vector2 value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(Vector3 value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(Vector3 value, float duration) { }
        public virtual async UniTask PlaySmoothCustomAsync(Vector4 value) { await PlaySmoothCustomAsync(value, GetTime().AllDuration); }
        public virtual async UniTask PlaySmoothCustomAsync(Vector4 value, float duration) { }
#pragma warning restore CS1998

        public virtual void Stop() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
    }
}