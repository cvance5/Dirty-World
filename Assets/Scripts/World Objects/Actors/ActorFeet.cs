using UnityEngine;

namespace WorldObjects.Actors
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class ActorFeet : TriggerList2D
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
            else OnFootLeave.Raise();
        }
    }
}