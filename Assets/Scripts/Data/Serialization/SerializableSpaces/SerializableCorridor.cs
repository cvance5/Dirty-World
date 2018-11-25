using Newtonsoft.Json;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;

namespace Data.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableCorridor : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;
        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonConstructor]
        public SerializableCorridor() { }

        public SerializableCorridor(Corridor corridor)
        {
            _bottomLeftCorner = corridor.BottomLeftCorner;
            _topRightCorner = corridor.TopRightCorner;

            _modifiers = corridor.Modifiers;
            _enemySpawns = corridor.EnemySpawns;
        }

        public override Space ToObject()
        {
            Space corridor = new Corridor(_bottomLeftCorner, _topRightCorner);
            corridor.AddEnemySpawns(_enemySpawns);
            return corridor;
        }
    }
}