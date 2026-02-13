using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorCanvasGroup: IEffectorComponent
    {
        #region Data
        private enum FuncList { Alpha }
        public override bool ThisFloat => true;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<CanvasGroup, FuncList, float> _dataFloat; public virtual void FloatUpdate() { base.MarkDirty(); }
        
        public override EffectorEmpty Data => _data; private EffectorComponentFunc<CanvasGroup, FuncList> _data => ThisFloat ? _dataFloat : null;
        public CanvasGroup Component => _data?.Component;
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
                case FuncList.Alpha:
                    _data.Component.alpha = value; break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override void PlaySmooth()
        {
            PlaySmoothCustom(_dataFloat.Value);
        }
        public override void PlaySmoothCustom(float value, float duration)
        {
            SmoothFloatPart(value, duration);
        }

        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_dataFloat.Value);
        }
        public override async UniTask PlaySmoothCustomAsync(float value, float duration)
        {
            if (!gameObject.activeSelf)
                PlayMomentCustom(value);
            else if (SmoothFloatPart(value, duration)) await EffectTween;
        }

        private bool SmoothFloatPart(float value, float duration)
        {
            if (!ThisFloat) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Alpha:
                    EffectTween = Tween.Alpha(_data.Component, value, CompiledSettings); break;
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