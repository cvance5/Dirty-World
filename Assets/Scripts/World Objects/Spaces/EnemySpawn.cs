using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.Spaces
{
    public class EnemySpawn
    {
        public IntVector2 Position { get; }
        public EnemyTypes Type { get; }

        public EnemySpawn(IntVector2 position, EnemyTypes type)
        {
            Position = position;
            Type = type;
        }
    }
}