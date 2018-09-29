namespace Actors.Player
{
    public class PlayerFeet : TriggerList2D
    {
        public SmartEvent OnFootTouch = new SmartEvent();
        public SmartEvent OnFootLeave = new SmartEvent();

        public bool IsColliding => _overlaps.Count > 0;

        protected override void UpdateOverlaps()
        {
            base.UpdateOverlaps();
            if (IsColliding)
            {
                OnFootTouch.Raise();
            }
            else OnFootTouch.Raise();
        }
    }
}