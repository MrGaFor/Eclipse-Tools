using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Audio
{
    [System.Serializable]
    public class Audio3DSettings : AudioModule
    {
        [SerializeField, LabelWidth(100)] private bool _3D = false;
        [SerializeField, LabelWidth(100), ShowIf("_3D"), Range(0f, 5f)] private float _dopplerLevel = 1f;
        [SerializeField, LabelWidth(100), ShowIf("_3D"), Range(0f, 360f)] private float _spread = 0f;
        [SerializeField, LabelWidth(100), ShowIf("_3D"), Min(0)] private float _minDistance = 0f;
        [SerializeField, LabelWidth(100), ShowIf("_3D")] private float _maxDistance = 30f;
        [SerializeField, LabelWidth(100), ShowIf("_3D")] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField, LabelWidth(100), ShowIf("@_rolloffMode == AudioRolloffMode.Custom && _3D")] private AnimationCurve _customRolloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public void Apply(AudioSource source)
        {
            if (!_3D)
            {
                source.spatialBlend = 0f;
            }
            else
            {
                source.spatialBlend = 1f;
                source.dopplerLevel = _dopplerLevel;
                source.spread = _spread;
                source.minDistance = _minDistance;
                source.maxDistance = _maxDistance;
                source.rolloffMode = _rolloffMode;
                if (_rolloffMode == AudioRolloffMode.Custom) source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _customRolloffCurve);
            }
        }

        #region EDITOR
#if UNITY_EDITOR
        public void OnDrawGizmosSelected(Vector3 position)
        {
            if (_3D)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(position, _minDistance);
                Gizmos.DrawWireSphere(position, _maxDistance);
            }
        }
#endif
        #endregion
    }
}
