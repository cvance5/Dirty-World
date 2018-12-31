using Newtonsoft.Json;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public class SerializableRoom : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;

        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonConstructor]
        private SerializableRoom() { }

        public SerializableRoom(Room room)
        {
            _bottomLeftCorner = room.BottomLeftCorner;
            _topRightCorner = room.TopRightCorner;
        }

        protected override Space ToRawObject() => new Room(_bottomLeftCorner, _topRightCorner);
    }
}