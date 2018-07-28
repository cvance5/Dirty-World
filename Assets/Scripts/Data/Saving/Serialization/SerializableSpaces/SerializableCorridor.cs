using Newtonsoft.Json;
using WorldObjects;
using WorldObjects.Spaces;

namespace Data.Saving.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableCorridor : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;
        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;
        [JsonProperty("isHazardous")]
        private readonly bool _isHazardous;

        [JsonConstructor]
        public SerializableCorridor() { }

        public SerializableCorridor(Corridor corridor)
        {
            _bottomLeftCorner = corridor.BottomLeftCorner;
            _topRightCorner = corridor.TopRightCorner;
            _isHazardous = corridor.IsHazardous;
        }

        public override Space ToObject() => new Corridor(_bottomLeftCorner, _topRightCorner, _isHazardous);
    }
}