using Newtonsoft.Json;
using WorldObjects;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableShaft : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;
        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonConstructor]
        public SerializableShaft() { }

        public SerializableShaft(Shaft shaft)
        {
            _bottomLeftCorner = shaft.BottomLeftCorner;
            _topRightCorner = shaft.TopRightCorner;
        }

        public override Space ToObject() => new Shaft(_bottomLeftCorner, _topRightCorner);
    }
}
