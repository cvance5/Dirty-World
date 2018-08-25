using UnityEngine;

public class WaitForCustomCallback : CustomYieldInstruction
{
    public override bool keepWaiting => _keepWaiting;
    private bool _keepWaiting = true;

    public WaitForCustomCallback() { }

    public void Callback()
    {
        _keepWaiting = false;
    }
}
