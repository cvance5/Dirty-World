using Data.Serialization.SerializableSpaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects;
using WorldObjects.Spaces;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableChunk : ISerializable<Chunk>
    {
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonProperty("topRight")]
        private readonly IntVector2 _topRight;

        [JsonProperty("botLeft")]
        private readonly IntVector2 _bottomLeft;

        [JsonProperty("blocks")]
        private readonly List<SerializableBlock> _blocks;

        [JsonProperty("spaces")]
        private readonly List<SerializableSpace> _spaces;

        [JsonProperty("hazards")]
        private readonly List<SerializableHazard> _hazards;

        [JsonConstructor]
        public SerializableChunk() { }

        public SerializableChunk(Chunk chunk)
        {
            _position = chunk.Position;
            _topRight = chunk.TopRightCorner;
            _bottomLeft = chunk.BottomLeftCorner;

            _blocks = new List<SerializableBlock>();
            foreach (var kvp in chunk.BlockMap)
            {
                _blocks.Add(new SerializableBlock(kvp.Value));
            }

            _spaces = new List<SerializableSpace>();
            foreach (var space in chunk.Spaces)
            {
                _spaces.Add(ToSerializableSpace(space));
            }

            _hazards = new List<SerializableHazard>();
            foreach (var hazard in chunk.Hazards)
            {
                _hazards.Add(new SerializableHazard(hazard));
            }
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        public static SerializableChunk Deserialize(string chunkJson) => JsonConvert.DeserializeObject<SerializableChunk>(chunkJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        public Chunk ToObject()
        {
            var chunk = new UnityEngine.GameObject($"Chunk [{_position.X}, {_position.Y}]").AddComponent<Chunk>();
            chunk.transform.position = _position;
            chunk.AssignExtents(_bottomLeft, _topRight);

            foreach (var serializableBlock in _blocks)
            {
                var block = serializableBlock.ToObject();
                chunk.Register(block);
            }

            foreach (var serializedSpace in _spaces)
            {
                var space = serializedSpace.ToObject();
                chunk.Register(space);
            }

            foreach (var serializedHazard in _hazards)
            {
                var hazard = serializedHazard.ToObject();
                chunk.Register(hazard);
            }

            return chunk;
        }

        private SerializableSpace ToSerializableSpace(Space space)
        {
            if (space is Shaft)
            {
                return new SerializableShaft(space as Shaft);
            }
            else if (space is Corridor)
            {
                return new SerializableCorridor(space as Corridor);
            }
            else throw new System.Exception($"Unknown space type: {space.GetType().Name}. Cannot serialize.");
        }
    }
}