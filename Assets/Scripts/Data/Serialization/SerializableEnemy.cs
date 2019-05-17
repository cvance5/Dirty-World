using WorldObjects.Actors.Enemies;
using Newtonsoft.Json;
using WorldObjects.WorldGeneration.EnemyGeneration;
using MathConcepts;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableEnemy : ISerializable<EnemyHealth>
    {
        [JsonProperty("type")]
        private readonly EnemyTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonConstructor]
        public SerializableEnemy() { }

        public SerializableEnemy(EnemyHealth enemy)
        {
            _type = EnemySpawner.ConvertToEnum(enemy.GetType());
            _position = enemy.Position;
        }

        public EnemyHealth ToObject()
        {
            var enemyData = EnemySpawner.SpawnEnemy(_type, _position);
            return enemyData;
        }
    }
}