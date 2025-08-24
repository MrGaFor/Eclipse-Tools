using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorCamera : IEffectorComponent
    {
        #region Data
        private enum FuncList { FieldOfView, OrthographicSize }
        public override bool ThisFloat => true;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<Camera, FuncList, float> _dataFloat; public virtual void FloatUpdate() { base.MarkDirty(); }
        
        public override EffectorEmpty Data => _data; private EffectorComponentFunc<Camera, FuncList> _data => ThisFloat ? _dataFloat : null;
        public Camera Component => _data?.Component;
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
                case FuncList.FieldOfView:
                    _data.Component.fieldOfView = value; break;
                case FuncList.OrthographicSize:
                    _data.Component.orthographicSize = value; break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_dataFloat.Value);
        }
        public override async UniTask PlaySmoothCustomAsync(float value, float duration)
        {
            if (!ThisFloat) return;
            StartPlaySmooth();
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.FieldOfView:
                    EffectTween = Tween.CameraFieldOfView(_data.Component, value, CompiledSettings); break;
                case FuncList.OrthographicSize:
                    EffectTween = Tween.CameraOrthographicSize(_data.Component, value, CompiledSettings); break;
                default:
                    used = false; break;
            }
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
            if (used) await EffectTween;
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