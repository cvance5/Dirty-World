using Newtonsoft.Json;
using System.Collections.Generic;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        [JsonProperty("enemySpawns")]
        protected List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();

        [JsonProperty("modifiers")]
        protected List<ModifierTypes> _modifiers = new List<ModifierTypes>();

        public abstract Space ToObject();
    }
}