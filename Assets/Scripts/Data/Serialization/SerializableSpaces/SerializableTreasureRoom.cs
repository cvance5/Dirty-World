using Items;
using Newtonsoft.Json;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableTreasureRoom : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;

        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonProperty("treasure")]
        private readonly Item[] _treasure;

        [JsonConstructor]
        private SerializableTreasureRoom() { }

        public SerializableTreasureRoom(TreasureRoom treasureRoom)
            :base(treasureRoom)
        {
            _bottomLeftCorner = treasureRoom.BottomLeftCorner;
            _topRightCorner = treasureRoom.TopRightCorner;
            _treasure = treasureRoom.Treasure;
        }

        protected override Space ToRawObject() => new TreasureRoom(_bottomLeftCorner, _topRightCorner, _treasure);
    }
}