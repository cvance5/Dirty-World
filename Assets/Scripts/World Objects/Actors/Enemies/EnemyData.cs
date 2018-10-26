using Data;

namespace WorldObjects.Actors.Enemies
{
    public abstract class EnemyData : ActorData
    {
        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);
        }

        protected override void OnWorldObjectDestroy()
        {
            PositionTracker.StopTracking(this);    
        }

        protected override void OnWorldObjectUnloaded()
        {
            PositionTracker.StopTracking(this);
        }
    }
}