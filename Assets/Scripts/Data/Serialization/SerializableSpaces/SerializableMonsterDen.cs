using MathConcepts;
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
        private SerializableMonsterDen() { }

        public SerializableMonsterDen(MonsterDen monsterDen)
            : base(monsterDen)
        {
            _centerpoint = monsterDen.Centerpoint;
            _radius = monsterDen.Radius;
        }

        protected override Space ToRawObject()
        {
            Space monsterDen = new MonsterDen(_centerpoint, _radius);
            return monsterDen;
        }
    }
}