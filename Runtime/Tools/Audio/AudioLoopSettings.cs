using Sirenix.OdinInspector;
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

        public void Apply(AudioSource source)
        {
            if (_loop)
            {
                source.loop = true;
                if (_loopType == LoopType.Limited)
                    _disableTimers.Add(source, Coroutine.Coroutines.I.StartCoroutine(EndTimerDisable(source, _loopCount)));
            }
            else
            {
                source.loop = false;
                _disableTimers.Add(source, Coroutine.Coroutines.I.StartCoroutine(EndTimerDisable(source)));
            }
        }
        public void Stop(AudioSource source)
        {
            if (_disableTimers.ContainsKey(source))
            {
                Coroutine.Coroutines.I.StopCoroutine(_disableTimers[source]);
                _disableTimers.Remove(source);
            }
            source.gameObject.SetActive(false);
        }
        private IEnumerator EndTimerDisable(AudioSource source, int repeating = 1)
        {
            yield return new WaitForSecondsRealtime((source.clip.length * repeating) / Mathf.Abs(source.pitch));
            _disableTimers.Remove(source);
            source.gameObject.SetActive(false);
        }

    }
}
