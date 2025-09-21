using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EC.Audio
{
    [System.Serializable]
    public class AudioLoopSettings : AudioModule
    {
        [SerializeField, LabelWidth(100), HorizontalGroup("loop", 122)] private bool _loop = false;
        private enum LoopType { Unlimited, Limited }
        [SerializeField, LabelWidth(50), HorizontalGroup("loop"), ShowIf("_loop"), LabelText("Type")] private LoopType _loopType = LoopType.Unlimited;
        [SerializeField, LabelWidth(50), HorizontalGroup("loop"), ShowIf("@_loop && _loopType == LoopType.Limited"), LabelText("Count"), Min(1)] private int _loopCount = 1;

        private Dictionary<AudioSource, UnityEngine.Coroutine> _disableTimers = new();

        public void Apply(AudioSource source, AudioFadeSettings fade)
        {
            if (_loop)
            {
                source.loop = true;
                if (_loopType == LoopType.Limited)
                    _disableTimers.Add(source, Coroutine.Coroutines.I.StartCoroutine(EndTimerDisable(source, fade, _loopCount)));
            }
            else
            {
                source.loop = false;
                _disableTimers.Add(source, Coroutine.Coroutines.I.StartCoroutine(EndTimerDisable(source, fade)));
            }
        }
        public void Stop(AudioSource source, AudioFadeSettings fade)
        {
            if (_disableTimers.ContainsKey(source))
            {
                Coroutine.Coroutines.I.StopCoroutine(_disableTimers[source]);
                fade.Stop(source);
                _disableTimers.Remove(source);
            }
        }
        private IEnumerator EndTimerDisable(AudioSource source, AudioFadeSettings fade, int repeating = 1)
        {
            yield return new WaitForSecondsRealtime((source.clip.length * repeating) / Mathf.Abs(source.pitch) - fade.OutDuration);
            fade.Stop(source);
            _disableTimers.Remove(source);
        }

    }
}
