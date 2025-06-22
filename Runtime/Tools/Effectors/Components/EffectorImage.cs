using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorImage : IEffectorComponent
    {
        #region Data
        private enum FuncList { Alpha, Color, FillAmount }
        public override bool ThisFloat => _dataFloat.Func == FuncList.Alpha || _dataFloat.Func == FuncList.FillAmount;
        public override bool ThisColor => _dataColor.Func == FuncList.Color;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<UnityEngine.UI.Image, FuncList, float> _dataFloat; public virtual void FloatUpdate() { _dataColor.Func = _dataFloat.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<UnityEngine.UI.Image, FuncList, Color> _dataColor;public virtual void ColorUpdate() { _dataFloat.Func = _dataColor.Func; base.MarkDirty(); }

        public override EffectorEmpty Data => _data; private EffectorComponentFunc<UnityEngine.UI.Image, FuncList> _data => ThisFloat ? _dataFloat : ThisColor ? _dataColor : null;
        public UnityEngine.UI.Image Component => _data?.Component;
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
            PlayMomentCustom(_dataFloat.Value);
            PlayMomentCustom(_dataColor.Value);
        }
        public override void PlayMomentCustom(float value)
        {
            if (!ThisFloat) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Alpha:
                    _data.Component.color = new Color(_data.Component.color.r, _data.Component.color.g, _data.Component.color.b, _dataFloat.Value); break;
                case FuncList.FillAmount:
                    _data.Component.fillAmount = value; break;
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
                    _data.Component.color = value; break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override void PlaySmooth()
        {
            PlaySmoothCustom(_dataFloat.Value);
            PlaySmoothCustom(_dataColor.Value);
        }
        public override void PlaySmoothCustom(float value)
        {
            if (!ThisFloat) return;
            StartPlaySmooth();
            switch (_data.Func)
            {
                case FuncList.Alpha:
                    EffectTween = Tween.Alpha(_data.Component, value, CompiledSettings); break;
                case FuncList.FillAmount:
                    EffectTween = Tween.UIFillAmount(_data.Component, value, CompiledSettings); break;
            }
            EndPlaySmooth();
        }
        public override void PlaySmoothCustom(Color value)
        {
            if (!ThisColor) return;
            StartPlaySmooth();
            switch (_data.Func)
            {
                case FuncList.Color:
                    EffectTween = Tween.Color(_data.Component, value, CompiledSettings); break;
            }
            EndPlaySmooth();
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