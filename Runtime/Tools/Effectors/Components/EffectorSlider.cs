using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorSlider : IEffectorComponent
    {
        #region Data
        private enum FuncList { Value }
        public override bool ThisFloat => true;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<Slider, FuncList, float> _dataFloat; public virtual void FloatUpdate() { base.MarkDirty(); }

        public override EffectorEmpty Data => _data; private EffectorComponentFunc<Slider, FuncList> _data => ThisFloat ? _dataFloat : null;
        public Slider Component => _data?.Component;
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
        }
        public override void PlayMomentCustom(float value)
        {
            if (!ThisFloat) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Value:
                    _data.Component.value = value; break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override void PlaySmooth()
        {
            PlaySmoothCustom(_dataFloat.Value);
        }
        public override void PlaySmoothCustom(float value)
        {
            if (!ThisFloat) return;
            StartPlaySmooth();
            switch (_data.Func)
            {
                case FuncList.Value:
                    EffectTween = Tween.UISliderValue(_data.Component, value, CompiledSettings); break;
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