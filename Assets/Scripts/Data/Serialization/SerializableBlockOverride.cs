using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Blocks;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableBlockOverride : ISerializable<KeyValuePair<IntVector2, BlockTypes>>
    {
        [JsonProperty("position")]
        private readonly IntVector2 _position;
        [JsonProperty("type")]
        private readonly BlockTypes _type;

        public SerializableBlockOverride(KeyValuePair<IntVector2, BlockTypes> blockOverride)
        {
            _position = blockOverride.Key;
            _type = blockOverride.Value;
        }

        public KeyValuePair<IntVector2, BlockTypes> ToObject() => new KeyValuePair<IntVector2, BlockTypes>(_position, _type);
    }
}