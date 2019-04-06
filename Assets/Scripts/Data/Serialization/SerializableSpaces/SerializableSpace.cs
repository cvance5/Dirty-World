using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        [JsonProperty("enemySpawns")]
        protected List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();

        [JsonProperty("blockOverrides")]
        protected List<SerializableBlockOverride> _blockOverrides = new List<SerializableBlockOverride>();

        [JsonProperty("modifiers")]
        protected List<ModifierTypes> _modifiers = new List<ModifierTypes>();

        [JsonConstructor]
        protected SerializableSpace() { }

        public SerializableSpace(Space space)
        {
            _enemySpawns = space.EnemySpawns;
            foreach(var blockOverride in space.BlockOverrides)
            {
                _blockOverrides.Add(new SerializableBlockOverride(blockOverride));
            }
            _modifiers = space.Modifiers;
        }

        public Space ToObject()
        {
            var rawSpace = ToRawObject();

            rawSpace.AddEnemySpawns(_enemySpawns);
            var blockOverrides = new Dictionary<IntVector2, BlockTypes>();
            foreach(var serializedBlockOverride in _blockOverrides)
            {
                var blockOverride = serializedBlockOverride.ToObject();
                blockOverrides.Add(blockOverride.Key, blockOverride.Value);
            }
            rawSpace.AddBlockOverrides(blockOverrides);
            rawSpace.AddModifiers(_modifiers);

            return rawSpace;
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        public static SerializableSpace Deserialize(string spaceJson) => JsonConvert.DeserializeObject<SerializableSpace>(spaceJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

        protected abstract Space ToRawObject();
    }
}