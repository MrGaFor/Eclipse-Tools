using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorDelay : IEffectorComponent
    {
        #region Data
        [SerializeField, BoxGroup("Data", ShowLabel = false), FoldoutGroup("Data/Settings"), HideLabel] private EffectorDelayData _data;
        private new EffectorDelayData Data => _data;

        [System.Serializable]
        private class EffectorDelayData
        {
            [HideLabel, LabelWidth(70), MinValue(0)] public float Time;
            [HideLabel] public EffectSettingsStopModule Stop;
            [HideLabel] public EffectLifeEventModule Events;
        }

        protected override void CompileSettings()
        {
            IsCompiled = true;
            CompiledSettings = new TweenSettings();
            CompiledSettings.duration = Data.Time;
            CompiledSettings.ease = Ease.Linear;
         }
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
            StartPlayMoment();
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_data.Time);
        }
        public override async UniTask PlaySmoothCustomAsync(float value)
        {
            StartPlaySmooth();
            float defaultDuration = CompiledSettings.duration;
            CompiledSettings.duration = value;
            EffectTween = Tween.Custom(0f, 1f, CompiledSettings, data => { });
            CompiledSettings.duration = defaultDuration;
            await EffectTween;
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