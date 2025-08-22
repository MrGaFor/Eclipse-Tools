using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Effects
{
    [System.Serializable]
    public class EffectorComponentFuncData<TComponent, TFunc, TData> : EffectorComponentFunc<TComponent, TFunc>
        where TComponent : Object where TFunc : System.Enum
    {
        [BoxGroup("Data"), HorizontalGroup("Data/value"), HideLabel, PropertyOrder(-1)] public TData Value;
    }
    [System.Serializable]
    public class EffectorComponentFunc<TComponent, TFunc> : EffectorComponent<TComponent>
        where TComponent : Object where TFunc : System.Enum
    {
        [BoxGroup("Data"), HorizontalGroup("Data/value", 120), HideLabel, PropertyOrder(-1)] public TFunc Func;
    }
    [System.Serializable]
    public class EffectorComponentValue<TComponent, TData> : EffectorComponent<TComponent>
        where TComponent : Object
    {
        [BoxGroup("Data"), HorizontalGroup("Data/value"), HideLabel, PropertyOrder(-1)] public TData Value;
    }
    [System.Serializable]
    public class EffectorComponent<TComponent> : EffectorEmpty
        where TComponent : Object
    {
        [BoxGroup("Data", ShowLabel = false), HorizontalGroup("Data/value", 250), LabelWidth(70), PropertyOrder(-1)] public TComponent Component;
    }
    [System.Serializable]
    public class EffectorEmpty
    {
        [FoldoutGroup("Data/Settings"), HideLabel] public EffectSettingsCurveModule Curve;
        [FoldoutGroup("Data/Settings"), HideLabel] public EffectSettingsTimeModule Time;
        [FoldoutGroup("Data/Settings"), HideLabel] public EffectSettingsLoopModule Loop;
        [FoldoutGroup("Data/Settings"), HideLabel] public EffectSettingsStopModule Stop;
        [FoldoutGroup("Data/Settings"), HideLabel] public EffectLifeEventModule Events;
    }
        #region Modules
        [System.Serializable]
        public class EffectSettingsCurveModule
        {
            public enum CurveTypes { Ease, Custom }
            [HorizontalGroup("curve", 140), LabelWidth(70), LabelText("Curve")] public CurveTypes CurveType;
            [HorizontalGroup("curve"), HideLabel, ShowIf("IsEaseCurve")] public PrimeTween.Ease Ease;
            [HorizontalGroup("curve"), HideLabel, ShowIf("IsCustomCurve")] public AnimationCurve Curve;
            private bool IsEaseCurve() => CurveType == CurveTypes.Ease;
            private bool IsCustomCurve() => CurveType == CurveTypes.Custom;
        }
        [System.Serializable]
        public class EffectSettingsTimeModule
        {
            [HorizontalGroup("delay"), LabelWidth(70), MinValue(0)] public float StartDelay;
            [HorizontalGroup("delay"), LabelWidth(70), MinValue(0)] public float EndDelay;
            [HorizontalGroup("duration"), LabelWidth(70), MinValue(0)] public float Duration;

            public float AllDuration => StartDelay + Duration + EndDelay;
        }
        [System.Serializable]
        public class EffectSettingsLoopModule
        {
            public enum LoopTypes { None, Custom, Loop }
            [HorizontalGroup("loop"), LabelWidth(70), LabelText("Loop")] public LoopTypes LoopType;
            [HorizontalGroup("loop", 30), HideLabel, ShowIf("IsLoopCustom")] public int LoopCount;
            [HorizontalGroup("loop"), HideLabel, HideIf("IsLoopNone")] public PrimeTween.CycleMode LoopMode;
            private bool IsLoopCustom() => LoopType == LoopTypes.Custom;
            private bool IsLoopNone() => LoopType == LoopTypes.None;
        }
        [System.Serializable]
        public class EffectSettingsStopModule
        {
            public enum KillAction { Stop, Complete }
            [HorizontalGroup("stoptype"), LabelWidth(70)] public KillAction StopType;
        }
        [System.Serializable]
        public class EffectLifeEventModule
        {
            #region Editor
#if UNITY_EDITOR
            [SerializeField, HorizontalGroup("evs"), Button("OnPlay"), GUIColor("@_isOnPlay == true ? new Color(0.5f, 1f, 0.5f, 1f) : Color.gray")] private void ChangeOnStart() { _isOnPlay = !_isOnPlay; _onPlay = new(); }
            [SerializeField, HorizontalGroup("evs"), Button("OnUpdate"), GUIColor("@_isOnUpdate == true ? new Color(0.5f, 1f, 0.5f, 1f) : Color.gray")] private void ChangeOnUpdate() { _isOnUpdate = !_isOnUpdate; _onUpdate = new(); }
            [SerializeField, HorizontalGroup("evs"), Button("OnComplete"), GUIColor("@_isOnComplete == true ? new Color(0.5f, 1f, 0.5f, 1f) : Color.gray")] private void ChangeOnComplete() { _isOnComplete = !_isOnComplete; _onComplete = new(); }
#endif
            #endregion

            [SerializeField, HideInInspector] private bool _isOnPlay;
            [SerializeField, HideInInspector] private bool _isOnUpdate;
            [SerializeField, HideInInspector] private bool _isOnComplete;

            [SerializeField, HorizontalGroup("ev", order: 1), HideLabel, ShowIf("@_isOnPlay")] private UnityEvent _onPlay;
            [SerializeField, HorizontalGroup("ev", order: 1), HideLabel, ShowIf("@_isOnUpdate")] private UnityEvent _onUpdate;
            [SerializeField, HorizontalGroup("ev", order: 1), HideLabel, ShowIf("@_isOnComplete")] private UnityEvent _onComplete;

            public void CallPlayEvent() { if (_isOnPlay) _onPlay?.Invoke(); }
            public void CallUpdateEvent() { if (_isOnUpdate) _onUpdate?.Invoke(); }
            public void CallCompleteEvent() { if (_isOnComplete) _onComplete?.Invoke(); }
        }
        #endregion
}