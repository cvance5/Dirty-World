using Data;

namespace WorldObjects.Actors.Enemies
{
    public abstract class EnemyHealth : ActorHealth
    {
        protected override void OnActorAwake()
        {
            PositionTracker.BeginTracking(this);
            OnEnemyAwake();
        }

        protected abstract void OnEnemyAwake();

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