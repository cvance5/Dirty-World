using System;

namespace UI.Effects
{
    public class InstantEffect : UIEffect
    {
        private readonly Action _effect;

        public InstantEffect(Action effect)
        {
            _effect = effect;
        }

        public override void Play(Action OnCompleteCallback = null)
        {
            _effect?.Invoke();
            OnCompleteCallback?.Invoke();
        }
    }
}