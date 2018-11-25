using Newtonsoft.Json;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableMonsterDen : SerializableSpace
    {
        [JsonProperty("centerpoint")]
        private readonly IntVector2 _centerpoint;

        [JsonProperty("radius")]
        private readonly int _radius;

        [JsonConstructor]
        public SerializableMonsterDen() { }

        public SerializableMonsterDen(MonsterDen monsterDen)
        {
            _centerpoint = monsterDen.Centerpoint;
            _radius = monsterDen.Radius;

            _modifiers = monsterDen.Modifiers;
            _enemySpawns = monsterDen.EnemySpawns;
        }

        public override Space ToObject()
        {
            Space monsterDen = new MonsterDen(_centerpoint, _radius);
            monsterDen.AddEnemySpawns(_enemySpawns);
            return monsterDen;
        }
    }
}