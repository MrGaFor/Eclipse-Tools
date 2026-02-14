using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorLayoutElement : IEffectorComponent
    {
        #region Data
        private enum FuncList { MinWidth, MinHeight, PreferredWidth, PreferredHeight, FlexibleWidth, FlexibleHeight }
        public override bool ThisFloat => true;

        [SerializeField, HideLabel, ShowIf("ThisFloat")] private EffectorComponentFuncData<UnityEngine.UI.LayoutElement, FuncList, float> _dataFloat;
        
        public override EffectorEmpty Data => _data; private EffectorComponentFunc<UnityEngine.UI.LayoutElement, FuncList> _data => _dataFloat;
        public UnityEngine.UI.LayoutElement Component => _data?.Component;
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
                case FuncList.MinWidth:
                    _data.Component.minWidth = value; break;
                case FuncList.MinHeight:
                    _data.Component.minHeight = value; break;
                case FuncList.PreferredWidth:
                    _data.Component.preferredWidth = value; break;
                case FuncList.PreferredHeight:
                    _data.Component.preferredHeight = value; break;
                case FuncList.FlexibleWidth:
                    _data.Component.flexibleWidth = value; break;
                case FuncList.FlexibleHeight:
                    _data.Component.flexibleHeight = value; break;
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
            else if (SmoothFloatPart(value, duration))
                await EffectTween;
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
                case FuncList.MinWidth:
                    EffectTween = Tween.Custom(_data.Component.minWidth, value, CompiledSettings, newvalue => _data.Component.minWidth = newvalue); break;
                case FuncList.MinHeight:
                    EffectTween = Tween.Custom(_data.Component.minHeight, value, CompiledSettings, newvalue => _data.Component.minHeight = newvalue); break;
                case FuncList.PreferredWidth:
                    EffectTween = Tween.Custom(_data.Component.preferredWidth, value, CompiledSettings, newvalue => _data.Component.preferredWidth = newvalue); break;
                case FuncList.PreferredHeight:
                    EffectTween = Tween.Custom(_data.Component.preferredHeight, value, CompiledSettings, newvalue => _data.Component.preferredHeight = newvalue); break;
                case FuncList.FlexibleWidth:
                    EffectTween = Tween.Custom(_data.Component.flexibleWidth, value, CompiledSettings, newvalue => _data.Component.flexibleWidth = newvalue); break;
                case FuncList.FlexibleHeight:
                    EffectTween = Tween.Custom(_data.Component.flexibleHeight, value, CompiledSettings, newvalue => _data.Component.flexibleHeight = newvalue); break;
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