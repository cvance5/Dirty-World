using MathConcepts;
using Newtonsoft.Json;
using WorldObjects.Blocks;
using WorldObjects.Construction;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableBlock : ISerializable<Block>
    {
        [JsonProperty("type")]
        private readonly BlockTypes _type;
        [JsonProperty("position")]
        private readonly IntVector2 _position;
        [JsonProperty("health")]
        private readonly int _health;
        [JsonProperty("stability")]
        private readonly int _stability;

        [JsonConstructor]
        private SerializableBlock() { }

        public SerializableBlock(Block block)
        {
            _type = BlockLoader.ConvertToEnum(block.GetType());
            _position = block.Position;
            _health = block.Health;
            _stability = block.Stability;
        }

        public Block ToObject()
        {
            var block = BlockLoader.CreateBlock(_type, _position);
            block.Health = _health;
            block.Stability = _stability;

            return block;
        }
    }
}