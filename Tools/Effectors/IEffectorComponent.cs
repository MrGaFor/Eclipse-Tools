using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [AddComponentMenu("")]
    public abstract class IEffectorComponent : MonoBehaviour
    {
        #region Data
        /* Example vars:
        public override bool ThisFloat => false;
        public override bool ThisVector3 => false;
        public override bool ThisColor => false;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<RectTransform, FuncList, float> _dataFloat; public virtual void FloatUpdate() { _dataVector3.Func = _dataFloat.Func; _dataColor.Func = _dataFloat.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("Vector3Update", IncludeChildren = true), ShowIf("ThisVector3")] private EffectorComponentFuncData<RectTransform, FuncList, Vector3> _dataVector3; public virtual void Vector3Update() { _dataFloat.Func = _dataVector3.Func; _dataColor.Func = _dataVector3.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<RectTransform, FuncList, Color> _dataColor;public virtual void ColorUpdate() { _dataFloat.Func = _dataColor.Func; _dataVector3.Func = _dataColor.Func; base.MarkDirty(); }

        public override EffectorEmpty Data => _data;
        private EffectorComponentFunc<RectTransform, FuncList> _data => ThisFloat ? _dataFloat : ThisVector3 ? _dataVector3 : ThisColor ? _dataColor : null;
        */
        public virtual EffectorEmpty Data { get; set; }
        public virtual bool ThisFloat => false;
        public virtual bool ThisColor => false;
        public virtual bool ThisVector3 => false;

        #region Compiling
        protected void MarkDirty()
        {
            IsCompiled = false;
        }

        protected bool IsCompiled = false;
        protected TweenSettings CompiledSettings;
        protected Tween EffectTween;

        protected void CompileSettings()
        {
            IsCompiled = true;
            CompiledSettings = new TweenSettings();
            CompiledSettings.startDelay = Data.Time.StartDelay;
            CompiledSettings.endDelay = Data.Time.EndDelay;
            CompiledSettings.duration = Data.Time.Duration;
            CompiledSettings.ease = Data.Curve.CurveType == EffectSettingsCurveModule.CurveTypes.Ease ? Data.Curve.Ease : Ease.Custom;
            if (Data.Curve.CurveType != EffectSettingsCurveModule.CurveTypes.Ease) CompiledSettings.customEase = Data.Curve.Curve;
            CompiledSettings.cycleMode = Data.Loop.LoopMode;
            CompiledSettings.cycles = Data.Loop.LoopType == EffectSettingsLoopModule.LoopTypes.None ? 0 : (Data.Loop.LoopType == EffectSettingsLoopModule.LoopTypes.Loop ? -1 : Data.Loop.LoopCount);
        }
        #endregion
        #endregion

        public virtual void PlayMoment() { }
        public virtual void PlayMomentCustom(float value) { }
        public virtual void PlayMomentCustom(Color value) { }
        public virtual void PlayMomentCustom(Vector3 value) { }
        public virtual void PlayMomentCustom(Gradient value) { }

        public virtual void PlaySmooth() { }
        public virtual void PlaySmoothCustom(float value) { }
        public virtual void PlaySmoothCustom(Color value) { }
        public virtual void PlaySmoothCustom(Vector3 value) { }
        public virtual void PlaySmoothCustom(Gradient value) { }

        public virtual void Stop() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
    }
}