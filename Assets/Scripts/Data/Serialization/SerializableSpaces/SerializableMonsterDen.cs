using Newtonsoft.Json;
using WorldObjects.Spaces;

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
        }

        public override Space ToObject() => new MonsterDen(_centerpoint, _radius);
    }
}