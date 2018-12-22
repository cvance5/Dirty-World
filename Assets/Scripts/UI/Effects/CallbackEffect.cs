using System;

namespace UI.Effects
{
    public class CallbackEffect : UIEffect
    {
        private readonly Action _effect;
        private Action _onCompleteCallback;

        public CallbackEffect(Action effect) => _effect = effect;

        public override void Play(Action OnCompleteCallback = null)
        {
            _onCompleteCallback = OnCompleteCallback;
            _effect?.Invoke();
        }

        public void Complete() => _onCompleteCallback?.Invoke();
    }
}