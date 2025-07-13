using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Scenes
{
    [HideMonoScript]
    public class SceneLoaderUI : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings", ShowLabel = false), HorizontalGroup("Settings/dur"), LabelWidth(100)] private float _showDuration = 0.2f; public float ShowDuration => _showDuration;
        [SerializeField, HorizontalGroup("Settings/dur"), LabelWidth(100)] private float _hideDuration = 0.2f; public float HideDuration => _hideDuration;

        [SerializeField, HorizontalGroup("Settings/effs")] private EC.Effects.IEffectorComponent[] _onShow;
        [SerializeField, HorizontalGroup("Settings/effs")] private EC.Effects.IEffectorComponent[] _onHide;

        public void PlaySmooth(bool isShowing)
        {
            foreach (var eff in (isShowing ? _onShow : _onHide))
                eff.PlaySmooth();
        }
        public void PlayMoment(bool isShowing)
        {
            foreach (var eff in (isShowing ? _onShow : _onHide))
                eff.PlayMoment();
        }

    }
}
