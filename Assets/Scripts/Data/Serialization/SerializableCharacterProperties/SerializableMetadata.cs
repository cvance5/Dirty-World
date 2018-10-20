using Newtonsoft.Json;
using System;

namespace Data.Serialization.SerializableCharacterProperties
{
    public class SerializableMetadata : ISerializable<Characters.Metadata>
    {
        [JsonProperty("playtime")]
        public double TimePlayed;

        [JsonConstructor]
        private SerializableMetadata() { }

        public SerializableMetadata(Characters.Metadata metadata)
        {
            TimePlayed = metadata.TimePlayed;
        }

        public Characters.Metadata ToObject()
        {
            var metadata = new Characters.Metadata();
            metadata.AddTimePlayed(TimePlayed);
            return metadata;
        }
    }
}