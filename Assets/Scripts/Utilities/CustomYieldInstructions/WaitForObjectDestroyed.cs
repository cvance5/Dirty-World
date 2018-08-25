using UnityEngine;

namespace Utilities.CustomYieldInstructions
{
    public class WaitForObjectDestroyed : CustomYieldInstruction
    {
        public override bool keepWaiting => _target != null;

        private readonly Object _target;

        public WaitForObjectDestroyed(Object target)
        {
            _target = target;
        }
    }
}