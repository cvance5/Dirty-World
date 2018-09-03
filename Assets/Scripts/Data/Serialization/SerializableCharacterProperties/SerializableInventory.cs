using Newtonsoft.Json;
using Player;

namespace Data.Serialization.SerializableCharacterProperties
{
    public class SerializableInventory : ISerializable<Inventory>
    {
        public SerializableInventory(Inventory inventory)
        {

        }

        public static SerializableInventory Deserialize(string inventoryJson) => JsonConvert.DeserializeObject<SerializableInventory>(inventoryJson);

        public Inventory ToObject()
        {
            throw new System.NotImplementedException();
        }
    }
}