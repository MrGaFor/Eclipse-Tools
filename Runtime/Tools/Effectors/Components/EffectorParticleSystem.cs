using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorParticleSystem : IEffectorComponent
    {
        #region Data
        private enum FuncList { Color, StartLifetime, StartSpeed, StartSize, EmissionRate }
        public override bool ThisFloat => _dataFloat.Func == FuncList.StartLifetime || _dataFloat.Func == FuncList.StartSpeed || _dataFloat.Func == FuncList.StartSize || _dataFloat.Func == FuncList.EmissionRate;
        public override bool ThisColor => _dataColor.Func == FuncList.Color;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<ParticleSystem, FuncList, float> _dataFloat; public virtual void FloatUpdate() { _dataColor.Func = _dataFloat.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<ParticleSystem, FuncList, Color> _dataColor; public virtual void ColorUpdate() { _dataFloat.Func = _dataColor.Func; base.MarkDirty(); }

        public override EffectorEmpty Data => _data; private EffectorComponentFunc<ParticleSystem, FuncList> _data => ThisFloat ? _dataFloat : ThisColor ? _dataColor : null;
        public ParticleSystem Component => _data?.Component;
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
            var main = _data.Component.main;
            var emiss = _data.Component.emission;
            switch (_data.Func)
            {
                case FuncList.StartLifetime:
                    main.startLifetime = value; break;
                case FuncList.StartSpeed:
                    main.startSpeed = value; break;
                case FuncList.StartSize:
                    main.startSize = value; break;
                case FuncList.EmissionRate:
                    emiss.rateOverTime = value; break;
            }
            EndPlayMoment();
        }
        public override void PlayMomentCustom(Color value)
        {
            if (!ThisColor) return;
            StartPlayMoment();
            var main = _data.Component.main;
            var emiss = _data.Component.emission;
            switch (_data.Func)
            {
                case FuncList.Color:
                    SetColor(_data.Component, value); break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_dataFloat.Value);
            await PlaySmoothCustomAsync(_dataColor.Value);
        }
        public override async UniTask PlaySmoothCustomAsync(float value, float duration)
        {
            if (!ThisFloat) return;
            StartPlaySmooth();
            var main = _data.Component.main;
            var emiss = _data.Component.emission;
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.StartLifetime:
                    EffectTween = Tween.Custom(main.startLifetime.constant, value, CompiledSettings, newvalue => main.startLifetime = newvalue); break;
                case FuncList.StartSpeed:
                    EffectTween = Tween.Custom(main.startSpeed.constant, value, CompiledSettings, newvalue => main.startSpeed = newvalue); break;
                case FuncList.StartSize:
                    EffectTween = Tween.Custom(main.startSize.constant, value, CompiledSettings, newvalue => main.startSize = newvalue); break;
                case FuncList.EmissionRate:
                    EffectTween = Tween.Custom(emiss.rateOverTime.constant, value, CompiledSettings, newvalue => emiss.rateOverTime = newvalue); break;
                default:
                    used = false; break;
            }
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
            if (used) await EffectTween;
            EndPlaySmooth();
        }
        public override async UniTask PlaySmoothCustomAsync(Color value, float duration)
        {
            if (!ThisColor) return;
            StartPlaySmooth();
            var main = _data.Component.main;
            var emiss = _data.Component.emission;
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Color:
                    EffectTween = Tween.Custom(main.startColor.color, value, CompiledSettings, newvalue => SetColor(_data.Component, newvalue)); break;
                default:
                    used = false; break;
            }
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
            if (used) await EffectTween;
            EndPlaySmooth();
        
        }
        private void SetColor(ParticleSystem ps, Color color)
        {
            var main = ps.main;
            main.startColor = color;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
            int count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++)
                particles[i].startColor = color;
            ps.SetParticles(particles, count);
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