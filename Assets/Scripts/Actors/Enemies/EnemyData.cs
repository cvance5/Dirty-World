namespace Actors.Enemies
{
    public class EnemyData : ActorData
    {
        protected override void Die()
        {
            Destroy(gameObject);
        }
    }
}