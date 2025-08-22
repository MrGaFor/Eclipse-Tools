using Sirenix.OdinInspector;
using UnityEngine;
using PrimeTween;
using Cysharp.Threading.Tasks;

namespace EC.Effects
{
    [ExecuteAlways, HideMonoScript]
    public class EffectorGradient : IEffectorComponent
    {
        #region Data
        public override bool ThisColor => true;

        [SerializeField, HideLabel, OnValueChanged("ColorUpdate", IncludeChildren = true), ShowIf("ThisColor")] private EffectorComponentValue<Material, Gradient> _dataColor; public virtual void ColorUpdate() { base.MarkDirty(); }

        public override EffectorEmpty Data => _data; private EffectorComponentValue<Material, Gradient> _data => ThisColor ? _dataColor : null;
        public Material Component => _data?.Component;

        private void Awake()
        {
            _gradientLast = _dataColor.Value;
            UpdateGradient(_gradientLast);
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
            PlayMomentCustom(_dataColor.Value);
        }
        public override void PlayMomentCustom(Gradient value)
        {
            if (!ThisFloat) return;
            StartPlayMoment();
            UpdateGradient(_dataColor.Value);
            EndPlayMoment();
        }
        #endregion

        #region Smooth Player
        public override async UniTask PlaySmooth()
        {
            await PlaySmoothCustom(_dataColor.Value);
        }
        public override async UniTask PlaySmoothCustom(Gradient value, float duration)
        {
            if (!ThisColor) return;
            StartPlaySmooth();
            float buffDuration = Data.Time.Duration;
            if (duration != Data.Time.Duration) Data.Time.Duration = duration;
            Gradient gradientLast = _gradientLast;
            EffectTween = Tween.Custom(0f, 1f, CompiledSettings, time => {  UpdateGradient(LerpGradient(gradientLast, value, time)); });
            if (buffDuration != Data.Time.Duration) Data.Time.Duration = buffDuration;
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

        #region UpdateGradient
        private Texture2D _gradientTexture;
        private Gradient _gradientLast;
        private void UpdateGradient(Gradient gradient)
        {
            if (_gradientTexture == null || _gradientTexture.width != 32)
            {
                _gradientTexture = new Texture2D(32, 1, TextureFormat.RGBA32, false);
                _gradientTexture.wrapMode = TextureWrapMode.Clamp;
                _gradientTexture.name = "GeneratedGradientTex";
            }

            for (int i = 0; i < 32; i++)
            {
                float t = i / (32 - 1f);
                Color color = gradient.Evaluate(t);
                _gradientTexture.SetPixel(i, 0, color);
            }

            _gradientTexture.Apply();

            if (_data.Component != null)
                _data.Component.SetTexture("_GradientTex", _gradientTexture);
            _gradientLast = gradient;
        }
        public static Gradient LerpGradient(Gradient a, Gradient b, float t)
        {
            t = Mathf.Clamp01(t);
            Gradient result = new Gradient();

            GradientColorKey[] aColors = a.colorKeys;
            GradientColorKey[] bColors = b.colorKeys;
            int count = Mathf.Max(aColors.Length, bColors.Length);
            GradientColorKey[] resultColors = new GradientColorKey[count];

            for (int i = 0; i < count; i++)
            {
                float time = i / (float)(count - 1);
                Color colA = a.Evaluate(time);
                Color colB = b.Evaluate(time);
                resultColors[i] = new GradientColorKey(Color.Lerp(colA, colB, t), time);
            }

            GradientAlphaKey[] resultAlphas = new GradientAlphaKey[count];
            for (int i = 0; i < count; i++)
            {
                float time = i / (float)(count - 1);
                float alphaA = a.Evaluate(time).a;
                float alphaB = b.Evaluate(time).a;
                resultAlphas[i] = new GradientAlphaKey(Mathf.Lerp(alphaA, alphaB, t), time);
            }

            result.SetKeys(resultColors, resultAlphas);
            return result;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private int _editorDeltaFrameCount = 0;
        private void OnDrawGizmos()
        {
            if (Application.isPlaying || _dataColor.Value == null) return;
            if (_editorDeltaFrameCount == 0 || !_gradientTexture)
            {
                UpdateGradient(_dataColor.Value);
                _editorDeltaFrameCount = 100;
            }
            else
                _editorDeltaFrameCount--;
        }
#endif
        #endregion
    }
}