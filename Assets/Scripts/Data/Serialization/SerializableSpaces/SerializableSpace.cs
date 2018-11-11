using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace Data.Serialization.SerializableSpaces
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        [JsonProperty("enemySpawns")]
        protected Dictionary<IntVector2, EnemyTypes> _enemySpawns
            = new Dictionary<IntVector2, EnemyTypes>();

        [JsonProperty("modifiers")]
        protected List<ModifierTypes> _modifiers = new List<ModifierTypes>();

        public abstract Space ToObject();
    }
}