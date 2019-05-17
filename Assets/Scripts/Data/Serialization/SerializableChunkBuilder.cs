using MathConcepts;
using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.WorldGeneration;

namespace Data.Serialization
{
    public class SerializableChunkBuilder : ISerializable<ChunkBuilder>
    {
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonProperty("chunkSize")]
        private readonly int _chunkSize;

        [JsonProperty("enemies")]
        private List<SerializableEnemy> _enemies = new List<SerializableEnemy>();

        [JsonProperty("spaceNames")]
        private List<string> _spaceNames = new List<string>();

        [JsonConstructor]
        private SerializableChunkBuilder() { }

        public SerializableChunkBuilder(ChunkBuilder chunkBuilder)
        {
            _position = chunkBuilder.Position;
            _chunkSize = chunkBuilder.ChunkSize;

            _enemies = new List<SerializableEnemy>();
            foreach (var enemy in chunkBuilder.Enemies)
            {
                _enemies.Add(new SerializableEnemy(enemy));
            }

            foreach (var space in chunkBuilder.Spaces)
            {
                _spaceNames.Add(space.Name);
            }
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        public static SerializableChunkBuilder Deserialize(string chunkJson) => JsonConvert.DeserializeObject<SerializableChunkBuilder>(chunkJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        public ChunkBuilder ToObject()
        {
            var chunkBuilder = new ChunkBuilder(_position, _chunkSize);
            foreach (var serializedEnemy in _enemies)
            {
                var enemy = serializedEnemy.ToObject();
                chunkBuilder.AddEnemy(enemy);
            }

            foreach (var spaceName in _spaceNames)
            {
                chunkBuilder.AddSpace(GameManager.World.SpaceArchitect.GetSpaceByName(spaceName));
            }

            return chunkBuilder;
        }
    }
}