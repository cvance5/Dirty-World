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

        public Space ToObject()
        {
            var rawSpace = ToRawObject();

            rawSpace.AddEnemySpawns(_enemySpawns);
            rawSpace.AddModifiers(_modifiers);

            return rawSpace;
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

        protected abstract Space ToRawObject();
    }
}