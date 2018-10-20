using Characters;
using Data.Serialization.SerializableCharacterProperties;
using Newtonsoft.Json;

namespace Data.Serialization
{
    public class SerializableCharacter : ISerializable<Character>
    {
        [JsonProperty("inventory")]
        public SerializableInventory Inventory;

        [JsonProperty("equipment")]
        public SerializableEquipment Equipment;

        [JsonProperty("metadata")]
        public SerializableMetadata Metadata;

        [JsonConstructor]
        private SerializableCharacter() { }

        public SerializableCharacter(Character character)
        {
            Inventory = new SerializableInventory(character.Inventory);
            Equipment = new SerializableEquipment(character.Equipment);
            Metadata = new SerializableMetadata(character.Metadata);
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static SerializableCharacter Deserialize(string characterJson) => JsonConvert.DeserializeObject<SerializableCharacter>(characterJson);

        public Character ToObject()
        {
            return new Character()
            {
                Inventory = Inventory.ToObject(),
                Equipment = Equipment.ToObject(),
                Metadata = Metadata.ToObject()
            };
        }
    }
}