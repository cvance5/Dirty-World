using MathConcepts;
using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.Spaces;

namespace Data.Serialization
{
    public class SerializableSpace : ISerializable<Space>
    {
        [JsonProperty("name")]
        private string _name;

        [JsonProperty("extents")]
        private SerializableExtents _extents;

        [JsonProperty("enemySpawns")]
        private List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();

        [JsonProperty("blockOverrides")]
        private List<SerializableBlockOverride> _blockOverrides = new List<SerializableBlockOverride>();

        [JsonProperty("modifiers")]
        private List<ModifierTypes> _modifiers = new List<ModifierTypes>();

        [JsonConstructor]
        private SerializableSpace() { }

        public SerializableSpace(Space space)
        {
            _name = space.Name;
            _extents = new SerializableExtents(space.Extents);

            _enemySpawns = space.EnemySpawns;
            foreach(var blockOverride in space.BlockOverrides)
            {
                _blockOverrides.Add(new SerializableBlockOverride(blockOverride));
            }
            _modifiers = space.Modifiers;
        }

        public Space ToObject()
        {
            var space = new Space(_name, _extents.ToObject());
            space.AddEnemySpawns(_enemySpawns);
            var blockOverrides = new Dictionary<IntVector2, BlockTypes>();
            foreach(var serializedBlockOverride in _blockOverrides)
            {
                var blockOverride = serializedBlockOverride.ToObject();
                blockOverrides.Add(blockOverride.Key, blockOverride.Value);
            }
            space.AddBlockOverrides(blockOverrides);
            space.AddModifiers(_modifiers);

            return space;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static SerializableSpace Deserialize(string spaceJson) => JsonConvert.DeserializeObject<SerializableSpace>(spaceJson);
    }
}