using Data.Serialization.SerializableSpaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects;
using WorldObjects.Spaces;

namespace Data.Serialization
{
    public class SerializableChunkBlueprint : ISerializable<ChunkBlueprint>
    {
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonProperty("botLeft")]
        private readonly IntVector2 _bottomLeft;

        [JsonProperty("topRight")]
        private readonly IntVector2 _topRight;

        [JsonProperty("spaces")]
        private readonly List<SerializableSpace> _spaces = new List<SerializableSpace>();

        [JsonProperty("enemies")]
        private readonly List<SerializableEnemy> _enemies = new List<SerializableEnemy>();

        public SerializableChunkBlueprint(ChunkBlueprint chunkBlueprint)
        {
            _position = chunkBlueprint.Position;
            _topRight = chunkBlueprint.TopRightCorner;
            _bottomLeft = chunkBlueprint.BottomLeftCorner;

            _spaces = new List<SerializableSpace>();
            foreach (var space in chunkBlueprint.Spaces)
            {
                _spaces.Add(ToSerializableSpace(space));
            }

            _enemies = new List<SerializableEnemy>();
            foreach (var enemy in chunkBlueprint.Enemies)
            {
                _enemies.Add(new SerializableEnemy(enemy));
            }
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        public static SerializableChunk Deserialize(string chunkJson) => JsonConvert.DeserializeObject<SerializableChunk>(chunkJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        public ChunkBlueprint ToObject()
        {
            var chunkBlueprint = new ChunkBlueprint(_position, _bottomLeft, _topRight);

            foreach (var serializedSpace in _spaces)
            {
                var space = serializedSpace.ToObject();
                chunkBlueprint.Register(space);
            }

            foreach (var serializedEnemy in _enemies)
            {
                var enemy = serializedEnemy.ToObject();
                chunkBlueprint.Register(enemy);
            }

            return chunkBlueprint;
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
            else if(space is MonsterDen)
            {
                return new SerializableMonsterDen(space as MonsterDen);
            }
            else throw new System.Exception($"Unknown space type: {space.GetType().Name}. Cannot serialize.");
        }
    }
}