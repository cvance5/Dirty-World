using Data.Serialization.SerializableCharacterProperties;
using Newtonsoft.Json;
using Player;

namespace Data.Serialization
{
    public class SerializableCharacter : ISerializable<Character>
    {
        [JsonProperty("inventory")]
        public SerializableInventory Inventory;

        public SerializableCharacter(Character character)
        {
            Inventory = new SerializableInventory(character.Inventory);
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static SerializableCharacter Deserialize(string characterJson) => JsonConvert.DeserializeObject<SerializableCharacter>(characterJson);

        public Character ToObject()
        {
            return new Character()
            {
                Inventory = Inventory.ToObject()
            };
        }
    }
}