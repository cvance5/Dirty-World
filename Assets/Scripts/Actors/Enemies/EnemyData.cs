using Data;

namespace Actors.Enemies
{
    public abstract class EnemyData : ActorData
    {
        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);
        }

        protected void OnDestroy()
        {
            PositionTracker.StopTracking(this);    
        }
    }
}