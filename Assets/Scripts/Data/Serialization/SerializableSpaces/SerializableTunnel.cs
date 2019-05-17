using MathConcepts;
using Newtonsoft.Json;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableTunnel : SerializableSpace
    {
        [JsonProperty("bottomLeftCorner")]
        protected readonly IntVector2 _bottomLeftCorner;
        [JsonProperty("topRightCorner")]
        protected readonly IntVector2 _topRightCorner;

        [JsonConstructor]
        protected SerializableTunnel() { }

        public SerializableTunnel(Tunnel tunnel)
            : base(tunnel)
        {
            _bottomLeftCorner = tunnel.BottomLeftCorner;
            _topRightCorner = tunnel.TopRightCorner;
        }

        protected override Space ToRawObject()
        {
            Space corridor = new Tunnel(_bottomLeftCorner, _topRightCorner);
            return corridor;
        }
    }
}