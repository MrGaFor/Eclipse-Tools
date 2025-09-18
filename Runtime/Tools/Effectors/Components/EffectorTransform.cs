using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorTransform : IEffectorComponent
    {
        #region Data
        private enum FuncList { Position, LocalPosition, Rotate, LocalRotate, Scale }
        public override bool ThisVector3 => true;

        [SerializeField, HideLabel, OnValueChanged("Vector3Update", IncludeChildren = true), ShowIf("ThisVector3")] private EffectorComponentFuncData<Transform, FuncList, Vector3> _dataVector3; public virtual void Vector3Update() { base.MarkDirty(); }
        public override EffectorEmpty Data => _data; private EffectorComponentFunc<Transform, FuncList> _data => ThisVector3 ? _dataVector3 : null;
        public Transform Component => _data?.Component;
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
            PlayMomentCustom(_dataVector3.Value);
        }
        public override void PlayMomentCustom(Vector3 value)
        {
            if (!ThisVector3) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Position:
                    _data.Component.position = value; break;
                case FuncList.LocalPosition:
                    _data.Component.localPosition = value; break;
                case FuncList.Rotate:
                    _data.Component.rotation = Quaternion.Euler(value); break;
                case FuncList.LocalRotate:
                    _data.Component.localRotation = Quaternion.Euler(value); break;
                case FuncList.Scale:
                    _data.Component.localScale = value; break;
            }
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override void PlaySmooth()
        {
            PlaySmoothCustom(_dataVector3.Value);
        }
        public override void PlaySmoothCustom(Vector3 value, float duration)
        {
            SmoothVectorPart(value, duration);
        }

        public override async UniTask PlaySmoothAsync()
        {
            await PlaySmoothCustomAsync(_dataVector3.Value);
        }
        public override async UniTask PlaySmoothCustomAsync(Vector3 value, float duration)
        {
            if (SmoothVectorPart(value, duration))
                await EffectTween;
        }

        private bool SmoothVectorPart(Vector3 value, float duration)
        {
            if (!ThisVector3) return false;
            StartPlaySmooth();
            float buffDuration = CompiledSettings.duration;
            if (duration != CompiledSettings.duration) CompiledSettings.duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Position:
                    EffectTween = Tween.Position(_data.Component, value, CompiledSettings); break;
                case FuncList.LocalPosition:
                    EffectTween = Tween.LocalPosition(_data.Component, value, CompiledSettings); break;
                case FuncList.Rotate:
                    EffectTween = Tween.Rotation(_data.Component, value, CompiledSettings); break;
                case FuncList.LocalRotate:
                    EffectTween = Tween.LocalRotation(_data.Component, value, CompiledSettings); break;
                case FuncList.Scale:
                    EffectTween = Tween.Scale(_data.Component, value, CompiledSettings); break;
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