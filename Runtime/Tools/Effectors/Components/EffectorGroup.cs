using Cysharp.Threading.Tasks;
using PrimeTween;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace EC.Effects
{
    [HideMonoScript]
    public class EffectorGroup : IEffectorComponent
    {
        #region Data
        [SerializeField, BoxGroup("Components", ShowLabel = false)] private IEffectorComponent[] _components;
        #endregion

        #region Moment Player
        public override void PlayMoment()
        {
            Stop();
            foreach (var component in _components)
                component.PlayMoment();
        }
        public override void PlayMomentCustom(float value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        public override void PlayMomentCustom(string value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        public override void PlayMomentCustom(Vector2 value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        public override void PlayMomentCustom(Vector3 value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        public override void PlayMomentCustom(Color value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        public override void PlayMomentCustom(Gradient value)
        {
            Stop();
            foreach (var component in _components)
                component.PlayMomentCustom(value);
        }
        #endregion

        #region Smooth Player
        private int DurationInTicks => Mathf.RoundToInt(_components.Max(v => v.GetTime().AllDuration) * 1000f);

        public override async UniTask PlaySmooth()
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmooth().Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(float value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(string value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(Gradient value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(Vector3 value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(Color value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        public override async UniTask PlaySmoothCustom(Vector2 value)
        {
            Stop();
            foreach (var component in _components)
                component.PlaySmoothCustom(value).Forget();
            await UniTask.Delay(DurationInTicks);
        }
        #endregion

        #region Stop|Pause|Resume
        public override void Stop()
        {
            foreach (var component in _components)
                component.Stop();
        }
        public override void Pause()
        {
            foreach (var component in _components)
                component.Pause();
        }
        public override void Resume()
        {
            foreach (var component in _components)
                component.Resume();
        }
        #endregion
    }
}