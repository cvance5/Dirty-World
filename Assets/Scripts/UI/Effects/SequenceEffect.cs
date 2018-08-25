using System;
using System.Collections.Generic;

namespace UI.Effects
{
    public class SequenceEffect : UIEffect
    {
        private bool _isPlaying;
        private Action _onCompleteCallback;
        private readonly Queue<UIEffect> _sequencedEffects;

        public SequenceEffect(params UIEffect[] effects)
        {
            _sequencedEffects = new Queue<UIEffect>(effects);
        }

        public override void Play(Action OnCompleteCallback = null)
        {
            if (!_isPlaying)
            {
                _onCompleteCallback = OnCompleteCallback;
                _isPlaying = true;
                PlayNext();
            }
            else _log.Warning($"The sequence is already playing!");
        }

        private void PlayNext()
        {
            if (_sequencedEffects.Count > 0)
            {
                var nextEffect = _sequencedEffects.Dequeue();
                nextEffect.Play(PlayNext);
            }
            else
            {
                _isPlaying = false;
                _onCompleteCallback?.Invoke();
            }
        }

        private static readonly Log _log = new Log("SequenceEffect");
    }
}