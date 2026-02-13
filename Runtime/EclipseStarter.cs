using UnityEngine;

namespace EC
{
    public class EclipseStarter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitAfterAssemblies()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            PrimeTween.PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            PrimeTween.PrimeTweenConfig.warnZeroDuration = false;
            PrimeTween.PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            PrimeTween.PrimeTweenConfig.warnStructBoxingAllocationInCoroutine = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAfterScene()
        {
            Updater.UpdaterCore.Init();
            Coroutine.Coroutines.Init();
            GPU.GPUInstance.Init();
            Inputer.InputKeyController.Init();
        }
    }
}
