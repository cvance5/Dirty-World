using DG.Tweening;
using System;

namespace UI.Effects
{
    public class TweenEffect : UIEffect
    {
        private readonly Tween _tween;

        public TweenEffect(Tween tween)
        {
            _tween = tween;
            _tween.Pause();
        }

        public override void Play(Action OnCompleteCallback = null)
        {
            _tween.Play();
            _tween.onComplete = () => OnCompleteCallback?.Invoke();
        }
    }
}