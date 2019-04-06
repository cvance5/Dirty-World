using Newtonsoft.Json;
using WorldObjects.Spaces;

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
        private SerializableCorridor() { }

        public SerializableCorridor(Corridor corridor)
            : base(corridor)
        {
            _bottomLeftCorner = corridor.BottomLeftCorner;
            _topRightCorner = corridor.TopRightCorner;
        }

        protected override Space ToRawObject()
        {
            Space corridor = new Corridor(_bottomLeftCorner, _topRightCorner);
            return corridor;
        }
    }
}