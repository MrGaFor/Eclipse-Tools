using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EC.Mini
{
    [HideMonoScript]
    public class FloatCurveConverter : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings", false), HideLabel] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/curve"), LabelWidth(70)] private float _multiple = 1f;
        [SerializeField, BoxGroup("Settings"), HorizontalGroup("Settings/curve"), LabelWidth(70)] private float _divide = 1f;
        [SerializeField, BoxGroup("Settings"), FoldoutGroup("Settings/Events")] private UnityEvent<float> _onInvoke;

        public void Invoke(float value)
        {
            _onInvoke?.Invoke(_curve.Evaluate(value / _divide) * _multiple);
        }

    }
}
