using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace EC.Scenes
{
    [HideMonoScript]
    public class SceneLoaderUI : MonoBehaviour
    {
        public int ShowDuration => Mathf.RoundToInt(_onShow.Max(v => v.GetTime().AllDuration) * 1000f);
        public int HideDuration => Mathf.RoundToInt(_onHide.Max(v => v.GetTime().AllDuration) * 1000f);

        [SerializeField, HorizontalGroup("Settings/effs")] private EC.Effects.IEffectorComponent[] _onShow;
        [SerializeField, HorizontalGroup("Settings/effs")] private EC.Effects.IEffectorComponent[] _onHide;

        public async UniTask PlaySmooth(bool isShowing)
        {
            foreach (var eff in (isShowing ? _onShow : _onHide))
                eff.PlaySmoothAsync().Forget();
            await UniTask.Delay(isShowing ? ShowDuration : HideDuration);
        }
        public void PlayMoment(bool isShowing)
        {
            foreach (var eff in (isShowing ? _onShow : _onHide))
                eff.PlayMoment();
        }

    }
}
