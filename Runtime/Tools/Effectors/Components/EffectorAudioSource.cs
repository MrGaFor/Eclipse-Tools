using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorAudioSource : IEffectorComponent
    {
        #region Data
        private enum FuncList { Volume, Pitch }
        public override bool ThisFloat => true;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<AudioSource, FuncList, float> _dataFloat; public virtual void FloatUpdate() { base.MarkDirty(); }
        
        public override EffectorEmpty Data => _data; private EffectorComponentFunc<AudioSource, FuncList> _data => ThisFloat ? _dataFloat : null;
        public AudioSource Component => _data?.Component;
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
                case FuncList.Volume:
                    _data.Component.volume = value; break;
                case FuncList.Pitch:
                    _data.Component.pitch = value; break;
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
                case FuncList.Volume:
                    EffectTween = Tween.AudioVolume(_data.Component, value, CompiledSettings); break;
                case FuncList.Pitch:
                    EffectTween = Tween.AudioPitch(_data.Component, value, CompiledSettings); break;
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