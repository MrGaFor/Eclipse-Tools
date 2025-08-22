using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorTextMeshProUGUI : IEffectorComponent
    {
        #region Data
        private enum FuncList { Text, Alpha, Color, FontSize, OutlineWidth }
        public override bool ThisFloat => _dataFloat.Func == FuncList.Alpha || _dataFloat.Func == FuncList.FontSize || _dataFloat.Func == FuncList.OutlineWidth;
        public override bool ThisString => _dataFloat.Func == FuncList.Text;
        public override bool ThisColor => _dataColor.Func == FuncList.Color;

        [SerializeField, HideLabel, OnValueChanged("FloatUpdate", IncludeChildren = true), ShowIf("ThisFloat")] private EffectorComponentFuncData<TMPro.TextMeshProUGUI, FuncList, float> _dataFloat; public virtual void FloatUpdate() { _dataColor.Func = _dataFloat.Func; _dataString.Func = _dataFloat.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("StringUpdate", IncludeChildren = true), ShowIf("ThisString")] private EffectorComponentFuncData<TMPro.TextMeshProUGUI, FuncList, string> _dataString; public virtual void StringUpdate() { _dataColor.Func = _dataString.Func; _dataFloat.Func = _dataString.Func; base.MarkDirty(); }
        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentFuncData<TMPro.TextMeshProUGUI, FuncList, Color> _dataColor; public virtual void ColorUpdate() { _dataFloat.Func = _dataColor.Func; _dataString.Func = _dataColor.Func; base.MarkDirty(); }

        public override EffectorEmpty Data => _data; private EffectorComponentFunc<TMPro.TextMeshProUGUI, FuncList> _data => ThisFloat ? _dataFloat : ThisColor ? _dataColor : ThisString ? _dataString : null;
        public TMPro.TextMeshProUGUI Component => _data?.Component;
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
            PlayMomentCustom(_dataString.Value);
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
                case FuncList.FontSize:
                    _data.Component.fontSize = value; break;
                case FuncList.OutlineWidth:
                    _data.Component.outlineWidth = value; break;
            }
            EndPlayMoment();
        }
        public override void PlayMomentCustom(string value)
        {
            if (!ThisString) return;
            StartPlayMoment();
            switch (_data.Func)
            {
                case FuncList.Text:
                    _data.Component.text = value; break;
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
        public override async UniTask PlaySmooth()
        {
            await PlaySmoothCustom(_dataFloat.Value);
            await PlaySmoothCustom(_dataString.Value);
            await PlaySmoothCustom(_dataColor.Value);
        }
        public override async UniTask PlaySmoothCustom(float value, float duration)
        {
            if (!ThisFloat) return;
            StartPlaySmooth();
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Alpha:
                    EffectTween = Tween.Alpha(_data.Component, value, CompiledSettings); break;
                case FuncList.FontSize:
                    EffectTween = Tween.Custom(_data.Component.fontSize, value, CompiledSettings, newvalue => _data.Component.fontSize = newvalue); break;
                case FuncList.OutlineWidth:
                    EffectTween = Tween.Custom(_data.Component.outlineWidth, value, CompiledSettings, newvalue => _data.Component.outlineWidth = newvalue); break;
                default:
                    used = false; break;
            }
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
            if (used) await EffectTween;
            EndPlaySmooth();
        }
        public override async UniTask PlaySmoothCustom(string value, float duration)
        {
            if (!ThisString) return;
            StartPlaySmooth();
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Text:
                    _data.Component.ForceMeshUpdate(); _data.Component.maxVisibleCharacters = 0; _data.Component.text = value; EffectTween = PrimeTween.Tween.TextMaxVisibleCharacters(_data.Component, value.Length, CompiledSettings); break;
                default:
                    used = false; break;
            }
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
            if (used) await EffectTween;
            EndPlaySmooth();
        }
        public override async UniTask PlaySmoothCustom(Color value, float duration)
        {
            if (!ThisColor) return;
            StartPlaySmooth();
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            bool used = true;
            switch (_data.Func)
            {
                case FuncList.Color:
                    EffectTween = Tween.Color(_data.Component, value, CompiledSettings); break;
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