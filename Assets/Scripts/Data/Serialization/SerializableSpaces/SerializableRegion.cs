using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableRegion : ISerializable<Region>
    {
        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;

        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonProperty("spaces")]
        private List<SerializableSpace> _spaces = new List<SerializableSpace>();

        [JsonConstructor]
        private SerializableRegion() { }

        public SerializableRegion(Region region)
        {
            _bottomLeftCorner = region.BottomLeftCorner;
            _topRightCorner = region.TopRightCorner;

            foreach (var space in region.Spaces)
            {
                _spaces.Add(SerializableSpaceHelper.ToSerializableSpace(space));
            }
        }

        public Region ToObject()
        {
            var spaces = new List<Space>();
            foreach (var space in _spaces)
            {
                spaces.Add(space.ToObject());
            }
            return new Region(_bottomLeftCorner, _topRightCorner, spaces);
        }
    }
}