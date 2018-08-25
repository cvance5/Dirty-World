using System;

namespace UI
{
    public abstract class UIEffect
    {
        public abstract void Play(Action OnCompleteCallback = null);
    }
}